/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 * 	
 * 	libfintx is free software; you can redistribute it and/or
 *	modify it under the terms of the GNU Lesser General Public
 * 	License as published by the Free Software Foundation; either
 * 	version 2.1 of the License, or (at your option) any later version.
 *	
 * 	libfintx is distributed in the hope that it will be useful,
 * 	but WITHOUT ANY WARRANTY; without even the implied warranty of
 * 	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * 	Lesser General Public License for more details.
 *	
 * 	You should have received a copy of the GNU Lesser General Public
 * 	License along with libfintx; if not, write to the Free Software
 * 	Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 * 	
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libfintx
{
    public static class Helper
    {
        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;
        }

        static public string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            string returnValue = Encoding.ASCII.GetString(encodedDataAsBytes);

            return returnValue;
        }

        static public string Encrypt(string Segments)
        {
            return "HNVSD:999:1+@" + Segments.Length + "@" + Segments + "'";
        }

        static public string Parse_String(string StrSource, string StrStart, string StrEnd)
        {
            int Start, End;

            if (StrSource.Contains(StrStart) && StrSource.Contains(StrEnd))
            {
                Start = StrSource.IndexOf(StrStart, 0) + StrStart.Length;
                End = StrSource.IndexOf(StrEnd, Start);

                return StrSource.Substring(Start, End - Start);
            }
            else
            {
                return string.Empty;
            }
        }

        public static bool Parse_Segment(int UserID, int BLZ, int HBCIVersion, string Message)
        {
            try
            {
                String[] values = Message.Split('\'');

                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var dir = Path.Combine(documents, Program.Buildname);

				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}

                List<string> msg = new List<string>();

                foreach (var item in values)
                {
                    msg.Add(item + Environment.NewLine.Replace(",", ""));
                }

                string msg_ = string.Join("", msg.ToArray());

                string bpd = "HIBPA" + Parse_String(msg_, "HIBPA", "\r\n" + "HIUPA");
                string upd = "HIUPA" + Parse_String(msg_, "HIUPA", "\r\n" + "HNSHA");

				// BPD
				dir = Path.Combine(dir, "BPD");

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                if (!File.Exists(Path.Combine(dir, "280_" + BLZ + ".bpd")))
                {
                    using (File.Create(Path.Combine(dir, "280_" + BLZ + ".bpd")))
                    { };

                    File.WriteAllText(Path.Combine(dir, "280_" + BLZ + ".bpd"), bpd);
                }
                else
                    File.WriteAllText(Path.Combine(dir, "280_" + BLZ + ".bpd"), bpd);

				// UPD
				dir = Path.Combine(documents, Program.Buildname);
				dir = Path.Combine(dir, "UPD");

				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}

                if (!File.Exists(Path.Combine(dir, "280_" + BLZ + "_" + UserID + ".upd")))
                {
                    using (File.Create(Path.Combine(dir, "280_" + BLZ + "_" + UserID + ".upd")))
                    { };

                    File.WriteAllText(Path.Combine(dir, "280_" + BLZ + "_" + UserID + ".upd"), upd);
                }
                else
                    File.WriteAllText(Path.Combine(dir, "280_" + BLZ + "_" + UserID + ".upd"), upd);

                foreach (var item in values)
                {
                    if (item.Contains("HIRMS"))
                    {
                        var item_ = item;

                        if (item_.Contains("3920"))
                        {
                            var TAN = Parse_String(item_.ToString(), "Benutzer:", "PIN").Substring(0, 3);
                            Segment.HIRMS = TAN;

                            var TANf = Parse_String(item_.ToString(), "Benutzer:", "+");
                            Segment.HIRMSf = TANf;
                        }
                    }

                    if (item.Contains("HNHBK"))
                    {
                        var ID = Parse_String(item.ToString(), "+1+", ":1");
                        Segment.HNHBK = ID;
                    }

                    if (item.Contains("HISYN"))
                    {
                        var ID = item.Substring(13, item.Length - 13);
                        Segment.HISYN = ID;
                    }

                    if (item.Contains("HNHBS"))
                    {
                        var item_ = item + "'";

                        var MSG = Parse_String(item_.Replace("HNHBS:", ""), "+", "'");

                        if (MSG.Equals("0") || MSG == null)
                            Segment.HNHBS = "2";
                        else
                            Segment.HNHBS = Convert.ToString(Convert.ToInt16(MSG) + 1);
                    }

                    if (item.Contains("HISALS"))
                    {
                        var SEG = Parse_String(item.Replace("HISALS:", ""), ":", ":");

                        Segment.HISALS = SEG;

                        Segment.HISALSf = item;
                    }

                    if (item.Contains("HITANS"))
                    {
                        var TAN = Parse_String(item.Replace("HITANS:", ""), ":", "+").Replace(":", "+");

                        Segment.HITANS = TAN;
                    }

                    if (item.Contains("HKKAZ"))
                    {
                        if (String.IsNullOrEmpty(Segment.HKKAZ))
                        {
                            var VER = Parse_String(item, "HKKAZ;", ";");

                            Segment.HKKAZ = VER;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(Segment.HIRMS))
                {
                    UserID = 0;
                    return true;
                }
                else
                {
                    UserID = 0;

                    // Error
                    var BankCode = "HIRMG" + Helper.Parse_String(msg_, "HIRMG", "HNHBS");

                    String[] values_ = BankCode.Split('+');

                    foreach (var item in values_)
                    {
                        if (!item.StartsWith("HIRMG"))
                            Console.WriteLine(item.Replace("::", ": "));
                    }

                    return false;
                }
            }
            catch
            {
                UserID = 0;
                
                Console.WriteLine("Software error");
                return false;
            }
        }

        public static bool Parse_Message(string Message)
        {
            try
            {
                String[] values = Message.Split('\'');

                foreach (var item in values)
                {
                    if (item.Contains("HNHBS"))
                    {
                        var item_ = item + "'";

                        var MSG = Parse_String(item_.Replace("HNHBS:", ""), "+", "'");

                        if (MSG.Equals("0") || MSG == null)
                            Segment.HNHBS = "2";
                        else
                            Segment.HNHBS = Convert.ToString(Convert.ToInt16(MSG) + 1);
                    }
                }

                if (!String.IsNullOrEmpty(Segment.HNHBS))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public static string Parse_Balance(string Message)
        {
            var Balance = Parse_String(Message, "+EUR+", ":EUR:");

            if (Balance.Contains("C:"))
                Balance = Balance.Replace("C:", "");
            else if (Balance.Contains("D:"))
                Balance = "-" + Balance.Replace("D:", "");
            else
                Balance = string.Empty;

            return Balance;
        }
    }
}