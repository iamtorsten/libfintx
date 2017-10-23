/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2017 Torsten Klinger
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

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace libfintx
{
    public static class RDHMessage
    {
        public static string Create(int Version, string MsgNum, string DialogID, int BLZ, string UserID,
            string SystemID, string Segments, int SegmentNum)
        {
            if (String.IsNullOrEmpty(MsgNum))
                MsgNum = "1";

            MsgNum += "";
            DialogID += "";

            var HEAD_LEN = 29;
            var TRAIL_LEN = 11;

            Random Rnd = new Random();
            int RndNr = Rnd.Next();

            var encHead = string.Empty;
            var sigHead = string.Empty;
            var sigTrail = string.Empty;

            var secRef = Math.Round(Convert.ToDecimal(RndNr.ToString().Replace("-", "")) * 999999 + 1000000);

            string date = Convert.ToString(DateTime.Now.Year) + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd");
            string time = Convert.ToString(DateTime.Now.TimeOfDay).Substring(0, 8).Replace(":", "");

            if (Version != 300)
            {
                Log.Write("HBCI version not supported");

                throw new Exception("HBCI Version not supported");
            }

            byte[] encryptedSessionKey = null;

            sigHead = "HNSHK:" + SEGNUM.SETVal(2) + ":4+" + RDH_Profile.RDHPROFILE + "+2+" + secRef + "+1+1+1::" + SystemID + "+1+1:" + date + ":" + time +
                "+1:6:1+6:10:19+" + SEG_Country.Germany + ":" + BLZ + ":" + UserID + ":" + Keytype.Sig + ":" + RDH_Profile.Version + ":1'";

            // Private key
            String SIGDATA = String.Empty;

            using (RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider())
            {
                rsaKey.ImportCspBlob(KeyManager.Import_Private_SIG_Key());

                SIGDATA = Helper.DecodeFrom64(Sig.SignData(Segments, rsaKey));
            }

            sigTrail = "HNSHA:" + Convert.ToString(SegmentNum + 1) + ":2+" + secRef + "+" + "@" +
               SIGDATA.Length + "@" + SIGDATA + "'";

            Segments = sigHead + Segments + sigTrail;

            byte[] encryptedMessage = null;

            using (RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider())
            {
                rsaKey.ImportCspBlob(KeyManager.Import_Public_Bank_Key());

                byte[] iv = null;

                Crypt.Encrypt(rsaKey, Segments, out iv, out encryptedSessionKey, out encryptedMessage);
            }

            encHead = "HNVSK:" + Enc.SECFUNC_ENC_PLAIN + ":3+" + RDH_Profile.RDHPROFILE + "+4+1+1::" + SystemID + "+1:" + date + ":" + time + "+2:2:13:@" +
               encryptedSessionKey.Length + "@" + Converter.FromHexString(Converter.ByteArrayToString(encryptedSessionKey)) + ":" + Enc.ENC_KEYTYPE_RSA + ":1+" + SEG_Country.Germany + ":" + BLZ + ":0:" +
               Keytype.Enc + ":" + RDH_Profile.Version + ":1+0'";

            var payload = "HNVSD:999:1+@" + encryptedMessage.Length + "@" + Converter.FromHexString(Converter.ByteArrayToString(encryptedMessage)) + "'";

            var msgLen = HEAD_LEN + TRAIL_LEN + MsgNum.Length + DialogID.Length + payload.Length + encHead.Length;

            var paddedLen = ("000000000000").Substring(0, 12 - Convert.ToString(msgLen).Length) + Convert.ToString(msgLen);

            var msgHead = "HNHBK:" + SEGNUM.SETVal(1) + ":3+" + paddedLen + "+" + ("300") + "+" + DialogID + "+" + MsgNum + "'";

            var msgEnd = "HNHBS:" + Convert.ToString(SegmentNum + 2) + ":1+" + MsgNum + "'";

            return msgHead + encHead + payload + msgEnd;
        }

        public static string Send(string Url, int Port, string Message)
        {
            Log.Write("Connect to HBCI Server");
            Log.Write("Url: " + Url);
            Log.Write("Port: " + Port);

            if (Trace.Enabled)
                Trace.Write(Message);

            try
            {
                IPAddress[] ipaddress = Dns.GetHostAddresses(Url);

                string ip3 = string.Empty;

                foreach (IPAddress ip2 in ipaddress)
                {
                    ip3 = ip2.ToString();
                }

                IPEndPoint ip = new IPEndPoint(IPAddress.Parse(ip3), Port);

                // Init and connect to client
                TcpClient client = new TcpClient();
                client.Connect(Url, Port);

                // Stream string to server
                // input += "\n";
                Stream stream = client.GetStream();
                byte[] @byte = Encoding.Default.GetBytes(Message);
                stream.Write(@byte, 0, @byte.Length);

                // Read response from server.
                // Provide enough buffer
                byte[] buffer = new byte[16384];

                System.Threading.Thread.Sleep(1000);

                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                var response = Encoding.Default.GetString(buffer, 0, bytesRead);

                client.Close();

                string RDHMessage = response;

                if (Trace.Enabled)
                    Trace.Write(RDHMessage);

                return RDHMessage;
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString());

                return string.Empty;
            }
        }
    }
}
