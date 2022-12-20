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
using System.Linq;
using System.Text.RegularExpressions;
using libfintx.Logger.Log;

namespace libfintx.FinTS
{
    public static class UPD
    {
        public static string Value { get; set; }

        public static List<AccountInformation> AccountList { get; set; }

        public static void ParseUpd(string upd)
        {
            Value = upd;
            ParseAccounts(upd);
        }

        public static AccountInformation GetAccountInformations(string accountnumber, string bankcode)
        {
            return AccountList?.FirstOrDefault(a => a.AccountNumber == accountnumber && a.AccountBankCode == bankcode);
        }

        /// <summary>
        /// Parse accounts and store informations
        /// </summary>
        /// <param name="message"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        private static bool ParseAccounts(string message)
        {
            AccountList = new List<AccountInformation>();
            try
            {
                List<string> segments = Helper.SplitSegments(message);

                foreach (string segment in segments)
                {
                    if (!segment.StartsWith("HIUPD"))
                        continue;

                    string Accountnumber = null;
                    string SubAccountFeature = null;
                    string Accountbankcode = null;
                    string Accountiban = null;
                    string Accountuserid = null;
                    string Accounttype = null;
                    string Accountcurrency = null;
                    string Accountowner = null;
                    List<AccountPermission> Accountpermissions = new List<AccountPermission>();

                    // HIUPD:165:6:4+0123456789::280:10050000+DE22100500000123456789+5985932562+10+EUR+Meier+Peter+Sparkassenbuch Gold
                    var match = Regex.Match(segment, @"HIUPD.*?\+(.*?)\+(.*?)\+(.*?)\+(.*?)\+(.*?)\+(.*?)\+(.*?)\+(.*?)\+");
                    if (match.Success)
                    {
                        var accountInfo = match.Groups[1].Value;
                        var matchInfo = Regex.Match(accountInfo, @"(\d+):(.*?):280:(\d+)");
                        if (matchInfo.Success)
                        {
                            Accountnumber = matchInfo.Groups[1].Value;
                            SubAccountFeature = matchInfo.Groups[2].Value;
                            Accountbankcode = matchInfo.Groups[3].Value;
                        }

                        Accountiban = match.Groups[2].Value;
                        Accountuserid = match.Groups[3].Value;
                        Accounttype = match.Groups[4].Value;
                        Accountcurrency = match.Groups[5].Value;
                        Accountowner = $"{match.Groups[6]} {match.Groups[7]}";
                        Accounttype = match.Groups[8].Value;

                        if (Accountnumber?.Length > 2 || Accountiban?.Length > 2)
                        {
                            // Account permissions
                            string pat = @"\+[A-Z]+:1";
                            var res = Regex.Matches(segment, pat, RegexOptions.Singleline);

                            for (int c = 0; c <= res.Count - 1; c++)
                            {
                                if (res[c].Value.Length < 10)
                                {
                                    Accountpermissions.Add(new AccountPermission
                                    {
                                        Segment = res[c].Value.Replace("+", "").Replace(":1", ""),
                                        Description = AccountPermission.Permission(res[c].Value.Replace("+", "").Replace(":1", ""))
                                    });
                                }
                            }
                        }
                    }
                    else // Fallback
                    {
                        Accountiban = "DE" + Helper.Parse_String(segment, "+DE", "+");
                        Accountowner = Helper.Parse_String(segment, "EUR+", "+");
                        Accounttype = Helper.Parse_String(segment.Replace("++EUR+", ""), "++", "++");

                        if (Accountnumber?.Length > 2 || Accountiban?.Length > 2)
                        {
                            // Account permissions
                            string pat = "\\+.*?:1";
                            MatchCollection res = Regex.Matches(segment, pat, RegexOptions.Singleline);

                            for (int c = 0; c <= res.Count - 1; c++)
                            {
                                if (res[c].Value.Length < 10)
                                {
                                    Accountpermissions.Add(new AccountPermission
                                    {
                                        Segment = res[c].Value.Replace("+", "").Replace(":1", ""),
                                        Description = AccountPermission.Permission(res[c].Value.Replace("+", "").Replace(":1", ""))
                                    });
                                }
                            }
                        }
                    }

                    if (Accountnumber?.Length > 2 || Accountiban?.Length > 2)
                        AccountList.Add(new AccountInformation()
                        {
                            AccountNumber = Accountnumber,
                            SubAccountFeature = SubAccountFeature,
                            AccountBankCode = Accountbankcode,
                            AccountIban = Accountiban,
                            AccountUserId = Accountuserid,
                            AccountType = Accounttype,
                            AccountCurrency = Accountcurrency,
                            AccountOwner = Accountowner,
                            AccountPermissions = Accountpermissions
                        }); ;
                }

                if (AccountList.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString());
                return false;
            }
        }
    }
}
