using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace libfintx
{
    public class HIUPD
    {
        public List<AccountInformations> AccountList { get; set; }

        public HIUPD()
        {
            AccountList = new List<AccountInformations>();
        }

        public AccountInformations GetAccountInformations(string accountnumber, string bankcode)
        {
            return AccountList.FirstOrDefault(a => a.Accountnumber == accountnumber && a.Accountbankcode == bankcode);
        }

        public static HIUPD Parse_HIPUPD(string upd)
        {
            var result = new HIUPD();

            if (upd == null)
                return null;

            Parse_Accounts(upd, result.AccountList);

            return result;
        }

        /// <summary>
        /// Parse accounts and store informations
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Items"></param>
        /// <returns></returns>
        public static bool Parse_Accounts(string Message, List<AccountInformations> Items)
        {
            try
            {
                string pattern = $@"HIUPD.*?$";
                MatchCollection result = Regex.Matches(Message, pattern, RegexOptions.Multiline);

                for (int ctr = 0; ctr <= result.Count - 1; ctr++)
                {
                    string Accountnumber = null;
                    string Accountbankcode = null;
                    string Accountiban = null;
                    string Accountuserid = null;
                    string Accounttype = null;
                    string Accountcurrency = null;
                    string Accountowner = null;
                    List<AccountPermissions> Accountpermissions = new List<AccountPermissions>();

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
                                    Accountpermissions.Add(new AccountPermissions
                                    {
                                        Segment = res[c].Value.Replace("+", "").Replace(":1", ""),
                                        Description = AccountPermissions.Permission(res[c].Value.Replace("+", "").Replace(":1", ""))
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
                                    Accountpermissions.Add(new AccountPermissions
                                    {
                                        Segment = res[c].Value.Replace("+", "").Replace(":1", ""),
                                        Description = AccountPermissions.Permission(res[c].Value.Replace("+", "").Replace(":1", ""))
                                    });
                                }
                            }
                        }
                    }

                    if (Accountnumber?.Length > 2 || Accountiban?.Length > 2)
                        Items.Add(new AccountInformations()
                        {
                            Accountnumber = Accountnumber,
                            Accountbankcode = Accountbankcode,
                            Accountiban = Accountiban,
                            Accountuserid = Accountuserid,
                            Accounttype = Accounttype,
                            Accountcurrency = Accountcurrency,
                            Accountowner = Accountowner,
                            Accountpermissions = Accountpermissions
                        });
                }

                if (Items.Count > 0)
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
