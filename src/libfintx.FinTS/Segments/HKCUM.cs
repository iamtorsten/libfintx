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
using System.Threading.Tasks;
using libfintx.FinTS.Message;
using libfintx.Logger.Log;
using libfintx.Sepa;

namespace libfintx.FinTS
{
    public static class HKCUM
    {
        /// <summary>
        /// Rebooking
        /// </summary>
        public static async Task<String> Init_HKCUM(FinTsClient client, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage)
        {
            Log.Write("Starting job HKCUM: Rebooking money");

            client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg3);

            var connectionDetails = client.ConnectionDetails;
            string segments = "HKCUM:" + client.SEGNUM + ":1+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.002.03+@@";

            var message = pain00100203.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, new DateTime(1999, 1, 1));

            segments = segments.Replace("@@", "@" + (message.Length - 1) + "@") + message;

            if (Helper.IsTANRequired("HKCUM"))
            {
                client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg4);
                segments = HKTAN.Init_HKTAN(client, segments, "HKCUM");
            }

            var TAN = await FinTSMessage.Send(client, FinTSMessage.Create(client, client.HNHBS, client.HNHBK, segments, client.HIRMS));

            Helper.Parse_Message(client, TAN);

            return TAN;
        }
    }
}
