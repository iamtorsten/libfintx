/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2016 - 2022 Torsten Klinger
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
using libfintx.FinTS.Data;
using libfintx.FinTS.Message;
using libfintx.Logger.Log;
using libfintx.Sepa;

namespace libfintx.FinTS
{
    public static class HKCSE
    {
        /// <summary>
        /// Transfer terminated
        /// </summary>
        public static async Task<String> Init_HKCSE(FinTsClient client, string ReceiverName, string ReceiverIBAN,
            string ReceiverBIC, decimal Amount, string Usage, DateTime ExecutionDay)
        {
            Log.Write("Starting job HKCSE: Transfer money terminated");
            var connectionDetails = client.ConnectionDetails;
            string segments = string.Empty;

            string sepaMessage = string.Empty;

            client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg3);

            SEG sEG = new SEG();

            if (client.HISPAS == 1)
            {
                segments = sEG.toSEG("HKCSE",
                    client.SEGNUM,
                    1,
                    0,
                    connectionDetails.Iban +
                    DEG.Separator +
                    connectionDetails.Bic +
                    "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.03+@@");
                //segments = "HKCSE:" + client.SEGNUM + ":1+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.03+@@";
                sepaMessage = pain00100103.Create(connectionDetails.AccountHolder, connectionDetails.Iban,
                    connectionDetails.Bic, ReceiverName, ReceiverIBAN, ReceiverBIC, Amount, Usage, ExecutionDay);
            }
            else if (client.HISPAS == 2)
            {
                segments = sEG.toSEG("HKCSE",
                    client.SEGNUM,
                    1,
                    0,
                    connectionDetails.Iban +
                    DEG.Separator +
                    connectionDetails.Bic +
                    "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.002.03+@@");
                //segments = "HKCSE:" + client.SEGNUM + ":1+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.002.03+@@";
                sepaMessage = pain00100203.Create(connectionDetails.AccountHolder, connectionDetails.Iban,
                    connectionDetails.Bic, ReceiverName, ReceiverIBAN, ReceiverBIC, Amount, Usage, ExecutionDay);
            }
            else if (client.HISPAS == 3)
            {
                segments = sEG.toSEG("HKCSE",
                    client.SEGNUM,
                    1,
                    0,
                    connectionDetails.Iban +
                    DEG.Separator +
                    connectionDetails.Bic +
                    "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.003.03+@@");
                //segments = "HKCSE:" + client.SEGNUM + ":1+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.003.03+@@";
                sepaMessage = pain00100303.Create(connectionDetails.AccountHolder, connectionDetails.Iban,
                    connectionDetails.Bic, ReceiverName, ReceiverIBAN, ReceiverBIC, Amount, Usage, ExecutionDay);
            }

            segments = segments.Replace("@@", "@" + (sepaMessage.Length - 1) + "@") + sepaMessage;

            if (Helper.IsTANRequired("HKCSE"))
            {
                client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg4);
                segments = HKTAN.Init_HKTAN(client, segments, "HKCSE");
            }

            var message = FinTSMessage.Create(client, client.HNHBS, client.HNHBK, segments, client.HIRMS);
            var response = await FinTSMessage.Send(client, message);

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
