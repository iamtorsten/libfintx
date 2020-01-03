using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace libfintx
{
    public class HIUPD
    {
        public List<AccountInformation> AccountList { get; set; }

        public HIUPD()
        {
            AccountList = new List<AccountInformation>();
        }

        public AccountInformation GetAccountInformations(string accountnumber, string bankcode)
        {
            return AccountList.FirstOrDefault(a => a.AccountNumber == accountnumber && a.AccountBankCode == bankcode);
        }

        public static HIUPD Parse_HIPUPD(string upd)
        {
            var result = new HIUPD();

            if (upd == null)
                return null;

            ParseAccounts(upd, result.AccountList);

            return result;
        }

        /// <summary>
        /// Parse accounts and store informations
        /// </summary>
        /// <param name="message"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool ParseAccounts(string message, List<AccountInformation> items)
        {
            try
            {
                string pattern = $@"HIUPD.*?$";
                MatchCollection result = Regex.Matches(message, pattern, RegexOptions.Multiline);

                for (int ctr = 0; ctr <= result.Count - 1; ctr++)
                {
                    string Accountnumber = null;
                    string Accountbankcode = null;
                    string Accountiban = null;
                    string Accountuserid = null;
                    string Accounttype = null;
                    string Accountcurrency = null;
                    string Accountowner = null;
                    List<AccountPermission> Accountpermissions = new List<AccountPermission>();

                    // HIUPD:165:6:4+0123456789::280:10050000+DE22100500000123456789+5985932562+10+EUR+Meier+Peter+Sparkassenbuch Gold
                    var match = Regex.Match(result[ctr].Value, @"HIUPD.*?\+(.*?)\+(.*?)\+(.*?)\+(.*?)\+(.*?)\+(.*?)\+(.*?)\+(.*?)\+");
                    if (match.Success)
                    {
                        var accountInfo = match.Groups[1].Value;
                        var matchInfo = Regex.Match(accountInfo, @"(\d+):(.*?):280:(\d+)");
                        if (matchInfo.Success)
                        {
                            Accountnumber = matchInfo.Groups[1].Value;
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
                            string pat = "\\+.*?:1";
                            MatchCollection res = Regex.Matches(result[ctr].Value, pat, RegexOptions.Singleline);

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
                        Accountiban = "DE" + Helper.Parse_String(result[ctr].Value, "+DE", "+");
                        Accountowner = Helper.Parse_String(result[ctr].Value, "EUR+", "+");
                        Accounttype = Helper.Parse_String(result[ctr].Value.Replace("++EUR+", ""), "++", "++");

                        if (Accountnumber?.Length > 2 || Accountiban?.Length > 2)
                        {
                            // Account permissions
                            string pat = "\\+.*?:1";
                            MatchCollection res = Regex.Matches(result[ctr].Value, pat, RegexOptions.Singleline);

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
                        items.Add(new AccountInformation()
                        {
                            AccountNumber = Accountnumber,
                            AccountBankCode = Accountbankcode,
                            AccountIban = Accountiban,
                            AccountUserId = Accountuserid,
                            AccountType = Accounttype,
                            AccountCurrency = Accountcurrency,
                            AccountOwner = Accountowner,
                            AccountPermissions = Accountpermissions
                        });
                }

                if (items.Count > 0)
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