/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2018 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 * 	
 * 	libfintx is free software; you can redistribute it and/or
 *	modify it under the terms of the GNU Lesser General Public
 * 	License as published by the Free Software Foundation; either
 * 	version 2.1 of the License, or (at your option) any later version.
 *	
 * 	libfintx is distributed in the hope that it will be useful,
 * 	but WITHOUT ANY WARRANTY; without even the implied warranty of
 * 	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * 	Lesser General Public License for more details.
 *	
 * 	You should have received a copy of the GNU Lesser General Public
 * 	License along with libfintx; if not, write to the Free Software
 * 	Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 * 	
 */

/*
 *
 *	Based on Olaf Willuhn's Java implementation of flicker codes for HHD 1.3 and HHD 1.4,
 *	available at https://github.com/willuhn/hbci4java/blob/master/src/org/kapott/hbci/manager/FlickerCode.java
 *
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace libfintx
{
    public class FlickerCode
    {
        private bool InstanceFieldsInitialized = false;

        private void InitializeInstanceFields()
        {
            startCode = new Startcode(this);
            de1 = new DE(this);
            de2 = new DE(this);
            de3 = new DE(this);
        }

        /// <summary>
        /// Version
        /// </summary>
        public enum HHDVersion
        {
            HHD14,
            HHD13
        }

        /// <summary>
        /// Encoding user data
        /// </summary>
        public enum Encoding
        {
            /// <summary>
            /// ASC-Encoding.
            /// </summary>
            ASC,

            /// <summary>
            /// BCD-Encoding.
            /// </summary>
            BCD,
        }

        /// Number of bytes 
        private const int LC_LENGTH_HHD14 = 3;

        /// <summary>
        /// Number of bytes 
        /// </summary>
        private const int LC_LENGTH_HHD13 = 2;

        /// <summary>
        /// The position of the bit that contains the encoding
        /// </summary>
        private const int BIT_ENCODING = 6;

        /// <summary>
        /// The position of the bit which determines whether a control byte follows
        /// </summary>
        private const int BIT_CONTROLBYTE = 7;

        /// <summary>
        /// HHD-Version
        /// </summary>
        public HHDVersion version;

        /// <summary>
        /// Length of the entire code
        /// </summary>
        public int lc = 0;

        /// <summary>
        /// Startcode
        /// </summary>
        public Startcode startCode;

        /// <summary>
        /// Data element 1
        /// </summary>
        public DE de1;

        /// <summary>
        /// Data element 2
        /// </summary>
        public DE de2;

        /// <summary>
        ///Data element 3
        /// </summary>
        public DE de3;

        /// <summary>
        /// Unused code
        /// </summary>
        public string rest = null;

        /// <summary>
        /// Parameter free constructor for manually merging code
        /// </summary>
        public FlickerCode()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        /// <summary>
        /// Parsing HHDuc-Code from given code
        /// </summary>
        public FlickerCode(string code)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }

            // Try HHD1.4 first
            try
            {
                Parse(code, HHDVersion.HHD14);
            }
            catch
            {
                // HHD 1.3 when HHD1.4 fails
                Parse(code, HHDVersion.HHD13);
            }
        }

        /// <summary>
        /// Parsing code with the specified HHD version.
        /// </summary>
        private void Parse(string code, HHDVersion version)
        {
            Reset();
            code = Clean(code);

            {
                // Trace LC
                int len = version == HHDVersion.HHD14 ? LC_LENGTH_HHD14 : LC_LENGTH_HHD13;
                this.lc = int.Parse(code.Substring(0, len));
                code = code.Substring(len);
            }

            // 2. Startcode / Control bytes
            code = this.startCode.Parse(code);

            // 3. LDE/DE 1-3
            code = this.de1.Parse(code);
            code = this.de2.Parse(code);
            code = this.de3.Parse(code);

            // 4.Chunk
            this.rest = code.Length > 0 ? code : null;
        }

        /// <summary>
        /// Remove CHLGUC0026....CHLGTEXT from code if exists
        /// </summary>
        private string Clean(string code)
        {
            code = code.Replace(" ", "");
            code = code.Trim();

            // Try CHLGUC0026....CHLGTEXT
            int t1Start = code.IndexOf("CHLGUC", StringComparison.Ordinal);
            int t2Start = code.IndexOf("CHLGTEXT", StringComparison.Ordinal);

            if (t1Start == -1 || t2Start == -1 || t2Start <= t1Start)
            {
                return code;
            }

            code = code.Substring(0, t2Start);
            code = code.Substring(t1Start);
            code = code.Substring(10);

            // Attach 0
            // HHD1.4
            return "0" + code;
        }

        /// <summary>
        /// Rendering code
        /// </summary>
        public virtual string Render()
        {
            // Payload
            string s = CreatePayload();

            // Luhn checksum
            string luhn = CreateLuhnChecksum();

            // 3. XOR checksum
            string xor = CreateXORChecksum(s);

            return s + luhn + xor;
        }

        private string CreatePayload()
        {
            StringBuilder sb = new StringBuilder();

            // Startcode
            sb.Append(this.startCode.RenderLength());

            // Control bytes
            foreach (int? i in this.startCode.controlBytes)
            {
                sb.Append(ToHex(i.Value, 2));
            }

            // Startcode
            sb.Append(this.startCode.RenderData());

            // Attach DEs
            DE[] deList = new DE[] { this.de1, this.de2, this.de3 };

            for (int i = 0; i < deList.Length; ++i)
            {
                DE de = deList[i];
                sb.Append(de.RenderLength());
                sb.Append(de.RenderData());
            }

            string s = sb.ToString();

            // Calculate the length and append it at the front
            int len = s.Length;
            len += 2;
            string lc = ToHex(len, 2);

            return (lc + s);
        }


        private string CreateXORChecksum(string payload)
        {
            int xorsum = 0;

            for (int i = 0; i < payload.Length; ++i)
            {
                xorsum ^= Convert.ToInt32(Convert.ToString(payload[i]), 16);
            }

            return ToHex(xorsum, 1);
        }

        private string CreateLuhnChecksum()
        {
            // Payload
            StringBuilder sb = new StringBuilder();

            //Controlbytes
            foreach (int? ii in this.startCode.controlBytes)
            {
                sb.Append(ToHex(ii.Value, 2));
            }

            // Startcode
            sb.Append(this.startCode.RenderData());

            // DE
            if (!string.ReferenceEquals(this.de1.data, null))
            {
                sb.Append(this.de1.RenderData());
            }
            if (!string.ReferenceEquals(this.de2.data, null))
            {
                sb.Append(this.de2.RenderData());
            }
            if (!string.ReferenceEquals(this.de3.data, null))
            {
                sb.Append(this.de3.RenderData());
            }

            string payload = sb.ToString();

            // Check digit
            int luhnsum = 0;
            int i = 0;

            for (i = 0; i < payload.Length; i += 2)
            {
                luhnsum += (1 * Convert.ToInt32(Convert.ToString(payload[i]), 16)) + Crosssum(2 * Convert.ToInt32(Convert.ToString(payload[i + 1]), 16));
            }

            int mod = luhnsum % 10;

            if (mod == 0)
            {
                return "0";
            }

            int rest = 10 - mod;
            int sum = luhnsum + rest;

            int luhn = sum - luhnsum;

            return ToHex(luhn, 1);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("VERSION:\n" + this.version + "\n");
            sb.Append("LC: " + this.lc + "\n");
            sb.Append("Startcode:\n" + this.startCode + "\n");
            sb.Append("DE1:\n" + this.de1 + "\n");
            sb.Append("DE2:\n" + this.de2 + "\n");
            sb.Append("DE3:\n" + this.de3 + "\n");
            sb.Append("CB : " + this.rest + "\n");

            return sb.ToString();
        }

        /// <summary>
        /// Resetting code
        /// </summary>
        private void Reset()
        {
            this.lc = 0;
            this.startCode = new Startcode(this);
            this.de1 = new DE(this);
            this.de2 = new DE(this);
            this.de3 = new DE(this);
            this.rest = null;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FlickerCode))
            {
                return false;
            }

            FlickerCode other = (FlickerCode)obj;

            if (this.lc != other.lc)
            {
                return false;
            }

            if (!this.startCode.Equals(other.startCode))
            {
                return false;
            }

            if (!this.de1.Equals(other.de1))
            {
                return false;
            }

            if (!this.de2.Equals(other.de2))
            {
                return false;
            }

            if (!this.de3.Equals(other.de3))
            {
                return false;
            }

            if (string.ReferenceEquals(this.rest, null))
            {
                return (string.ReferenceEquals(other.rest, null));
            }

            return this.rest.Equals(other.rest);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        /// <summary>
        /// Bean
        /// </summary>
        public class DE
        {
            private readonly FlickerCode outerInstance;

            public DE(FlickerCode outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public int length = 0;

            public int lde = 0;

            public Encoding encoding;

            public string data = null;

            internal virtual string Parse(string s)
            {
                if (string.ReferenceEquals(s, null) || s.Length == 0)
                {
                    return s;
                }

                // LDE
                this.lde = int.Parse(s.Substring(0, 2));
                s = s.Substring(2);

                // Cutt of control bits
                this.length = BitSum(this.lde, 5);

                // User data
                this.data = s.Substring(0, this.length);
                s = s.Substring(this.length);

                return s;
            }

            internal virtual string RenderLength()
            {
                if (string.ReferenceEquals(this.data, null))
                {
                    return "";
                }

                Encoding enc = this.Encoding;

                int len = RenderData().Length / 2;

                if (enc == Encoding.BCD)
                {
                    return ToHex(len, 2);
                }

                if (outerInstance.version == HHDVersion.HHD14)
                {
                    len = len + (1 << BIT_ENCODING);

                    return ToHex(len, 2);
                }

                // HHD 1.3
                // Return only 1 in the left semi byte
                return "1" + ToHex(len, 1);
            }

            internal virtual Encoding Encoding
            {
                get
                {
                    if (string.ReferenceEquals(this.data, null))
                    {
                        return Encoding.BCD;
                    }

                    if (this.data.Equals("[0-9]{1,}"))
                    {
                        return Encoding.BCD;
                    }

                    return Encoding.ASC;
                }
            }

            internal virtual string RenderData()
            {
                if (string.ReferenceEquals(this.data, null))
                {
                    return "";
                }

                Encoding enc = this.Encoding;

                if (enc == Encoding.ASC)
                {
                    return ToHex(this.data);
                }

                // For BCD encoding, add "F" to the byte boundary
                string s = this.data;

                if (s.Length % 2 == 1)
                {
                    s += "F";
                }

                return s;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("  Length  : " + this.length + "\n");
                sb.Append("  LDE     : " + this.lde + "\n");
                sb.Append("  Data    : " + this.data + "\n");
                sb.Append("  Encoding: " + this.encoding + "\n");

                return sb.ToString();
            }

            public override bool Equals(object obj)
            {
                if (!(obj is DE))
                {
                    return false;
                }

                return this.ToString().Equals(obj.ToString());
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        /// <summary>
        /// Bean
        /// </summary>
        public class Startcode : DE
        {
            private readonly FlickerCode outerInstance;

            public Startcode(FlickerCode outerInstance) : base(outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            /// <summary>
            /// Control bytes.
            /// </summary>
            public IList<int?> controlBytes = new List<int?>();

            internal override string Parse(string s)
            {
                // LDE (hex)
                this.lde = Convert.ToInt32(s.Substring(0, 2), 16);
                s = s.Substring(2);

                this.length = BitSum(this.lde, 5);

                outerInstance.version = HHDVersion.HHD13;

                // Control byte
                if (BitSet(this.lde, BIT_CONTROLBYTE))
                {
                    outerInstance.version = HHDVersion.HHD14;

                    // Controlbytes maximum
                    for (int i = 0; i < 10; ++i)
                    {
                        int controlByte = Convert.ToInt32(s.Substring(0, 2), 16);
                        this.controlBytes.Add(controlByte);

                        s = s.Substring(2);

                        if (!BitSet(controlByte, BIT_CONTROLBYTE))
                        {
                            break;
                        }
                    }
                }

                // Startcode
                this.data = s.Substring(0, this.length);
                s = s.Substring(this.length);

                return s;
            }

            internal override string RenderLength()
            {
                string s = base.RenderLength();

                // HHD 1.3 has no controlbytes
                if (outerInstance.version == HHDVersion.HHD13)
                {
                    return s;
                }

                // HHD 1.4 no controlbytes available
                if (this.controlBytes.Count == 0)
                {
                    return s;
                }

                int len = Convert.ToInt32(s, 16);

                if (this.controlBytes.Count > 0)
                {
                    len += (1 << BIT_CONTROLBYTE);
                }

                return ToHex(len, 2);
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder(base.ToString());

                sb.Append("  Controlbytes: " + this.controlBytes + "\n");

                return sb.ToString();
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Startcode))
                {
                    return false;
                }

                return this.ToString().Equals(obj.ToString());
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        private static string ToHex(int n, int len)
        {
            string s = Convert.ToString(n, 16).ToUpper();

            while (s.Length < len)
            {
                s = "0" + s;
            }

            return s;
        }

        private static string ToHex(string s)
        {
            StringBuilder sb = new StringBuilder();

            char[] chars = s.ToCharArray();

            foreach (char c in chars)
            {
                sb.Append(ToHex(c, 2));
            }

            return sb.ToString();
        }

        private static int Crosssum(int n)
        {
            int q = 0;

            while (n != 0)
            {
                q += n % 10;
                n = (int)Math.Floor(Convert.ToDouble(n / 10));
            }

            return q;
        }

        private static int BitSum(int num, int bits)
        {
            int sum = 0;

            for (int i = 0; i <= bits; ++i)
            {
                sum += (num & (1 << i));
            }

            return sum;
        }

        private static bool BitSet(int num, int bit)
        {
            return (num & (1 << bit)) != 0;
        }
    }
}