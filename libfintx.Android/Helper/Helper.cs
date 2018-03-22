/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2017 Torsten Klinger
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace libfintx
{
    public static class Helper
    {
        // Pad zeros
        private static byte[] PadZero()
        {
            var buffer = new byte[16];

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = 0;
            }

            return buffer;
        }

        // Combine byte arrays
        public static byte[] CombineByteArrays(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];

            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);

            return ret;
        }

        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;
        }

        static public string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            string returnValue = Encoding.ASCII.GetString(encodedDataAsBytes);

            return returnValue;
        }

        static public string DecodeFrom64EncodingDefault(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            string returnValue = Encoding.GetEncoding("iso8859-1").GetString(encodedDataAsBytes);

            return returnValue;
        }

        static public string Encrypt(string Segments)
        {
            return "HNVSD:999:1+@" + Segments.Length + "@" + Segments + "'";
        }

        static public string Parse_String(string StrSource, string StrStart, string StrEnd)
        {
            int Start, End;

            if (StrSource.Contains(StrStart) && StrSource.Contains(StrEnd))
            {
                Start = StrSource.IndexOf(StrStart, 0) + StrStart.Length;
                End = StrSource.IndexOf(StrEnd, Start);

                return StrSource.Substring(Start, End - Start);
            }
            else
            {
                return string.Empty;
            }
        }

        public static bool Parse_Segment(string UserID, int BLZ, int HBCIVersion, string Message)
        {
            try
            {
                String[] values = Message.Split('\'');

                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var dir = Path.Combine(documents, Program.Buildname);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                List<string> msg = new List<string>();

                foreach (var item in values)
                {
                    msg.Add(item + Environment.NewLine.Replace(",", ""));
                }

                string msg_ = string.Join("", msg.ToArray());

                string bpd = "HIBPA" + Parse_String(msg_, "HIBPA", "\r\n" + "HIUPA");
                string upd = "HIUPA" + Parse_String(msg_, "HIUPA", "\r\n" + "HNSHA");

                // BPD
                dir = Path.Combine(dir, "BPD");

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                if (!File.Exists(Path.Combine(dir, "280_" + BLZ + ".bpd")))
                {
                    using (File.Create(Path.Combine(dir, "280_" + BLZ + ".bpd")))
                    { };

                    File.WriteAllText(Path.Combine(dir, "280_" + BLZ + ".bpd"), bpd);
                }
                else
                    File.WriteAllText(Path.Combine(dir, "280_" + BLZ + ".bpd"), bpd);

                // UPD
                dir = Path.Combine(documents, Program.Buildname);
                dir = Path.Combine(dir, "UPD");

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                if (!File.Exists(Path.Combine(dir, "280_" + BLZ + "_" + UserID + ".upd")))
                {
                    using (File.Create(Path.Combine(dir, "280_" + BLZ + "_" + UserID + ".upd")))
                    { };

                    File.WriteAllText(Path.Combine(dir, "280_" + BLZ + "_" + UserID + ".upd"), upd);
                }
                else
                    File.WriteAllText(Path.Combine(dir, "280_" + BLZ + "_" + UserID + ".upd"), upd);

                foreach (var item in values)
                {
                    if (item.Contains("HIRMS"))
                    {
                        var item_ = item;

                        if (item_.Contains("3920"))
                        {
                            string TAN = string.Empty;
                            string TANf = string.Empty;

                            string[] procedures = Regex.Split(item_, @"\D+");

                            foreach (string value in procedures)
                            {
                                if (!string.IsNullOrEmpty(value))
                                {
                                    int i = int.Parse(value);

                                    if (Convert.ToString(i).StartsWith("9"))
                                    {
                                        if (String.IsNullOrEmpty(TAN))
                                            TAN = Convert.ToString(i);
                                        else
                                        {
                                            if (String.IsNullOrEmpty(TANf))
                                                TANf = Convert.ToString(i);
                                            else
                                                TANf = TANf + ";" + Convert.ToString(i);
                                        }
                                    }
                                }
                            }

                            Segment.HIRMS = TAN;
                            Segment.HIRMSf = TANf;
                        }
                    }

                    if (item.Contains("HNHBK"))
                    {
                        var ID = Parse_String(item.ToString(), "+1+", ":1");
                        Segment.HNHBK = ID;
                    }

                    if (item.Contains("HISYN"))
                    {
                        var ID = item.Substring(13, item.Length - 13);
                        Segment.HISYN = ID;
                    }

                    if (item.Contains("HNHBS"))
                    {
                        var item_ = item + "'";

                        var MSG = Parse_String(item_.Replace("HNHBS:", ""), "+", "'");

                        if (MSG.Equals("0") || MSG == null)
                            Segment.HNHBS = "2";
                        else
                            Segment.HNHBS = Convert.ToString(Convert.ToInt16(MSG) + 1);
                    }

                    if (item.Contains("HISALS"))
                    {
                        var SEG = Parse_String(item.Replace("HISALS:", ""), ":", ":");

                        Segment.HISALS = SEG;

                        Segment.HISALSf = item;
                    }

                    if (item.Contains("HITANS"))
                    {
                        var TAN = Parse_String(item.Replace("HITANS:", ""), ":", "+").Replace(":", "+");

                        Segment.HITANS = TAN;
                    }

                    if (item.Contains("HKKAZ"))
                    {
                        if (String.IsNullOrEmpty(Segment.HKKAZ))
                        {
                            var VER = Parse_String(item, "HKKAZ;", ";");

                            Segment.HKKAZ = VER;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(Segment.HIRMS))
                {
                    UserID = string.Empty;
                    return true;
                }
                else
                {
                    UserID = string.Empty;

                    // Error
                    var BankCode = "HIRMG" + Helper.Parse_String(msg_, "HIRMG", "HNHBS");

                    String[] values_ = BankCode.Split('+');

                    foreach (var item in values_)
                    {
                        if (!item.StartsWith("HIRMG"))
                        {
                            Console.WriteLine(item.Replace("::", ": "));

                            Log.Write(item.Replace("::", ": "));
                        }
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                UserID = string.Empty;

                Log.Write(ex.ToString());

                Console.WriteLine("Software error");

                return false;
            }
        }

        public static bool Parse_Message(string Message)
        {
            try
            {
                String[] values = Message.Split('\'');

                foreach (var item in values)
                {
                    if (item.Contains("HNHBS"))
                    {
                        var item_ = item + "'";

                        var MSG = Parse_String(item_.Replace("HNHBS:", ""), "+", "'");

                        if (MSG.Equals("0") || MSG == null)
                            Segment.HNHBS = "2";
                        else
                            Segment.HNHBS = Convert.ToString(Convert.ToInt16(MSG) + 1);
                    }
                }

                if (!String.IsNullOrEmpty(Segment.HNHBS))
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

        public static AccountBalance Parse_Balance(string Message)
        {
            var hirms = Message.Substring(Message.IndexOf("HIRMS") + 5);
            hirms = hirms.Substring(0, (hirms.Contains("'") ? hirms.IndexOf('\'') : hirms.Length));
            var hirmsParts = hirms.Split(':');

            AccountBalance balance = new AccountBalance();
            balance.Message = hirmsParts[hirmsParts.Length - 1];

            if (Message.Contains("+0020::"))
            {
                var hisal = Message.Substring(Message.IndexOf("HISAL") + 5);
                hisal = hisal.Substring(0, (hisal.Contains("'") ? hisal.IndexOf('\'') : hisal.Length));
                var hisalParts = hisal.Split('+');

                balance.Successful = true;

                var hisalAccountParts = hisalParts[1].Split(':');
                balance.AccountType = new AccountInformations()
                {
                    Accountnumber = hisalAccountParts[0],
                    Accountbankcode = hisalAccountParts[3],
                    Accounttype = hisalParts[2],
                    Accountcurrency = hisalParts[3]
                };

                var hisalBalanceParts = hisalParts[4].Split(':');
                balance.Balance = Convert.ToDecimal($"{(hisalBalanceParts[0] == "D" ? "-" : "")}{hisalBalanceParts[1]}");


                //from here on optional fields / see page 46 in "FinTS_3.0_Messages_Geschaeftsvorfaelle_2015-08-07_final_version.pdf"
                if (hisalParts.Length > 5)
                {
                    var hisalMarkedBalanceParts = hisalParts[5].Split(':');
                    balance.MarkedTransactions = Convert.ToDecimal($"{(hisalMarkedBalanceParts[0] == "D" ? "-" : "")}{hisalMarkedBalanceParts[1]}");
                }

                if (hisalParts.Length > 6)
                {
                    balance.CreditLine = Convert.ToDecimal(hisalParts[6].Split(':')[0].TrimEnd(','));
                }

                if (hisalParts.Length > 7)
                {
                    balance.AvailableBalance = Convert.ToDecimal(hisalParts[7].Split(':')[0].TrimEnd(','));
                }

                /* ---------------------------------------------------------------------------------------------------------
                 * In addition to the above fields, the following fields from HISAL could also be implemented:
                 * 
                 * - 9/Bereits verfügter Betrag
                 * - 10/Überziehung
                 * - 11/Buchungszeitpunkt
                 * - 12/Fälligkeit 
                 * 
                 * Unfortunately I'm missing test samples. So I drop support unless we get test messages for this fields.
                 ------------------------------------------------------------------------------------------------------------ */
            }
            else
            {
                balance.Successful = false;

                string msg = string.Empty;
                for (int i = 1; i < hirmsParts.Length; i++)
                {
                    msg = msg + "??" + hirmsParts[i].Replace("::", ": ");
                }
                Log.Write(msg);
            }

            return balance;
        }

        public static bool Parse_Accounts(string Message, List<AccountInformations> Items)
        {
            try
            {
                string pattern = "HIUPD.*?HKSAK";
                MatchCollection result = Regex.Matches(Message, pattern, RegexOptions.Singleline);

                for (int ctr = 0; ctr <= result.Count - 1; ctr++)
                {
                    string Accountnumber = "DE" + Parse_String(result[ctr].Value, "+DE", "+");
                    string Accountowner = Parse_String(result[ctr].Value, "EUR+", "+");
                    string Accounttype = Parse_String(result[ctr].Value.Replace("++EUR+", ""), "++", "++");

                    if (Accountnumber.Length > 2 && Accounttype.ToUpper().Contains("KONTO"))
                        Items.Add(new AccountInformations() { Accountnumber = Accountnumber, Accountowner = Accountowner, Accounttype = Accounttype });
                }

                return true;
            }
            catch { return false; }
        }

        public static bool Parse_TANProcesses()
        {
            try
            {
                List<TANprocess> list = new List<TANprocess>();

                switch (Segment.HIRMS)
                {
                    case "900": // iTAN
                        list.Add(new TANprocess { ProcessNumber = "900", ProcessName = "iTAN" });
                        break;
                    case "942": // mobile-TAN
                        list.Add(new TANprocess { ProcessNumber = "942", ProcessName = "mobile-TAN" });
                        break;
                    case "962": // Sm@rt-TAN plus manuell
                        list.Add(new TANprocess { ProcessNumber = "962", ProcessName = "Sm@rt-TAN plus manuell" });
                        break;
                    case "972": // Smart-TAN plus optisch
                        list.Add(new TANprocess { ProcessNumber = "972", ProcessName = "Sm@rt-TAN plus optisch" });
                        break;
                }

                var processes = Segment.HIRMSf;

                if (!String.IsNullOrEmpty(processes))
                {
                    var process = processes.Split(';');

                    foreach (var item in process)
                    {
                        switch (item)
                        {
                            case "900": // iTAN
                                list.Add(new TANprocess { ProcessNumber = "900", ProcessName = "iTAN" });
                                break;
                            case "942": // mobile-TAN
                                list.Add(new TANprocess { ProcessNumber = "942", ProcessName = "mobile-TAN" });
                                break;
                            case "962": // Sm@rt-TAN plus manuell
                                list.Add(new TANprocess { ProcessNumber = "962", ProcessName = "Sm@rt-TAN plus manuell" });
                                break;
                            case "972": // Smart-TAN plus optisch
                                list.Add(new TANprocess { ProcessNumber = "972", ProcessName = "Sm@rt-TAN plus optisch" });
                                break;
                        }
                    }
                }

                TANProcesses.items = list;

                return true;
            }
            catch { return false; }
        } 
    }
}