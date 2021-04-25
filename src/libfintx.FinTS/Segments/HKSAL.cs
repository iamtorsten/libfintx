/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2016 - 2021 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 *  
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *  
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU Affero General Public License for more details.
 *  
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program. If not, see <http://www.gnu.org/licenses/>.
 * 	
 */

using System;
using System.Threading.Tasks;
using libfintx.FinTS.Message;
using libfintx.Logger.Log;

namespace libfintx.FinTS
{
    public static class HKSAL
    {
        /// <summary>
        /// Balance
        /// </summary>
        public static async Task<String> Init_HKSAL(FinTsClient client)
        {
            Log.Write("Starting job HKSAL: Request balance");

            var connectionDetails = client.ConnectionDetails;
            AccountInformation activeAccount;
            if (client.activeAccount != null)
                activeAccount = client.activeAccount;
            else
                activeAccount = new AccountInformation()
                {
                    AccountNumber = connectionDetails.Account,
                    AccountBankCode = connectionDetails.Blz.ToString(),
                    AccountIban = connectionDetails.Iban,
                    AccountBic = connectionDetails.Bic,
                };


            string segments = string.Empty;

            client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg3);

            if (Convert.ToInt16(client.HISALS) >= 7)
                segments = "HKSAL:" + client.SEGNUM + ":" + client.HISALS + "+" + activeAccount.AccountIban + ":" + activeAccount.AccountBic + "+N'";
            else
                segments = "HKSAL:" + client.SEGNUM + ":" + client.HISALS + "+" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N'";

            if (Helper.IsTANRequired("HKSAL"))
            {
                client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg4);
                segments = HKTAN.Init_HKTAN(client, segments, "HKSAL");
            }

            string message = FinTSMessage.Create(client, client.HNHBS, client.HNHBK, segments, client.HIRMS);
            string response = await FinTSMessage.Send(client, message);

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
