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
    public static class HKDSE
    {
        /// <summary>
        /// Collect
        /// </summary>
        public static async Task<String> Init_HKDSE(FinTsClient client, string Payer, string PayerIBAN, string PayerBIC, decimal Amount, string Usage, DateTime SettlementDate, string MandateNumber, DateTime MandateDate, string CreditorIDNumber)
        {
            Log.Write("Starting job HKDSE: Collect money");

            client.SEGNUM = SEGNUM.SETInt(4);

            var connectionDetails = client.ConnectionDetails;
            string segments = "HKDSE:" + client.SEGNUM + ":1+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.002.02+@@";

            var message = pain00800202.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, Payer, PayerIBAN, PayerBIC, Amount, Usage, SettlementDate, MandateNumber, MandateDate, CreditorIDNumber);

            segments = segments.Replace("@@", "@" + (message.Length - 1) + "@") + message;

            if (Helper.IsTANRequired("HKDSE"))
            {
                client.SEGNUM = SEGNUM.SETInt(4);
                segments = HKTAN.Init_HKTAN(client, segments, "HKDSE");
            }

            var response = await FinTSMessage.Send(client, FinTSMessage.Create(client, client.HNHBS, client.HNHBK, segments, client.HIRMS));

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
