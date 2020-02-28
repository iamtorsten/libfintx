/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2020 Torsten Klinger
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
using System.Threading.Tasks;

namespace libfintx
{
    public static class Tan
    {
        /// <summary>
        /// TAN
        /// </summary>
        public static async Task<String> Send_TAN(FinTsClient client, string TAN)
        {
            Log.Write("Starting TAN process");
            var connectionDetails = client.ConnectionDetails;
            string segments = string.Empty;

            var HITANS = !string.IsNullOrEmpty(client.HITANS.Substring(0, 1)) ? int.Parse(client.HITANS.Substring(0, 1)) : 0;

            if (string.IsNullOrEmpty(client.HITAB)) // TAN Medium Name not set
            {
                // Version 2
                if (HITANS == 2)
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + client.HITANS.Substring(0, 1) + "+2++" + client.HITAN + "++N'";
                // Version 3
                else if (HITANS == 3)
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + client.HITANS.Substring(0, 1) + "+2++" + client.HITAN + "++N'";
                // Version 4
                else if (HITANS == 4)
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + client.HITANS.Substring(0, 1) + "+2++" + client.HITAN + "++N'";
                // Version 5
                else if (HITANS == 5)
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + client.HITANS.Substring(0, 1) + "+2++++" + client.HITAN + "++N'";
                // Version 6
                else if (HITANS == 6)
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + client.HITANS.Substring(0, 1) + "+2++++" + client.HITAN + "+N'";
                else // default
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + client.HITANS.Substring(0, 1) + "+2++++" + client.HITAN + "++N'";
            }
            else
            {
                // Version 2
                if (HITANS == 2)
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + client.HITANS.Substring(0, 1) + "+2++" + client.HITAN + "++N++++" + client.HITAB + "'";
                // Version 3
                else if (HITANS == 3)
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + client.HITANS.Substring(0, 1) + "+2++" + client.HITAN + "++N++++" + client.HITAB + "'";
                // Version 4
                else if (HITANS == 4)
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + client.HITANS.Substring(0, 1) + "+2++" + client.HITAN + "++N++++" + client.HITAB + "'";
                // Version 5
                else if (HITANS == 5)
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + client.HITANS.Substring(0, 1) + "+2++++" + client.HITAN + "++N++++" + client.HITAB + "'";
                // Version 6
                else if (HITANS == 6)
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + client.HITANS.Substring(0, 1) + "+2++++" + client.HITAN + "+N++++" + client.HITAB + "'";
                else // default
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + client.HITANS.Substring(0, 1) + "+2++" + client.HITAN + "++N++++" + client.HITAB + "'";
            }

            client.SEGNUM = SEGNUM.SETInt(3);

            string message = FinTSMessage.Create(connectionDetails.HbciVersion, client.HNHBS, client.HNHBK, connectionDetails.BlzPrimary, connectionDetails.UserId, connectionDetails.Pin, client.SystemId, segments, client.HIRMS + ":" + TAN, client.SEGNUM);
            string response = await FinTSMessage.Send(connectionDetails.Url, message);

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
