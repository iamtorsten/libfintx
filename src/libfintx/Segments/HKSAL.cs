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

namespace libfintx
{
    public static class HKSAL
    {
        /// <summary>
        /// Balance
        /// </summary>
        public static string Init_HKSAL(FinTsClient client)
        {
            Log.Write("Starting job HKSAL: Request balance");

            var connectionDetails = client.ConnectionDetails;
            string segments = string.Empty;

            SEG.NUM = SEGNUM.SETInt(3);

            if (Convert.ToInt16(client.HISALS) >= 7)
                segments = "HKSAL:" + SEG.NUM + ":" + client.HISALS + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+N'";
            else
                segments = "HKSAL:" + SEG.NUM + ":" + client.HISALS + "+" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+N'";

            if (Helper.IsTANRequired("HKSAL"))
            {
                SEG.NUM = SEGNUM.SETInt(4);
                segments = HKTAN.Init_HKTAN(client, segments);
            }

            string message = FinTSMessage.Create(connectionDetails.HbciVersion, client.HNHBS, client.HNHBK, connectionDetails.BlzPrimary, connectionDetails.UserId, connectionDetails.Pin, client.SystemId, segments, client.HIRMS, SEG.NUM);
            string response = FinTSMessage.Send(connectionDetails.Url, message);

            client.HITAN = Helper.Parse_String(Helper.Parse_String(response, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
