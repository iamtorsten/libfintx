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
    public static class HKEND
    {
        /// <summary>
        /// End of dialog
        /// </summary>
        /// <param name="connectionDetails"></param>
        /// <param name="dialogID"></param>
        public static async Task<String> Init_HKEND(FinTsClient client, string dialogID)
        {
            Log.Write("Starting job HKEND: End of dialog");

            var connectionDetails = client.ConnectionDetails;
            string segments = string.Empty;

            segments = "HKEND:" + SEGNUM.SETVal(3) + "1+" + dialogID + "'";

            client.SEGNUM = SEGNUM.SETInt(3);

            string message = FinTSMessage.Create(client, client.HNHBS, client.HNHBK, segments, client.HIRMS);
            return await FinTSMessage.Send(client, message);
        }
    }
}
