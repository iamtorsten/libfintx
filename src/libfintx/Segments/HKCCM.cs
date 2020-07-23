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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace libfintx
{
    public static class HKCCM
    {
        /// <summary>
        /// Collective transfer
        /// </summary>
        public static async Task<String> Init_HKCCM(FinTsClient client, List<Pain00100203CtData> PainData, string NumberofTransactions, decimal TotalAmount)
        {
            Log.Write("Starting job HKCCM: Collective transfer money");
            var connectionDetails = client.ConnectionDetails;
            client.SEGNUM = SEGNUM.SETInt(3);

            //var TotalAmount_ = TotalAmount.ToString().Replace(",", ".");

            string segments = "HKCCM:" + client.SEGNUM + ":1+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+++" + "urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.002.03+@@";

            var painMessage = pain00100203.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, PainData, NumberofTransactions, TotalAmount, new DateTime(1999, 1, 1));

            segments = segments.Replace("@@", "@" + (painMessage.Length - 1) + "@") + painMessage;

            if (Helper.IsTANRequired("HKCCM"))
            {
                client.SEGNUM = SEGNUM.SETInt(4);
                segments = HKTAN.Init_HKTAN(client, segments);
            }

            string message = FinTSMessage.Create(client, client.HNHBS, client.HNHBK, segments, client.HIRMS);
            var response = await FinTSMessage.Send(client, message);

            client.HITAN = Helper.Parse_String(Helper.Parse_String(response, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
