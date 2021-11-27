/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2016 - 2021 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 *  
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 3 of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software Foundation,
 *  Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 * 	
 */

using System;
using System.Text;
using System.Threading.Tasks;
using libfintx.FinTS.Message;
using libfintx.Logger.Log;

namespace libfintx.FinTS
{
    // HKTAN#7 decoupled S implementieren
    public static class Tan
    {
        /// <summary>
        /// TAN
        /// </summary>
        public static async Task<String> Send_TAN(FinTsClient client, string TAN)
        {
            Log.Write("Starting TAN process");
            string segments = string.Empty;

            StringBuilder sb = new StringBuilder();
            SEG sEG = new SEG();

            if (string.IsNullOrEmpty(client.HITAB)) // TAN Medium Name not set
            {
                // Version 2
                if (client.HITANS == 2)
                {
                    sb.Append("HKTAN");
                    sb.Append(sEG.Finisher);
                    sb.Append(SEG_NUM.Seg3);
                    sb.Append(sEG.Finisher);
                    sb.Append(client.HITANS);
                    sb.Append(sEG.Delimiter);
                    sb.Append("2");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAN);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append("N");
                    sb.Append(sEG.Terminator);
                    segments = sb.ToString();
                    // segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HITANS + "+2++" + client.HITAN + "++N'";
                }
                    
                // Version 3
                else if (client.HITANS == 3)
                {
                    sb.Append("HKTAN");
                    sb.Append(sEG.Finisher);
                    sb.Append(SEG_NUM.Seg3);
                    sb.Append(sEG.Finisher);
                    sb.Append(client.HITANS);
                    sb.Append(sEG.Delimiter);
                    sb.Append("2");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAN);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append("N");
                    sb.Append(sEG.Terminator);
                    segments = sb.ToString();
                    // segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HITANS + "+2++" + client.HITAN + "++N'";
                }

                // Version 4
                else if (client.HITANS == 4)
                {
                    sb.Append("HKTAN");
                    sb.Append(sEG.Finisher);
                    sb.Append(SEG_NUM.Seg3);
                    sb.Append(sEG.Finisher);
                    sb.Append(client.HITANS);
                    sb.Append(sEG.Delimiter);
                    sb.Append("2");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAN);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append("N");
                    sb.Append(sEG.Terminator);
                    segments = sb.ToString();
                    // segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HITANS + "+2++" + client.HITAN + "++N'";
                }

                // Version 5
                else if (client.HITANS == 5)
                {
                    sb.Append("HKTAN");
                    sb.Append(sEG.Finisher);
                    sb.Append(SEG_NUM.Seg3);
                    sb.Append(sEG.Finisher);
                    sb.Append(client.HITANS);
                    sb.Append(sEG.Delimiter);
                    sb.Append("2");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAN);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append("N");
                    sb.Append(sEG.Terminator);
                    segments = sb.ToString();
                    // segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HITANS + "+2++++" + client.HITAN + "++N'";
                }

                // Version 6
                else if (client.HITANS == 6)
                {
                    sb.Append("HKTAN");
                    sb.Append(sEG.Finisher);
                    sb.Append(SEG_NUM.Seg3);
                    sb.Append(sEG.Finisher);
                    sb.Append(client.HITANS);
                    sb.Append(sEG.Delimiter);
                    sb.Append("2");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAN);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append("N");
                    sb.Append(sEG.Terminator);
                    segments = sb.ToString();
                    // segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HITANS + "+2++++" + client.HITAN + "+N'";
                }

                // Version 7 -> decoupled
                // FinTS_3.0_Security_Sicherheitsverfahren_PINTAN_2020-07-10_final_version.pdf Seite 64 - 65
                else if (client.HITANS == 7)
                {
                    sb.Append("HKTAN");
                    sb.Append(sEG.Finisher);
                    sb.Append(SEG_NUM.Seg3);
                    sb.Append(sEG.Finisher);
                    sb.Append(client.HITANS);
                    sb.Append(sEG.Delimiter);
                    sb.Append("S");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAN);
                    sb.Append(sEG.Delimiter);
                    sb.Append("N");
                    sb.Append(sEG.Terminator);
                    segments = sb.ToString();
                    // segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HITANS + "+S++++" + client.HITAN + "+N'";
                }

                else // default
                {
                    sb.Append("HKTAN");
                    sb.Append(sEG.Finisher);
                    sb.Append(SEG_NUM.Seg3);
                    sb.Append(sEG.Finisher);
                    sb.Append(client.HITANS);
                    sb.Append(sEG.Delimiter);
                    sb.Append("S");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAN);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append("N");
                    sb.Append(sEG.Terminator);
                    segments = sb.ToString();
                    // segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HITANS + "+2++++" + client.HITAN + "++N'";
                }

            }
            else
            {
                // Version 2
                if (client.HITANS == 2)
                {
                    sb.Append("HKTAN");
                    sb.Append(sEG.Finisher);
                    sb.Append(SEG_NUM.Seg3);
                    sb.Append(sEG.Finisher);
                    sb.Append(client.HITANS);
                    sb.Append(sEG.Delimiter);
                    sb.Append("2");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAN);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append("N");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAB);
                    sb.Append(sEG.Terminator);
                    segments = sb.ToString();
                    // segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HITANS + "+2++" + client.HITAN + "++N++++" + client.HITAB + "'";

                }
                // Version 3
                else if (client.HITANS == 3)
                {
                    sb.Append("HKTAN");
                    sb.Append(sEG.Finisher);
                    sb.Append(SEG_NUM.Seg3);
                    sb.Append(sEG.Finisher);
                    sb.Append(client.HITANS);
                    sb.Append(sEG.Delimiter);
                    sb.Append("2");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAN);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append("N");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAB);
                    sb.Append(sEG.Terminator);
                    segments = sb.ToString();
                    // segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HITANS + "+2++" + client.HITAN + "++N++++" + client.HITAB + "'";
                }

                // Version 4
                else if (client.HITANS == 4)
                {
                    sb.Append("HKTAN");
                    sb.Append(sEG.Finisher);
                    sb.Append(SEG_NUM.Seg3);
                    sb.Append(sEG.Finisher);
                    sb.Append(client.HITANS);
                    sb.Append(sEG.Delimiter);
                    sb.Append("2");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAN);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append("N");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAB);
                    sb.Append(sEG.Terminator);
                    segments = sb.ToString();
                    // segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HITANS + "+2++" + client.HITAN + "++N++++" + client.HITAB + "'";
                }

                // Version 5
                else if (client.HITANS == 5)
                {
                    sb.Append("HKTAN");
                    sb.Append(sEG.Finisher);
                    sb.Append(SEG_NUM.Seg3);
                    sb.Append(sEG.Finisher);
                    sb.Append(client.HITANS);
                    sb.Append(sEG.Delimiter);
                    sb.Append("2");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAN);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append("N");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAB);
                    sb.Append(sEG.Terminator);
                    segments = sb.ToString();
                    // segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HITANS + "+2++++" + client.HITAN + "++N++++" + client.HITAB + "'";
                }

                // Version 6
                else if (client.HITANS == 6)
                {
                    sb.Append("HKTAN");
                    sb.Append(sEG.Finisher);
                    sb.Append(SEG_NUM.Seg3);
                    sb.Append(sEG.Finisher);
                    sb.Append(client.HITANS);
                    sb.Append(sEG.Delimiter);
                    sb.Append("2");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAN);
                    sb.Append(sEG.Delimiter);
                    sb.Append("N");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAB);
                    sb.Append(sEG.Terminator);
                    segments = sb.ToString();
                    // segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HITANS + "+2++++" + client.HITAN + "+N++++" + client.HITAB + "'";
                }

                // Version 7 -> decoupled
                // FinTS_3.0_Security_Sicherheitsverfahren_PINTAN_2020-07-10_final_version.pdf Seite 64 - 65
                else if (client.HITANS == 7)
                {
                    sb.Append("HKTAN");
                    sb.Append(sEG.Finisher);
                    sb.Append(SEG_NUM.Seg3);
                    sb.Append(sEG.Finisher);
                    sb.Append(client.HITANS);
                    sb.Append(sEG.Delimiter);
                    sb.Append("S");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAN);
                    sb.Append(sEG.Delimiter);
                    sb.Append("N");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAB);
                    sb.Append(sEG.Terminator);
                    segments = sb.ToString();
                    // segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HITANS + "+S++++" + client.HITAN + "+N++++" + client.HITAB + "'";
                }

                else // default
                {
                    sb.Append("HKTAN");
                    sb.Append(sEG.Finisher);
                    sb.Append(SEG_NUM.Seg3);
                    sb.Append(sEG.Finisher);
                    sb.Append(client.HITANS);
                    sb.Append(sEG.Delimiter);
                    sb.Append("2");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAN);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append("N");
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAB);
                    sb.Append(sEG.Terminator);
                    segments = sb.ToString();
                    // segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HITANS + "+2++" + client.HITAN + "++N++++" + client.HITAB + "'";
                }

            }

            client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg3);

            string message = FinTSMessage.Create(client, client.HNHBS, client.HNHBK, segments, client.HIRMS + (TAN != null ? ":" + TAN : null));
            string response = await FinTSMessage.Send(client, message);

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
