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

            sigHead = "HNSHK:" + SEGNUM.SETVal(2) + ":4+" + RDH_Profile.RDHPROFILE + "+2+" + secRef + "+1+1+1::" + SystemID + "+1+1:" + date + ":" + time +
                "+1:" + Sig.HASHALG_SHA256_SHA256 + ":1+6:" + Sig.SIGALG_RSA + ":" + Sig.SIGMODE_PSS + "+" + SEG_Country.Germany + ":" + RDH_KEYSTORE.BLZ + ":" + RDH_KEYSTORE.UserID + ":" + Keytype.Sig + ":" + RDH_Profile.Version + ":1'";

            // Sig
            var sig = Sig.SignDataSHA256(Segments);
            var signature = Sig.SignMessage(sig);

            Sig.Verify(sig, signature);

            var signedsig = Encoding.GetEncoding("iso8859-1").GetString(signature);

            sigTrail = "HNSHA:" + Convert.ToString(SegmentNum + 1) + ":2+" + secRef + "+" + "@" +
               signedsig.Length + "@" + signedsig + "'";

            if (DEBUG.Enabled)
                DEBUG.Write("sigTrail: " + sigTrail);

            Segments = sigHead + Segments + sigTrail;

            byte[] encryptedSessionKey = null;
            byte[] encryptedMessage = null;

            Crypt.Encrypt(Segments, out encryptedSessionKey, out encryptedMessage);

            encHead = "HNVSK:" + Enc.SECFUNC_ENC_PLAIN + ":3+" + RDH_Profile.RDHPROFILE + "+4+1+1::" + SystemID + "+1:" + date + ":" + time + "+2:2:" + Enc.ENCALG_2K3DES + ":@" +
               encryptedSessionKey.Length + "@" + Encoding.GetEncoding("iso8859-1").GetString(encryptedSessionKey) + ":" + Enc.ENC_KEYTYPE_RSA + ":1+" + SEG_Country.Germany + ":" + RDH_KEYSTORE.BLZ + ":0:" +
               Keytype.Enc + ":" + RDH_Profile.Version + ":1+0'";

            if (DEBUG.Enabled)
                DEBUG.Write("encHead: " + encHead);

            var payload = "HNVSD:999:1+@" + encryptedMessage.Length + "@" + Encoding.GetEncoding("iso8859-1").GetString(encryptedMessage) + "'";

            var msgLen = HEAD_LEN + TRAIL_LEN + MsgNum.Length + DialogID.Length + (payload.Length + 1) + encHead.Length; // (payload.Length + 1) = Add one digit for "abschließendes Trennzeichen"

            var paddedLen = ("000000000000").Substring(0, 12 - Convert.ToString(msgLen).Length) + Convert.ToString(msgLen);

            var msgHead = "HNHBK:" + SEGNUM.SETVal(1) + ":3+" + paddedLen + "+" + ("300") + "+" + DialogID + "+" + MsgNum + "'";

            if (DEBUG.Enabled)
                DEBUG.Write("msgHead: " + msgHead);

            var msgEnd = "HNHBS:" + Convert.ToString(SegmentNum + 2) + ":1+" + MsgNum + "'";

            if (DEBUG.Enabled)
                DEBUG.Write("msgEnd: " + msgEnd);

            return msgHead + encHead + payload + msgEnd;
        }

        public static string Send(string Url, int Port, string Message)
        {
            Log.Write("Connect to HBCI Server");
            Log.Write("Url: " + Url);
            Log.Write("Port: " + Port);

            if (Trace.Enabled)
                Trace.Write(Message);

            if (DEBUG.Enabled)
                DEBUG.Write("Message send: " + Message);

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
                byte[] @byte = Encoding.GetEncoding("iso8859-1").GetBytes(Message);
                stream.Write(@byte, 0, @byte.Length);

                // Read response from server.
                // Provide enough buffer
                byte[] buffer = new byte[16384];

                System.Threading.Thread.Sleep(1000);

                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                var response = Encoding.GetEncoding("iso8859-1").GetString(buffer, 0, bytesRead);

                client.Close();

                string RDHMessage = response;

                if (Trace.Enabled)
                    Trace.Write(RDHMessage);

                if (DEBUG.Enabled)
                    DEBUG.Write("Message received: " + RDHMessage);

                return RDHMessage;
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString());

                if (DEBUG.Enabled)
                    DEBUG.Write(ex.ToString());

                return string.Empty;
            }
        }
    }
}