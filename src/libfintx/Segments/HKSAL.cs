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

            client.SEGNUM = SEGNUM.SETInt(3);

            if (Convert.ToInt16(client.HISALS) >= 7)
                segments = "HKSAL:" + client.SEGNUM + ":" + client.HISALS + "+" + activeAccount.AccountIban + ":" + activeAccount.AccountBic + "+N'";
            else
                segments = "HKSAL:" + client.SEGNUM + ":" + client.HISALS + "+" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N'";

            if (Helper.IsTANRequired("HKSAL"))
            {
                client.SEGNUM = SEGNUM.SETInt(4);
                segments = HKTAN.Init_HKTAN(client, segments);
            }

            string message = FinTSMessage.Create(client, client.HNHBS, client.HNHBK, segments, client.HIRMS);
            string response = await FinTSMessage.Send(client, message);

            client.HITAN = Helper.Parse_String(Helper.Parse_String(response, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
