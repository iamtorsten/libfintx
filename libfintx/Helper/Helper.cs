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
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace libfintx
{
    public static class Helper
    {
        /// <summary>
        /// Pad zeros
        /// </summary>
        /// <returns></returns>
        private static byte[] PadZero()
        {
            var buffer = new byte[16];

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = 0;
            }

            return buffer;
        }

        /// <summary>
        /// Combine byte arrays
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static byte[] CombineByteArrays(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];

            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);

            return ret;
        }

        /// <summary>
        /// Encode to Base64
        /// </summary>
        /// <param name="toEncode"></param>
        /// <returns></returns>
        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;
        }

        /// <summary>
        /// Decode from Base64
        /// </summary>
        /// <param name="encodedData"></param>
        /// <returns></returns>
        static public string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            string returnValue = Encoding.ASCII.GetString(encodedDataAsBytes);

            return returnValue;
        }

        /// <summary>
        /// Decode from Base64 default
        /// </summary>
        /// <param name="encodedData"></param>
        /// <returns></returns>
        static public string DecodeFrom64EncodingDefault(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            string returnValue = Encoding.GetEncoding("ISO-8859-1").GetString(encodedDataAsBytes);

            return returnValue;
        }

        /// <summary>
        /// Encrypt -> HNVSD
        /// </summary>
        /// <param name="Segments"></param>
        /// <returns></returns>
        static public string Encrypt(string Segments)
        {
            return "HNVSD:999:1+@" + Segments.Length + "@" + Segments + "'";
        }

        /// <summary>
        /// Extract value from string
        /// </summary>
        /// <param name="StrSource"></param>
        /// <param name="StrStart"></param>
        /// <param name="StrEnd"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Parsing segment -> UPD, BPD
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="BLZ"></param>
        /// <param name="HBCIVersion"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static bool Parse_Segment(string UserID, int BLZ, int HBCIVersion, string Message)
        {
            try
            {
                String[] values = Message.Split('\'');

                List<string> msg = new List<string>();

                foreach (var item in values)
                {
                    msg.Add(item + Environment.NewLine.Replace(",", ""));
                }

                string msg_ = string.Join("", msg.ToArray());

                string bpd = "HIBPA" + Parse_String(msg_, "HIBPA", "\r\n" + "HIUPA");
                string upd = "HIUPA" + Parse_String(msg_, "HIUPA", "\r\n" + "HNSHA");

                if (Trace.Enabled)
                {

                    var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var dir = Path.Combine(documents, Program.Buildname);

                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

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
                }

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
                                int i;
                                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out i))
                                {
                                    if (Convert.ToString(i).StartsWith("9"))
                                    {
                                        if (String.IsNullOrEmpty(TAN))
                                            TAN = i.ToString();

                                        if (String.IsNullOrEmpty(TANf))
                                            TANf = i.ToString();
                                        else
                                            TANf += $";{i}";                                        
                                    }
                                }
                            }
                            if (string.IsNullOrEmpty(Segment.HIRMS))
                                Segment.HIRMS = TAN;
                            else
                            {
                                if (!TANf.Contains(Segment.HIRMS))
                                    throw new Exception($"Invalid HIRMS/Tan-Mode detected. Please choose one of the allowed modes: {TANf}");
                            }
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
                        var ID = item.Substring(item.IndexOf("+") + 1);
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
                        string pattern = @"HKKAZ;.*?;";
                        Regex rgx = new Regex(pattern);
                        string sentence = item;

                        foreach (Match match in rgx.Matches(sentence))
                        {
                            var VER = Parse_String(match.Value, "HKKAZ;", ";");

                            if (String.IsNullOrEmpty(Segment.HKKAZ))
                                Segment.HKKAZ = VER;
                            else
                            {
                                if (int.Parse(VER) > int.Parse(Segment.HKKAZ))
                                {
                                    Segment.HKKAZ = VER;
                                }
                            }
                        }

                        // Fallback if HKKAZ is not delivered by BPD (eg. Postbank)
                        if (String.IsNullOrEmpty(Segment.HKKAZ))
                            Segment.HKKAZ = "6";
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

                Console.WriteLine($"Software error: {ex.Message}");

                return false;
            }
        }

        /// <summary>
        /// Parsing bank message
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Parse balance
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
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
                    Accountbankcode = hisalAccountParts.Length > 3 ? hisalAccountParts[3] : null,
                    Accounttype = hisalParts[2],
                    Accountcurrency = hisalParts[3],
                    Accountbic = !string.IsNullOrEmpty(hisalAccountParts[1]) ? hisalAccountParts[1] : null
                };

                var hisalBalanceParts = hisalParts[4].Split(':');
                balance.Balance = Convert.ToDecimal($"{(hisalBalanceParts[0] == "D" ? "-" : "")}{hisalBalanceParts[1]}");


                //from here on optional fields / see page 46 in "FinTS_3.0_Messages_Geschaeftsvorfaelle_2015-08-07_final_version.pdf"
                if (hisalParts.Length > 5)
                {
                    var hisalMarkedBalanceParts = hisalParts[5].Split(':');
                    balance.MarkedTransactions = Convert.ToDecimal($"{(hisalMarkedBalanceParts[0] == "D" ? "-" : "")}{hisalMarkedBalanceParts[1]}");
                }

                if (hisalParts.Length > 6 && hisalParts[6].Contains(":"))
                {
                    balance.CreditLine = Convert.ToDecimal(hisalParts[6].Split(':')[0].TrimEnd(','));
                }

                if (hisalParts.Length > 7 && hisalParts[7].Contains(":"))
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
                string pattern = "HIUPD.*?HKSAK";
                MatchCollection result = Regex.Matches(Message, pattern, RegexOptions.Singleline);

                for (int ctr = 0; ctr <= result.Count - 1; ctr++)
                {
                    string Accountnumber = "DE" + Parse_String(result[ctr].Value, "+DE", "+");
                    string Accountowner = Parse_String(result[ctr].Value, "EUR+", "+");
                    string Accounttype = Parse_String(result[ctr].Value.Replace("++EUR+", ""), "++", "++");

                    if (Accountnumber.Length > 2)
                        Items.Add(new AccountInformations() { Accountnumber = Accountnumber, Accountowner = Accountowner, Accounttype = Accounttype });
                }

                if (Items.Count > 0)
                    return true;
                else
                    return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// Parse tan processes
        /// </summary>
        /// <returns></returns>
        public static bool Parse_TANProcesses()
        {
            // TODO: Parse instead hard coded

            try
            {
                List<TANprocess> list = new List<TANprocess>();

                switch (Segment.HIRMS)
                {
                    case "900": // iTAN
                        list.Add(new TANprocess { ProcessNumber = "900", ProcessName = "iTAN" });
                        break;
                    case "910": // chipTAN manuell
                        list.Add(new TANprocess { ProcessNumber = "910", ProcessName = "chipTAN manuell" });
                        break;
                    case "911": // chipTAN optisch
                        list.Add(new TANprocess { ProcessNumber = "911", ProcessName = "chipTAN optisch" });
                        break;
                    case "912": // chipTAN USB
                        list.Add(new TANprocess { ProcessNumber = "912", ProcessName = "chipTAN USB" });
                        break;
                    case "920": // smsTAN
                        list.Add(new TANprocess { ProcessNumber = "920", ProcessName = "smsTAN" });
                        break;
                    case "921": // pushTAN
                        list.Add(new TANprocess { ProcessNumber = "921", ProcessName = "pushTAN" });
                        break;
                    case "942": // mobile-TAN
                        list.Add(new TANprocess { ProcessNumber = "942", ProcessName = "mobile-TAN" });
                        break;
                    case "944": // SecureGo
                        list.Add(new TANprocess { ProcessNumber = "944", ProcessName = "SecureGo" });
                        break;
                    case "962": // Sm@rt-TAN plus manuell
                        list.Add(new TANprocess { ProcessNumber = "962", ProcessName = "Sm@rt-TAN plus manuell" });
                        break;
                    case "972": // Smart-TAN plus optisch
                        list.Add(new TANprocess { ProcessNumber = "972", ProcessName = "Sm@rt-TAN plus optisch" });
                        break;
                    case "982": // photo-TAN
                        list.Add(new TANprocess { ProcessNumber = "982", ProcessName = "photo-TAN" });
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
                            case "910": // chipTAN manuell
                                list.Add(new TANprocess { ProcessNumber = "910", ProcessName = "chipTAN manuell" });
                                break;
                            case "911": // chipTAN optisch
                                list.Add(new TANprocess { ProcessNumber = "911", ProcessName = "chipTAN optisch" });
                                break;
                            case "912": // chipTAN USB
                                list.Add(new TANprocess { ProcessNumber = "912", ProcessName = "chipTAN USB" });
                                break;
                            case "920": // smsTAN
                                list.Add(new TANprocess { ProcessNumber = "920", ProcessName = "smsTAN" });
                                break;
                            case "921": // pushTAN
                                list.Add(new TANprocess { ProcessNumber = "921", ProcessName = "pushTAN" });
                                break;
                            case "942": // mobile-TAN
                                list.Add(new TANprocess { ProcessNumber = "942", ProcessName = "mobile-TAN" });
                                break;
                            case "944": // SecureGo
                                list.Add(new TANprocess { ProcessNumber = "944", ProcessName = "SecureGo" });
                                break;
                            case "962": // Sm@rt-TAN plus manuell
                                list.Add(new TANprocess { ProcessNumber = "962", ProcessName = "Sm@rt-TAN plus manuell" });
                                break;
                            case "972": // Smart-TAN plus optisch
                                list.Add(new TANprocess { ProcessNumber = "972", ProcessName = "Sm@rt-TAN plus optisch" });
                                break;
                            case "982": // photo-TAN
                                list.Add(new TANprocess { ProcessNumber = "982", ProcessName = "photo-TAN" });
                                break;
                        }
                    }
                }

                TANProcesses.items = list;

                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Parse message and extract public bank keys -> Encryption, Signing
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="BLZ"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static bool Parse_Segment_RDH_Key(string Message, int BLZ, string UserID)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var dir = Path.Combine(documents, Program.Buildname);

            Message = Message.Replace("'?", "||?");

            String[] values = Message.Split('\'');

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

                if (item.Contains("HIISA"))
                {
                    if (item.Contains(":V:10"))
                    {
                        var item_ = item.ToString().Replace("||?", "'?");

                        RDH_KEYSTORE.KEY_ENCRYPTION_PUBLIC_BANK = Parse_String(item_, "@248@", ":12:");
                    }
                }

                if (item.Contains("HIISA"))
                {
                    if (item.Contains(":S:10"))
                    {
                        var item_ = item.ToString().Replace("||?", "'?");

                        RDH_KEYSTORE.KEY_SIGNING_PUBLIC_BANK = Parse_String(item_, "@248@", ":12:");
                    }
                }
            }

            if (!String.IsNullOrEmpty(RDH_KEYSTORE.KEY_ENCRYPTION_PUBLIC_BANK) &&
                !String.IsNullOrEmpty(RDH_KEYSTORE.KEY_SIGNING_PUBLIC_BANK))
            {
                // Update hbci key
                RDHKEY.Update(RDHKEY.RDHKEYFILE, RDHKEY.RDHKEYFILEPWD);

                // Release rdhkey credentials
                RDHKEY.RDHKEYFILE = string.Empty;
                RDHKEY.RDHKEYFILEPWD = string.Empty;

                return true;
            }
            else
            {
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

        static FlickerRenderer flickerCodeRenderer = null;

        /// <summary>
        /// Parse bank message and handle tan process
        /// </summary>
        /// <param name="BankCode"></param>
        public static void Parse_BankCode(string BankCode, PictureBox pictureBox)
        {
            var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

            String[] values = BankCode_.Split('+');

            string msg = string.Empty;

            foreach (var item in values)
            {
                if (!item.StartsWith("HIRMS"))
                    TransactionConsole.Output = item.Replace("::", ": ");
            }

            var HITAN = "HITAN" + Helper.Parse_String(BankCode.Replace("?'", "").Replace("?:", ":").Replace("<br>", Environment.NewLine).Replace("?+", "??"), "'HITAN", "'");

            string HITANFlicker = string.Empty;

            // chip-TAN / Sm@rt-TAN
            if (Segment.HIRMS.Equals("911") || Segment.HIRMS.Equals("972"))
            {
                HITANFlicker = HITAN;
            }

            String[] values_ = HITAN.Split('+');

            int i = 1;

            foreach (var item in values_)
            {
                i = i + 1;

                if (i == 6)
                    TransactionConsole.Output = TransactionConsole.Output + "??" + item.Replace("::", ": ").TrimStart();
            }

            // chip-TAN
            if (Segment.HIRMS.Equals("911"))
            {
                string FlickerCode = string.Empty;

                FlickerCode = "CHLGUC" + Helper.Parse_String(HITAN, "CHLGUC", "CHLGTEXT") + "CHLGTEXT";

                FlickerCode flickerCode = new FlickerCode(FlickerCode);

                flickerCodeRenderer = new FlickerRenderer(flickerCode.Render(), pictureBox);

                RUN_flickerCodeRenderer();

                Action action = STOP_flickerCodeRenderer;
                TimeSpan span = new TimeSpan(0, 0, 0, 50);

                ThreadStart start = delegate { RunAfterTimespan(action, span); };
                Thread thread = new Thread(start);
                thread.Start();
            }

            // Sm@rt-TAN
            if (Segment.HIRMS.Equals("972"))
            {
                HITANFlicker = HITAN.Replace("?@", "??");

                string FlickerCode = string.Empty;

                String[] values__ = HITANFlicker.Split('@');

                int ii = 1;

                foreach (var item in values__)
                {
                    ii = ii + 1;

                    if (ii == 4)
                        FlickerCode = item;
                }

                FlickerCode flickerCode = new FlickerCode(FlickerCode.Trim());

                flickerCodeRenderer = new FlickerRenderer(flickerCode.Render(), pictureBox);

                RUN_flickerCodeRenderer();

                Action action = STOP_flickerCodeRenderer;
                TimeSpan span = new TimeSpan(0, 0, 0, 50);

                ThreadStart start = delegate { RunAfterTimespan(action, span); };
                Thread thread = new Thread(start);
                thread.Start();
            }

            // photo-TAN
            if (Segment.HIRMS.Equals("982"))
            {
                var PhotoCode = Helper.Parse_String(BankCode, ".+@", "'HNSHA");

                var mCode = new MatrixCode(PhotoCode.Substring(5, PhotoCode.Length - 5));
            }
        }

        /// <summary>
        /// Parse bank message and handle tan process
        /// </summary>
        /// <param name="BankCode"></param>
        /// <param name="pictureBox"></param>
        /// <param name="flickerImage"></param>
        /// <param name="flickerWidth"></param>
        /// <param name="flickerHeight"></param>
        /// <param name="renderFlickerCodeAsGif"></param>
        public static void Parse_BankCode(string BankCode, PictureBox pictureBox, out Image flickerImage, int flickerWidth,
            int flickerHeight, bool renderFlickerCodeAsGif)
        {
            flickerImage = null;

            var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

            String[] values = BankCode_.Split('+');

            string msg = string.Empty;

            foreach (var item in values)
            {
                if (!item.StartsWith("HIRMS"))
                    TransactionConsole.Output = item.Replace("::", ": ");
            }

            var HITAN = "HITAN" + Helper.Parse_String(BankCode.Replace("?'", "").Replace("?:", ":").Replace("<br>", Environment.NewLine).Replace("?+", "??"), "'HITAN", "'");

            string HITANFlicker = string.Empty;

            // chip-TAN / Sm@rt-TAN
            if (Segment.HIRMS.Equals("911") || Segment.HIRMS.Equals("972"))
            {
                HITANFlicker = HITAN;
            }

            String[] values_ = HITAN.Split('+');

            int i = 1;

            foreach (var item in values_)
            {
                i = i + 1;

                if (i == 6)
                    TransactionConsole.Output = TransactionConsole.Output + "??" + item.Replace("::", ": ").TrimStart();
            }

            // chip-TAN
            if (Segment.HIRMS.Equals("911"))
            {
                string FlickerCode = string.Empty;

                FlickerCode = "CHLGUC" + Helper.Parse_String(HITAN, "CHLGUC", "CHLGTEXT") + "CHLGTEXT";

                FlickerCode flickerCode = new FlickerCode(FlickerCode);
                flickerCodeRenderer = new FlickerRenderer(flickerCode.Render(), pictureBox);
                if (!renderFlickerCodeAsGif)
                {
                    RUN_flickerCodeRenderer();

                    Action action = STOP_flickerCodeRenderer;
                    TimeSpan span = new TimeSpan(0, 0, 0, 50);

                    ThreadStart start = delegate { RunAfterTimespan(action, span); };
                    Thread thread = new Thread(start);
                    thread.Start();
                }
                else
                {
                    flickerImage = flickerCodeRenderer.RenderAsGif(flickerWidth, flickerHeight);
                }
            }

            // Sm@rt-TAN
            if (Segment.HIRMS.Equals("972"))
            {
                HITANFlicker = HITAN.Replace("?@", "??");

                string FlickerCode = string.Empty;

                String[] values__ = HITANFlicker.Split('@');

                int ii = 1;

                foreach (var item in values__)
                {
                    ii = ii + 1;

                    if (ii == 4)
                        FlickerCode = item;
                }

                FlickerCode flickerCode = new FlickerCode(FlickerCode.Trim());
                flickerCodeRenderer = new FlickerRenderer(flickerCode.Render(), pictureBox);
                if (!renderFlickerCodeAsGif)
                {
                    RUN_flickerCodeRenderer();

                    Action action = STOP_flickerCodeRenderer;
                    TimeSpan span = new TimeSpan(0, 0, 0, 50);

                    ThreadStart start = delegate { RunAfterTimespan(action, span); };
                    Thread thread = new Thread(start);
                    thread.Start();
                }
                else
                {
                    flickerImage = flickerCodeRenderer.RenderAsGif(flickerWidth, flickerHeight);
                }
            }

            // photo-TAN
            if (Segment.HIRMS.Equals("982"))
            {
                var PhotoCode = Helper.Parse_String(BankCode, ".+@", "'HNSHA");

                var mCode = new MatrixCode(PhotoCode.Substring(5, PhotoCode.Length - 5));
            }
        }

        /// <summary>
        /// Parse bank error code
        /// </summary>
        /// <param name="BankCode"></param>
        /// <returns></returns>
        public static string Parse_BankCode_Error(string BankCode)
        {
            // Error
            var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

            String[] values = BankCode_.Split('+');

            string msg = string.Empty;

            foreach (var item in values)
            {
                if (!item.StartsWith("HIRMS"))
                    msg = msg + "??" + item.Replace("::", ": ");
            }

            return msg;
        }

        /// <summary>
        /// RUN Flicker Code Rendering
        /// </summary>
        private static void RUN_flickerCodeRenderer()
        {
            flickerCodeRenderer.Start();
        }

        /// <summary>
        /// STOP Flicker Code Rendering
        /// </summary>
        public static void RunAfterTimespan(Action action, TimeSpan span)
        {
            Thread.Sleep(span);
            action();
        }

        private static void STOP_flickerCodeRenderer()
        {
            flickerCodeRenderer.Stop();
        }
    }
}


