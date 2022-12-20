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
using System.Text;
using System.Threading.Tasks;
using libfintx.FinTS.Data;
using libfintx.FinTS.Message;
using libfintx.FinTS.Segments;
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
                    SubAccountFeature = connectionDetails.SubAccount,
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
                        StringBuilder sb = new StringBuilder();
                        sb.Append(activeAccount.AccountNumber);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.SubAccountFeature);
                        sb.Append(DEG.Separator);
                        sb.Append(SEG_COUNTRY.Germany);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.AccountBankCode);
                        sb.Append(sEG.Delimiter + DEG.DeAdd);
                        sb.Append(sEG.Terminator);
                        string rawData = sb.ToString();
                        segments = sEG.toSEG(new SEG_DATA {
                            Header = "HKKAZ", Num = client.SEGNUM, Version = client.HIKAZS, RefNum = 0, RawData = rawData}
                        );
                        //segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N'";
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(activeAccount.AccountIban);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.AccountBic);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.AccountNumber);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.SubAccountFeature);
                        sb.Append(DEG.Separator);
                        sb.Append(SEG_COUNTRY.Germany);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.AccountBankCode);
                        sb.Append(sEG.Delimiter);
                        sb.Append(DEG.DeAdd);
                        sb.Append(sEG.Terminator);
                        string rawData = sb.ToString();
                        segments = sEG.toSEG(new SEG_DATA {
                            Header = "HKKAZ", Num = client.SEGNUM, Version = client.HIKAZS, RefNum = 0, RawData = rawData });

                        //segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountIban + ":" + activeAccount.AccountBic + ":" + activeAccount.AccountNumber + "::" + SEG_COUNTRY.Germany + ":" + activeAccount.AccountBankCode + "+N'";
                    }
                }
                else
                {
                    if (Convert.ToInt16(client.HIKAZS) < 7)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(activeAccount.AccountNumber);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.SubAccountFeature);
                        sb.Append(DEG.Separator);
                        sb.Append(SEG_COUNTRY.Germany);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.AccountBankCode);
                        sb.Append(sEG.Delimiter);
                        sb.Append(DEG.DeAdd);
                        sb.Append(sEG.Delimiter);
                        sb.Append(sEG.Delimiter);
                        sb.Append(sEG.Delimiter);
                        sb.Append(sEG.Delimiter);
                        sb.Append(Startpoint);
                        sb.Append(sEG.Terminator);
                        string rawData = sb.ToString();
                        segments = sEG.toSEG( new SEG_DATA {
                            Header = "HKKAZ", Num = client.SEGNUM, Version = client.HIKAZS, RefNum = 0, RawData = rawData });
                        //segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N++++" + Startpoint + "'";
                    }  
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(activeAccount.AccountIban);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.AccountBic);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.AccountNumber);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.SubAccountFeature);
                        sb.Append(DEG.Separator);
                        sb.Append(SEG_COUNTRY.Germany);
                        sb.Append(activeAccount.AccountBankCode);
                        sb.Append(sEG.Delimiter);
                        sb.Append(DEG.DeAdd);
                        sb.Append(sEG.Delimiter);
                        sb.Append(sEG.Delimiter);
                        sb.Append(sEG.Delimiter);
                        sb.Append(sEG.Delimiter);
                        sb.Append(Startpoint);
                        sb.Append(sEG.Terminator);
                        string rawData = sb.ToString();
                        segments = sEG.toSEG(new SEG_DATA { Header = "HKKAZ", Num = client.SEGNUM, Version = client.HIKAZS,
                            RefNum = 0, RawData = rawData });
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
                        StringBuilder sb = new StringBuilder();
                        sb.Append(activeAccount.AccountIban);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.AccountBic);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.AccountNumber);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.SubAccountFeature);
                        sb.Append(DEG.Separator);
                        sb.Append(SEG_COUNTRY.Germany);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.AccountBankCode);
                        sb.Append(sEG.Delimiter);
                        sb.Append(DEG.DeAdd);
                        sb.Append(sEG.Delimiter);
                        sb.Append(FromDate);
                        sb.Append(sEG.Delimiter);
                        sb.Append(ToDate);
                        sb.Append(sEG.Terminator);
                        string rawData = sb.ToString();
                        segments = sEG.toSEG(new SEG_DATA { Header = "HKKAZ", Num = client.SEGNUM,
                            Version = client.HIKAZS, RefNum = 0, RawData = rawData });
                        //segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N+" + FromDate + "+" + ToDate + "'";
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(activeAccount.AccountNumber);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.SubAccountFeature);
                        sb.Append(DEG.Separator);
                        sb.Append(SEG_COUNTRY.Germany);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.AccountBankCode);
                        sb.Append(sEG.Delimiter);
                        sb.Append(DEG.DeAdd);
                        sb.Append(sEG.Delimiter);
                        sb.Append(FromDate);
                        sb.Append(sEG.Delimiter);
                        sb.Append(ToDate);
                        sb.Append(sEG.Terminator);
                        string rawData = sb.ToString();
                        segments = sEG.toSEG(new SEG_DATA { Header = "HKKAZ", Num = client.SEGNUM,
                            Version = client.HIKAZS, RefNum = 0, RawData = rawData });
                        //segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountIban + ":" + activeAccount.AccountBic + ":" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N+" + FromDate + "+" + ToDate + "'";
                    }
                }
                else
                {
                    if (Convert.ToInt16(client.HIKAZS) < 7)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(activeAccount.AccountNumber);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.SubAccountFeature);
                        sb.Append(DEG.Separator);
                        sb.Append(SEG_COUNTRY.Germany);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.AccountBankCode);
                        sb.Append(sEG.Delimiter);
                        sb.Append(DEG.DeAdd);
                        sb.Append(sEG.Delimiter);
                        sb.Append(FromDate);
                        sb.Append(sEG.Delimiter);
                        sb.Append(ToDate);
                        sb.Append(sEG.Delimiter);
                        sb.Append(sEG.Delimiter);
                        sb.Append(Startpoint);
                        sb.Append(sEG.Terminator);
                        string rawData = sb.ToString();
                        segments = sEG.toSEG(new SEG_DATA { Header = "HKKAZ", Num = client.SEGNUM,
                            Version = client.HIKAZS, RefNum = 0, RawData = rawData });
                        //segments = "HKKAZ:" + client.SEGNUM + ":" + client.HIKAZS + "+" + activeAccount.AccountNumber + "::280:" + activeAccount.AccountBankCode + "+N+" + FromDate + "+" + ToDate + "++" + Startpoint + "'";
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(activeAccount.AccountIban);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.AccountBic);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.AccountNumber);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.SubAccountFeature);
                        sb.Append(DEG.Separator);
                        sb.Append(SEG_COUNTRY.Germany);
                        sb.Append(DEG.Separator);
                        sb.Append(activeAccount.AccountBankCode);
                        sb.Append(sEG.Delimiter);
                        sb.Append(DEG.DeAdd);
                        sb.Append(sEG.Delimiter);
                        sb.Append(FromDate );
                        sb.Append(sEG.Delimiter);
                        sb.Append(ToDate);
                        sb.Append(sEG.Delimiter);
                        sb.Append(sEG.Delimiter);
                        sb.Append(Startpoint);
                        sb.Append(sEG.Terminator);
                        string rawData = sb.ToString();
                        segments = sEG.toSEG(new SEG_DATA { Header = "HKKAZ", Num = client.SEGNUM,
                            Version = client.HIKAZS, RefNum = 0, RawData = rawData });
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
