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

namespace libfintx
{
    public static class HKKAZ
    {
        /// <summary>
        /// Transactions
        /// </summary>
        public static async Task<String> Init_HKKAZ(FinTsClient client, string FromDate, string ToDate, string Startpoint)
        {
            Log.Write("Starting job HKKAZ: Request transactions");

            var connectionDetails = client.ConnectionDetails;
            string segments = string.Empty;
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

            client.SEGNUM = SEGNUM.SETInt(3);

            if (string.IsNullOrEmpty(FromDate))
            {
                if (string.IsNullOrEmpty(Startpoint))
                {
                    if (Convert.ToInt16(client.HIKAZS) < 7)
                        segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N'";
                    else
                        segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountIban + ":" + activeAccount.AccountBic + ":" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N'";
                }
                else
                {
                    if (Convert.ToInt16(client.HIKAZS) < 7)
                        segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N++++" + Startpoint + "'";
                    else
                        segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountIban + ":" + activeAccount.AccountBic + ":" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N++++" + Startpoint + "'";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(Startpoint))
                {
                    if (Convert.ToInt16(client.HIKAZS) < 7)
                        segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N+" + FromDate + "+" + ToDate + "'";
                    else
                        segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountIban + ":" + activeAccount.AccountBic + ":" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N+" + FromDate + "+" + ToDate + "'";
                }
                else
                {
                    if (Convert.ToInt16(client.HIKAZS) < 7)
                        segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N+" + FromDate + "+" + ToDate + "++" + Startpoint + "'";
                   else
                        segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountIban + ":" + activeAccount.AccountBic + ":" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N+" + FromDate + "+" + ToDate + "++" + Startpoint + "'";
                }
            }

            if (Helper.IsTANRequired("HKKAZ"))
            {
                client.SEGNUM = SEGNUM.SETInt(4);
                segments = HKTAN.Init_HKTAN(client, segments, "HKKAZ");
            }

            string message = FinTSMessage.Create(client, client.HNHBS, client.HNHBK, segments, client.HIRMS);
            string response = await FinTSMessage.Send(client, message);

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
