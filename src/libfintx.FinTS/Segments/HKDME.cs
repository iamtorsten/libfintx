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
using System.Collections.Generic;
using System.Threading.Tasks;
using libfintx.FinTS.Message;
using libfintx.Logger.Log;
using libfintx.Sepa;

namespace libfintx.FinTS
{
    public static class HKDME
    {
        /// <summary>
        /// Collective collect
        /// </summary>
        public static async Task<String> Init_HKDME(FinTsClient client, DateTime SettlementDate, List<Pain00800202CcData> PainData, string NumberofTransactions, decimal TotalAmount)
        {
            Log.Write("Starting job HKDME: Collective collect money");

            client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg3);

            var TotalAmount_ = TotalAmount.ToString().Replace(",", ".");

            var connectionDetails = client.ConnectionDetails;
            // TODO: Anscheinend wird totalAmount nur in der Version 2 unterstützt, diese hat jedoch nicht jede Bank implementiert.
            //string segments = "HKDME:" + client.SEGNUM + ":2+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+" + TotalAmount_ + ":EUR++" + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.002.02+@@";
            SEG sEG = new SEG();
            string segments = sEG.toSEG("HKDME", client.SEGNUM, 1, 0, connectionDetails.Iban + sEG.Finisher + connectionDetails.Bic +
                sEG.Delimiter + sEG.Delimiter + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.002.02+@@");
            //string segments = "HKDME:" + client.SEGNUM + ":1+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "++" + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.002.02+@@";

            var message = pain00800202.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, SettlementDate, PainData, NumberofTransactions, TotalAmount);

            segments = segments.Replace("@@", "@" + (message.Length - 1) + "@") + message;

            if (Helper.IsTANRequired("HKDME"))
            {
                client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg4);
                segments = HKTAN.Init_HKTAN(client, segments, "HKDME");
            }

            var TAN = await FinTSMessage.Send(client, FinTSMessage.Create(client, client.HNHBS, client.HNHBK, segments, client.HIRMS));

            Helper.Parse_Message(client, TAN);

            return TAN;
        }
    }
}
