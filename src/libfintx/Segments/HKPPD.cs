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
    public static class HKPPD
    {
        /// <summary>
        /// Load prepaid
        /// </summary>
        public static async Task<String> Init_HKPPD(FinTsClient client, int MobileServiceProvider, string PhoneNumber, int Amount)
        {
            Log.Write("Starting job HKPPD: Load prepaid");

            client.SEGNUM = SEGNUM.SETInt(3);

            var connectionDetails = client.ConnectionDetails;
            string segments = "HKPPD:" + client.SEGNUM + ":2+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+" + MobileServiceProvider + "+" + PhoneNumber + "+" + Amount + ",:EUR'";

            if (Helper.IsTANRequired("HKPPD"))
            {
                client.SEGNUM = SEGNUM.SETInt(4);
                segments = HKTAN.Init_HKTAN(client, segments);
            }

            string message = FinTSMessage.Create(client, client.HNHBS, client.HNHBK, segments, client.HIRMS);
            var response = await FinTSMessage.Send(client, message);

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
