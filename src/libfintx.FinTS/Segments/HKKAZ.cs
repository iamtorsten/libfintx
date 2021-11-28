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
using libfintx.FinTS.Data;
using libfintx.FinTS.Message;
using libfintx.Logger.Log;

namespace libfintx.FinTS
{
    public static class HKKAZ
    {
        /// <summary>
        /// Transactions
        /// </summary>
        public static async Task<String> Init_HKKAZ(FinTsClient client, string FromDate, string ToDate, string Startpoint)
        {
            Log.Write("Starting job HKKAZ: Request transactions");

            SEG sEG = new SEG();

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

            client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg3);

            if (string.IsNullOrEmpty(FromDate))
            {
                if (string.IsNullOrEmpty(Startpoint))
                {
                    if (Convert.ToInt16(client.HIKAZS) < 7)
                    {
                        string rawData = activeAccount.AccountNumber + DEG.Separator + DEG.Separator + SEG_Country.Germany + DEG.Separator +
                            activeAccount.AccountBankCode + sEG.Delimiter + DEG.DeAdd + sEG.Terminator;
                        segments = sEG.toSEG("HKKAZ", client.SEGNUM, client.HICAZS, 0, rawData);
                        //segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N'";
                    }
                    else
                    {
                        string rawData = activeAccount.AccountIban + DEG.Separator + activeAccount.AccountBic + DEG.Separator +
                            activeAccount.AccountNumber + DEG.Separator + DEG.Separator + SEG_Country.Germany + DEG.Separator + activeAccount.AccountBankCode +
                            sEG.Delimiter + DEG.DeAdd + sEG.Terminator;
                        segments = sEG.toSEG("HKKAZ", client.SEGNUM, client.HICAZS, 0, rawData);

                        //segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountIban + ":" + activeAccount.AccountBic + ":" + activeAccount.AccountNumber + "::" + SEG_Country.Germany + ":" + activeAccount.AccountBankCode + "+N'";
                    }
                }
                else
                {
                    if (Convert.ToInt16(client.HIKAZS) < 7)
                    {
                        string rawData = activeAccount.AccountNumber + DEG.Separator + DEG.Separator + SEG_Country.Germany + DEG.Separator +
                            activeAccount.AccountBankCode + sEG.Delimiter + DEG.DeAdd + sEG.Delimiter + sEG.Delimiter + sEG.Delimiter + sEG.Delimiter +
                            Startpoint + sEG.Terminator;
                        segments = sEG.toSEG("HKKAZ", client.SEGNUM, client.HICAZS, 0, rawData);
                        //segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N++++" + Startpoint + "'";
                    }  
                    else
                    {
                        string rawData = activeAccount.AccountIban + DEG.Separator + activeAccount.AccountBic + DEG.Separator +
                            activeAccount.AccountNumber + DEG.Separator + DEG.Separator + SEG_Country.Germany + activeAccount.AccountBankCode +
                            sEG.Delimiter + DEG.DeAdd + sEG.Delimiter + sEG.Delimiter + sEG.Delimiter + sEG.Delimiter + Startpoint + sEG.Terminator;
                        segments = sEG.toSEG("HKKAZ", client.SEGNUM, client.HICAZS, 0, rawData);
                        //segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountIban + ":" + activeAccount.AccountBic + ":" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N++++" + Startpoint + "'";
                    }   
                }
            }
            else
            {
                if (string.IsNullOrEmpty(Startpoint))
                {
                    if (Convert.ToInt16(client.HIKAZS) < 7)
                    {
                        string rawData = activeAccount.AccountIban + DEG.Separator + activeAccount.AccountBic + DEG.Separator +
                            activeAccount.AccountNumber + DEG.Separator + DEG.Separator + SEG_Country.Germany + DEG.Separator +
                            activeAccount.AccountBankCode + sEG.Delimiter + DEG.DeAdd + sEG.Delimiter +
                            FromDate + sEG.Delimiter + ToDate + sEG.Terminator;
                        segments = sEG.toSEG("HKKAZ", client.SEGNUM, client.HICAZS, 0, rawData);
                        //segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N+" + FromDate + "+" + ToDate + "'";
                    }
                    else
                    {
                        string rawData = activeAccount.AccountNumber + DEG.Separator + DEG.Separator + SEG_Country.Germany + DEG.Separator +
                            activeAccount.AccountBankCode + sEG.Delimiter + DEG.DeAdd + sEG.Delimiter +
                            FromDate + sEG.Delimiter + ToDate + sEG.Terminator;
                        segments = sEG.toSEG("HKKAZ", client.SEGNUM, client.HICAZS, 0, rawData);
                        //segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountIban + ":" + activeAccount.AccountBic + ":" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N+" + FromDate + "+" + ToDate + "'";
                    }
                }
                else
                {
                    if (Convert.ToInt16(client.HIKAZS) < 7)
                    {
                        string rawData = activeAccount.AccountNumber + DEG.Separator + DEG.Separator + SEG_Country.Germany +
                            DEG.Separator + activeAccount.AccountBankCode + sEG.Delimiter + DEG.DeAdd + sEG.Delimiter +
                            FromDate + sEG.Delimiter + ToDate + sEG.Delimiter + sEG.Delimiter + Startpoint + sEG.Terminator;
                        segments = sEG.toSEG("HKKAZ", client.SEGNUM, client.HICAZS, 0, rawData);
                        //segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N+" + FromDate + "+" + ToDate + "++" + Startpoint + "'";
                    }
                    else
                    {
                        string rawData = activeAccount.AccountIban + DEG.Separator + activeAccount.AccountBic + DEG.Separator +
                            activeAccount.AccountNumber + DEG.Separator + DEG.Separator + SEG_Country.Germany +
                            DEG.Separator + activeAccount.AccountBankCode + sEG.Delimiter + DEG.DeAdd + sEG.Delimiter +
                            FromDate + sEG.Delimiter + ToDate + sEG.Delimiter + sEG.Delimiter + Startpoint + sEG.Terminator;
                        segments = sEG.toSEG("HKKAZ", client.SEGNUM, client.HICAZS, 0, rawData);
                        //segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountIban + ":" + activeAccount.AccountBic + ":" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N+" + FromDate + "+" + ToDate + "++" + Startpoint + "'";
                    }    
                }
            }

            if (Helper.IsTANRequired("HKKAZ"))
            {
                client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg4);
                segments = HKTAN.Init_HKTAN(client, segments, "HKKAZ");
            }

            string message = FinTSMessage.Create(client, client.HNHBS, client.HNHBK, segments, client.HIRMS);
            string response = await FinTSMessage.Send(client, message);

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
