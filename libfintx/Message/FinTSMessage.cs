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
using System.Text;

namespace libfintx
{
    public static class FinTSMessage
    {
        public static string Create(int Version, string MsgNum, string DialogID, int BLZ, string UserID, string PIN, string SystemID, string Segments, string TAN)
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

            string TAN_ = string.Empty;

            if (TAN != null)
            {
                if (TAN.Length > 3)
                {
                    TAN_ = TAN.Substring(3, 7);
                    TAN = TAN.Substring(0, 3);
                }
            }

            if (Version == 220)
            {
                encHead = "HNVSK:998:2+998+1+1::" + SystemID + "+1:" + date + ":" + time + "+2:2:13:@8@00000000:5:1+280:" + BLZ + ":" + UserID + ":V:0:0+0'";

                Log.Write(encHead.Replace(UserID, "XXXXXX"));

                sigHead = string.Empty;

                if (TAN == null)
                {
                    sigHead = "HNSHK:2:3+" + "900" + "+" + secRef + "+1+1+1::" + SystemID + "+1+1:" + date + ":" + time + "+1:999:1+6:10:16+280:" + BLZ + ":" + UserID + ":S:0:0'";

                    Log.Write(sigHead.Replace(UserID, "XXXXXX"));
                }

                else
                {
                    sigHead = "HNSHK:2:3+" + TAN + "+" + secRef + "+1+1+1::" + SystemID + "+1+1:" + date + ":" + time + "+1:999:1+6:10:16+280:" + BLZ + ":" + UserID + ":S:0:0'";

                    Log.Write(sigHead.Replace(UserID, "XXXXXX").Replace(TAN, "XXXXXX"));
                }

                if (String.IsNullOrEmpty(TAN_))
                {
                    sigTrail = "HNSHA:" + (Segments.Length + 3) + ":1+" + secRef + "++" + PIN + "'";

                    Log.Write("HNSHA:" + (Segments.Length + 3) + ":1+" + secRef + "++" + "XXXXXX" + "'");
                }

                else
                {
                    sigTrail = "HNSHA:" + (Segments.Length + 3) + ":1+" + secRef + "++" + PIN + TAN_ + "'";

                    Log.Write("HNSHA:" + (Segments.Length + 3) + ":1+" + secRef + "++" + "XXXXXX" + "XXXXXX" + "'");
                }
            }
            else if (Version == 300)
            {
                encHead = "HNVSK:998:3+PIN:2+998+1+1::" + SystemID + "+1:" + date + ":" + time + "+2:2:13:@8@00000000:5:1+280:" + BLZ + ":" + UserID + ":V:0:0+0'";

                Log.Write(encHead.Replace(UserID, "XXXXXX"));

                if (TAN == null)
                {
                    sigHead = "HNSHK:2:4+PIN:1+" + "999" + "+" + secRef + "+1+1+1::" + SystemID + "+1+1:" + date + ":" + time + "+1:999:1+6:10:16+280:" + BLZ + ":" + UserID + ":S:0:0'";

                    Log.Write(sigHead.Replace(UserID, "XXXXXX").Replace(TAN, "XXXXXX"));
                }
                else
                {
                    sigHead = "HNSHK:2:4+PIN:2+" + TAN + "+" + secRef + "+1+1+1::" + SystemID + "+1+1:" + date + ":" + time + "+1:999:1+6:10:16+280:" + BLZ + ":" + UserID + ":S:0:0'";

                    Log.Write(sigHead.Replace(UserID, "XXXXXX").Replace(TAN, "XXXXXX"));
                }

                if (String.IsNullOrEmpty(TAN_))
                {
                    sigTrail = "HNSHA:" + (Segments.Length + 3) + ":2+" + secRef + "++" + PIN + "'";

                    Log.Write("HNSHA:" + (Segments.Length + 3) + ":2+" + secRef + "++" + "XXXXXX" + "'");
                }

                else
                {
                    sigTrail = "HNSHA:" + (Segments.Length + 3) + ":2+" + secRef + "++" + PIN + TAN_ + "'";

                    Log.Write("HNSHA:" + (Segments.Length + 3) + ":2+" + secRef + "++" + "XXXXXX" + "XXXXXX" + "'");
                }
            }
            else
            {
                Log.Write("HBCI version not supported");

                return string.Empty;
            }

            Segments = sigHead + Segments + sigTrail;

            var payload = Helper.Encrypt(Segments);

            Log.Write(payload.Replace(UserID, "XXXXXX").Replace(PIN, "XXXXXX"));

            var msgLen = HEAD_LEN + TRAIL_LEN + MsgNum.Length * 2 + DialogID.Length + payload.Length + encHead.Length;

            var paddedLen = ("000000000000").Substring(0, 12 - Convert.ToString(msgLen).Length) + Convert.ToString(msgLen);

            var msgHead = string.Empty;

            if (Version == 220)
            {
                msgHead = "HNHBK:1:3+" + paddedLen + "+" + ("220") + "+" + DialogID + "+" + MsgNum + "'";

                Log.Write(msgHead);
            }
            else if (Version == 300)
            {
                msgHead = "HNHBK:1:3+" + paddedLen + "+" + ("300") + "+" + DialogID + "+" + MsgNum + "'";

                Log.Write(msgHead);
            }
            else
            {
                Log.Write("HBCI version not supported");

                return string.Empty;
            }

            var msgEnd = "HNHBS:" + (Segments.Length + 2) + ":1+" + MsgNum + "'";

            Log.Write(msgEnd);

            UserID = string.Empty;
            PIN = null;

            return msgHead + encHead + payload + msgEnd;
        }

        public static string Send(string Url, string Message)
        {
            Log.Write("Connect to FinTS Server");
            Log.Write("Url: " + Url);

            try
            {
                var req = WebRequest.Create(Url) as HttpWebRequest;

                byte[] data = Encoding.ASCII.GetBytes(Helper.EncodeTo64(Message));

                req.Method = "POST";
                req.Timeout = 10000;
                req.ContentType = "application/octet-stream";
                req.ContentLength = data.Length;
                req.KeepAlive = false;

                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Flush();
                }

                string FinTSMessage = string.Empty;

                using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                {
                    using (Stream resStream = res.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(resStream, Encoding.UTF8))
                        {
                            FinTSMessage = Helper.DecodeFrom64(streamReader.ReadToEnd());
                        }
                    }
                }

                return FinTSMessage;
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString());

                return string.Empty;
            }
        }
    }
}
