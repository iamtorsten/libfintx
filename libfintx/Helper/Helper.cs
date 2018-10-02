/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2018 Torsten Klinger
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

//#define WINDOWS

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

#if WINDOWS
using System.Windows.Forms;
#endif

namespace libfintx
{
    public static class Helper
    {
        /// <summary>
        /// Regex pattern for HIRMG/HIRMS messages.
        /// </summary>
        private const string PatternResultMessage = @"(\d{4}):.*?:(.+)";

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
        public static List<HBCIBankMessage> Parse_Segment(string UserID, int BLZ, int HBCIVersion, string Message)
        {
            try
            {
                List<HBCIBankMessage> result = new List<HBCIBankMessage>();

                String[] values = Message.Split('\'');

                List<string> msg = new List<string>();

                foreach (var item in values)
                {
                    msg.Add(item + Environment.NewLine.Replace(",", ""));
                }

                string msg_ = string.Join("", msg.ToArray());
            
                string bpd = "HIBPA" + Parse_String(msg_, "HIBPA", "\r\n" + "HIUPA");
                string upd = "HIUPA" + Parse_String(msg_, "HIUPA", "\r\n" + "HNHBS");

                // BPD
                SaveBPD(BLZ, bpd);

                // UPD
                SaveUPD(BLZ, UserID, upd);

                foreach (var item in values)
                {
                    if (item.Contains("HIRMG"))
                    {
                        // HIRMG:2:2+9050::Die Nachricht enthÃ¤lt Fehler.+9800::Dialog abgebrochen+9010::Initialisierung fehlgeschlagen, Auftrag nicht bearbeitet.
                        // HIRMG:2:2+9800::Dialogabbruch.

                        string[] HIRMG_messages = item.Split('+');
                        foreach (var HIRMG_message in HIRMG_messages)
                        {
                            var message = Parse_BankCode_Message(HIRMG_message);
                            if (message != null)
                                result.Add(message);
                        }
                    }

                    if (item.Contains("HIRMS"))
                    {
                        // HIRMS:3:2:2+9942::PIN falsch. Zugang gesperrt.'
                        string[] HIRMS_messages = item.Split('+');
                        foreach (var HIRMS_message in HIRMS_messages)
                        {
                            var message = Parse_BankCode_Message(HIRMS_message);
                            if (message != null)
                                result.Add(message);
                        }

                        if (item.Contains("3920"))
                        {
                            string TAN = string.Empty;
                            string TANf = string.Empty;

                            string[] procedures = Regex.Split(item, @"\D+");

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
                            {
                                Segment.HIRMS = TAN;
                            }
                            else
                            {
                                if (!TANf.Contains(Segment.HIRMS))
                                    throw new Exception($"Invalid HIRMS/Tan-Mode {Segment.HIRMS} detected. Please choose one of the allowed modes: {TANf}");
                            }
                            Segment.HIRMSf = TANf;

                            // Parsing TAN processes
                            if (!String.IsNullOrEmpty(Segment.HIRMS))
                                Parse_TANProcesses(bpd);
                            
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

                        Log.Write("Customer System ID: " + ID);
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
                    }

                    if (item.Contains("HISPAS"))
                    {
                        if (item.Contains("pain.001.001.03"))
                            Segment.HISPAS = 1;
                        else if (item.Contains("pain.001.002.03"))
                            Segment.HISPAS = 2;
                        else if (item.Contains("pain.001.003.03"))
                            Segment.HISPAS = 3;

                        if (Segment.HISPAS == 0)
                            Segment.HISPAS = 3; // -> Fallback. Most banks accept the newest pain version
                    }
                }

                // Fallback if HKKAZ is not delivered by BPD (eg. Postbank)
                if (bpd.ToLower().Contains("ing-diba")) //-> ing needs segment 5
                {
                    if (String.IsNullOrEmpty(Segment.HKKAZ))
                        Segment.HKKAZ = "5";
                }
                else
                {
                    if (String.IsNullOrEmpty(Segment.HKKAZ)) // -> this should handle all other banks
                        Segment.HKKAZ = "6";
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString());

                throw new InvalidOperationException($"Software error.", ex);
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
                if (hisalParts.Length > 5 && hisalParts[5].Contains(":"))
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

                    // HIUPD:165:6:4+3300785692::280:10050000+DE22100500003300785692+5985932562+10+EUR+Behrendt+Thomas+Sparkassenbuch Gold
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
                    }
                    else // Fallback
                    {
                        Accountiban = "DE" + Parse_String(result[ctr].Value, "+DE", "+");
                        Accountowner = Parse_String(result[ctr].Value, "EUR+", "+");
                        Accounttype = Parse_String(result[ctr].Value.Replace("++EUR+", ""), "++", "++");
                    }

                    if (Accountnumber?.Length > 2 || Accountiban?.Length > 2)
                        Items.Add(new AccountInformations() { Accountnumber = Accountnumber, Accountbankcode = Accountbankcode, Accountiban = Accountiban, Accountuserid = Accountuserid, Accounttype = Accounttype, Accountcurrency = Accountcurrency, Accountowner = Accountowner});
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

        /// <summary>
        /// Parse tan processes
        /// </summary>
        /// <returns></returns>
        private static bool Parse_TANProcesses(string bpd)
        {
            try
            {
                List<TANprocess> list = new List<TANprocess>();

                string[] processes = Segment.HIRMSf.Split(';');

                // Examples from bpd
                
                // 944:2:SECUREGO:
                // 920:2:smsTAN:
                // 920:2:BestSign:

                foreach (var process in processes)
                {
                    string pattern = process + ":.*?:.*?:(?'name'.*?):.*?:(?'name2'.*?):";

                    Regex rgx = new Regex(pattern);

                    foreach (Match match in rgx.Matches(bpd))
                    {
                        int i = 0;

                        if (!process.Equals("999")) // -> PIN/TAN step 1
                        {
                            if (int.TryParse(match.Groups["name2"].Value, out i))
                                list.Add(new TANprocess { ProcessNumber = process, ProcessName = match.Groups["name"].Value });
                            else
                                list.Add(new TANprocess { ProcessNumber = process, ProcessName = match.Groups["name2"].Value });
                        }
                    }
                }

                TANProcesses.items = list;

                return true;
            }
            catch { return false; }
        }

        public static string Parse_TANMedium(string BankCode)
        {
            if (BankCode.Contains("+A:1"))
                return Parse_String(BankCode + "'", "+A:1", "'").Replace(":", "");

            // HITAB:4:4:3+0+M:1:::::::::::mT?:MFN1:********0340'
            // HITAB:5:4:3+0+M:2:::::::::::Unregistriert 1::01514/654321::::::+M:1:::::::::::Handy:*********4321:::::::
            var match = Regex.Match(BankCode, @"\+M:1:+(\w.+)?(:[\**\d]+)");
            if (match.Success) 
            {
                return match.Groups[1].Value;
            }

            return BankCode;
        }

        static FlickerRenderer flickerCodeRenderer = null;

        /// <summary>
        /// Parse bank message and handle tan process
        /// </summary>
        /// <param name="BankCode"></param>

#if WINDOWS
        public static void Parse_BankCode(string BankCode, PictureBox pictureBox)
#else
        public static void Parse_BankCode(string BankCode, object pictureBox)
#endif
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

            var processes = TANProcesses.items;

            var processname = string.Empty;

            foreach (var item in processes)
            {
                if (item.ProcessNumber.Equals(Segment.HIRMS))
                    processname = item.ProcessName;
            }

            // Smart-TAN plus optisch
            // chipTAN optisch
            if (processname.Equals("Smart-TAN plus optisch") || processname.Contains("chipTAN optisch"))
            {
                HITANFlicker = HITAN;
            }

            String[] values_ = HITAN.Split('+');

            int i = 1;

            foreach (var item in values_)
            {
                i = i + 1;

                if (i == 6)
                {
                    TransactionConsole.Output = TransactionConsole.Output + "??" + item.Replace("::", ": ").TrimStart();

                    TransactionConsole.Output = TransactionConsole.Output.Replace("??", " ")
                            .Replace("0030: ", "")
                            .Replace("1.", Environment.NewLine + "1.")
                            .Replace("2.", Environment.NewLine + "2.")
                            .Replace("3.", Environment.NewLine + "3.")
                            .Replace("4.", Environment.NewLine + "4.")
                            .Replace("5.", Environment.NewLine + "5.")
                            .Replace("6.", Environment.NewLine + "6.")
                            .Replace("7.", Environment.NewLine + "7.")
                            .Replace("8.", Environment.NewLine + "8.");
                }
            }

            // chipTAN optisch
            if (processname.Contains("chipTAN optisch"))
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

            // Smart-TAN plus optisch
            if (processname.Equals("Smart-TAN plus optisch"))
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

            // Smart-TAN photo
            if (processname.Equals("Smart-TAN photo"))
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

#if WINDOWS
        public static void Parse_BankCode(string BankCode, PictureBox pictureBox, out Image flickerImage, int flickerWidth,
            int flickerHeight, bool renderFlickerCodeAsGif)
#else
        public static List<HBCIBankMessage> Parse_BankCode(string BankCode, object pictureBox, out Image flickerImage, int flickerWidth,
            int flickerHeight, bool renderFlickerCodeAsGif)
#endif

        {
            List<HBCIBankMessage> result = new List<HBCIBankMessage>();

            flickerImage = null;

            var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

            String[] values = BankCode_.Split('+');

            string msg = string.Empty;

            foreach (var item in values)
            {
                if (!item.StartsWith("HIRMS"))
                    TransactionConsole.Output = item.Replace("::", ": ");

                var message = Parse_BankCode_Message(item);
                if (message != null)
                    result.Add(message);
            }

            var HITAN = "HITAN" + Helper.Parse_String(BankCode.Replace("?'", "").Replace("?:", ":").Replace("<br>", Environment.NewLine).Replace("?+", "??"), "'HITAN", "'");

            string HITANFlicker = string.Empty;

            var processes = TANProcesses.items;

            var processname = string.Empty;

            foreach (var item in processes)
            {
                if (item.ProcessNumber.Equals(Segment.HIRMS))
                    processname = item.ProcessName;
            }

            // Smart-TAN plus optisch
            // chipTAN optisch
            if (processname.Equals("Smart-TAN plus optisch") || processname.Contains("chipTAN optisch"))
            {
                HITANFlicker = HITAN;
            }

            String[] values_ = HITAN.Split('+');

            int i = 1;

            foreach (var item in values_)
            {
                i = i + 1;

                if (i == 6)
                {
                    TransactionConsole.Output = TransactionConsole.Output + "??" + item.Replace("::", ": ").TrimStart();

                    TransactionConsole.Output = TransactionConsole.Output.Replace("??", " ")
                            .Replace("0030: ", "")
                            .Replace("1.", Environment.NewLine + "1.")
                            .Replace("2.", Environment.NewLine + "2.")
                            .Replace("3.", Environment.NewLine + "3.")
                            .Replace("4.", Environment.NewLine + "4.")
                            .Replace("5.", Environment.NewLine + "5.")
                            .Replace("6.", Environment.NewLine + "6.")
                            .Replace("7.", Environment.NewLine + "7.")
                            .Replace("8.", Environment.NewLine + "8.");
                }
            }

            // chipTAN optisch
            if (processname.Contains("chipTAN optisch"))
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

            // Smart-TAN plus optisch
            if (processname.Equals("Smart-TAN plus optisch"))
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

            // Smart-TAN photo
            if (processname.Equals("Smart-TAN photo"))
            {
                var PhotoCode = Helper.Parse_String(BankCode, ".+@", "'HNSHA");

                var mCode = new MatrixCode(PhotoCode.Substring(5, PhotoCode.Length - 5));
            }

            return result;
        }

        /// <summary>
        /// Parse a single bank result message.
        /// </summary>
        /// <param name="BankCodeMessage"></param>
        /// <returns></returns>
        public static HBCIBankMessage Parse_BankCode_Message(string BankCodeMessage)
        {
            var match = Regex.Match(BankCodeMessage, PatternResultMessage);
            if (match.Success)
            {
                var code = match.Groups[1].Value;
                var message = match.Groups[2].Value;

                message = message.Replace("?:", ":");

                return new HBCIBankMessage(code, message);
            }
            return null;
        }

        /// <summary>
        /// Parse bank error codes
        /// </summary>
        /// <param name="BankCode"></param>
        /// <returns>Banks messages with "??" as seperator.</returns>
        public static List<HBCIBankMessage> Parse_BankCode(string BankCode)
        {
            List<HBCIBankMessage> result = new List<HBCIBankMessage>();

            string[] segments = BankCode.Split('\'');
            foreach (var segment in segments)
            {
                if (segment.Contains("HIRMG") || segment.Contains("HIRMS"))
                {
                    string[] messages = segment.Split('+');
                    foreach (var HIRMG_message in messages)
                    {
                        var message = Parse_BankCode_Message(HIRMG_message);
                        if (message != null)
                            result.Add(message);
                    }
                }
            }

            return result;
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

        /// <summary>
        /// Make filename valid
        /// </summary>
        public static string MakeFilenameValid (string value)
        {
            return value.Replace(" ", "_").Replace(":", "");
        }

        public static string GetProgramBaseDir()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (Program.Buildname == null)
            {
                throw new InvalidOperationException("Der Wert von Program.Buildname muss gesetzt sein.");
            }

            return Path.Combine(documents, Program.Buildname);
        }

        private static string GetBPDDir()
        {
            var dir = GetProgramBaseDir();
            return Path.Combine(dir, "BPD");
        }

        private static string GetBPDFile(string dir, int BLZ)
        {
            return Path.Combine(dir, "280_" + BLZ + ".bpd");
        }

        private static string GetUPDDir()
        {
            var dir = GetProgramBaseDir();
            return Path.Combine(dir, "UPD");
        }

        private static string GetUPDFile(string dir, int BLZ, string UserID)
        {
            return Path.Combine(dir, "280_" + BLZ + "_" + UserID + ".upd");
        }

        public static string GetUPD(int BLZ, string UserID)
        {
            var dir = GetUPDDir();
            var file = GetUPDFile(dir, BLZ, UserID);
            return File.Exists(file) ? File.ReadAllText(file) : string.Empty;
        }

        public static void SaveUPD(int BLZ, string UserID, string upd)
        {
            string dir = GetUPDDir();
            Directory.CreateDirectory(dir);
            var file = GetUPDFile(dir, BLZ, UserID);
            if (!File.Exists(file))
            {
                using (File.Create(file)) { };
            }
            File.WriteAllText(file, upd);
        }

        public static string GetBPD(int BLZ)
        {
            var dir = GetBPDDir();
            var file = GetBPDFile(dir, BLZ);
            return File.Exists(file) ? File.ReadAllText(file) : string.Empty;
        }

        public static void SaveBPD(int BLZ, string upd)
        {
            string dir = GetBPDDir();
            Directory.CreateDirectory(dir);
            var file = GetBPDFile(dir, BLZ);
            if (!File.Exists(file))
            {
                using (File.Create(file)) { };
            }
            File.WriteAllText(file, upd);
        }
    }
}


