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

/*
 *
 *	Based on Timotheus Pokorra's C# implementation of OpenPetraPlugin_BankimportMT940,
 *	available at https://github.com/SolidCharity/OpenPetraPlugin_BankimportMT940/blob/master/Client/ParseMT940.cs
 *
 */

using libfintx.FinTS.Swift;
using libfintx.Globals;
using libfintx.Logger.Log;
using libfintx.Logger.Trace;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace libfintx.FinTS.Statement
{
    /// <summary>
    /// MT940 account statement
    /// </summary>
    public static class MT940
    {
        public static List<SwiftStatement> SWIFTStatements;
        private static SwiftStatement SWIFTStatement = null;

        private static string LTrim(string Code)
        {
            // Cut off leading zeros
            try
            {
                return Convert.ToInt64(Code).ToString();
            }
            catch (Exception)
            {
                // IBAN or BIC
                return Code;
            }
        }

        private static void Data(string swiftTag, string swiftData)
        {
            if (SWIFTStatement != null)
            {
                SWIFTStatement.Lines.Add(new SwiftLine(swiftTag, swiftData));
            }

            if (swiftTag == "OS")
            {
                // Ignore
            }
            else if (swiftTag == "20")
            {
                // 20 is used for each "page" of the SWIFTStatement; but we want to put all SWIFTTransactions together
                // the whole SWIFTStatement closes with 62F
                if (SWIFTStatement == null)
                {
                    SWIFTStatement = new SwiftStatement() { Type = swiftData };
                    SWIFTStatement.Lines.Add(new SwiftLine(swiftTag, swiftData));
                }
            }
            else if (swiftTag == "25")
            {
                int posSlash = swiftData.IndexOf("/");
                if (posSlash >= 0)
                {
                    SWIFTStatement.BankCode = swiftData.Substring(0, posSlash);
                    if (posSlash < swiftData.Length)
                        SWIFTStatement.AccountCode = LTrim(swiftData.Substring(posSlash + 1));
                }
            }
            else if (swiftTag.StartsWith("60")) // Anfangssaldo
            {
                // 60M is the start balance on each page of the SWIFTStatement.
                // 60F is the start balance of the whole SWIFTStatement.

                // First character is D or C
                int DebitCreditIndicator = (swiftData[0] == 'D' ? -1 : +1);

                // Next 6 characters: YYMMDD
                swiftData = swiftData.Substring(1);

                // Start date YYMMDD
                DateTime postingDate = new DateTime(2000 + Convert.ToInt32(swiftData.Substring(0, 2)),
                    Convert.ToInt32(swiftData.Substring(2, 2)),
                    Convert.ToInt32(swiftData.Substring(4, 2)));

                // Next 3 characters: Currency
                // Last characters: Balance with comma for decimal point
                SWIFTStatement.Currency = swiftData.Substring(6, 3);
                try
                {
                    decimal balance = DebitCreditIndicator * Convert.ToDecimal(swiftData.Substring(9).Replace(",",
                            Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator));

                    // Use first start balance. If missing, use intermediate balance.
                    if (swiftTag == "60F" || SWIFTStatement.StartBalance == 0 && swiftTag == "60M")
                    {
                        SWIFTStatement.StartBalance = balance;
                        SWIFTStatement.EndBalance = balance;
                    }
                }
                catch (FormatException)
                {
                    Log.Write($"Invalid balance: {swiftData}");
                }

                if (swiftTag == "60F" || swiftTag == "60M")
                {
                    SWIFTStatement.StartDate = postingDate;
                }
            }
            else if (swiftTag == "28C")
            {
                // this contains the number of the SWIFTStatement and the number of the page
                // only use for first page
                if (SWIFTStatement.SwiftTransactions.Count == 0)
                {
                    if (swiftData.IndexOf("/") != -1)
                    {
                        SWIFTStatement.Id = swiftData.Substring(0, swiftData.IndexOf("/"));
                    }
                    else
                    {
                        // Realtime SWIFTStatement.
                        // Not use SWIFTStatement number 0, because Sparkasse has 0/1 for valid SWIFTStatements
                        SWIFTStatement.Id = string.Empty;
                    }
                }
            }
            else if (swiftTag == "61")
            {
                // If there is no SWIFTStatement available, create one
                if (SWIFTStatement == null)
                {
                    SWIFTStatement = new SwiftStatement();
                    SWIFTStatement.Lines.Add(new SwiftLine(swiftTag, swiftData));
                }

                SwiftTransaction SWIFTTransaction = new SwiftTransaction();
                SWIFTStatement.SwiftTransactions.Add(SWIFTTransaction);

                // Valuta date (YYMMDD)
                try
                {
                    SWIFTTransaction.ValueDate = new DateTime(2000 + Convert.ToInt32(swiftData.Substring(0, 2)),
                        Convert.ToInt32(swiftData.Substring(2, 2)),
                        Convert.ToInt32(swiftData.Substring(4, 2)));
                }
                catch (ArgumentOutOfRangeException)
                {
                    // we have had the situation in the bank file with a date 30 Feb 2010.
                    // probably because the instruction by the donor is to transfer the money on the 30 day each month
                    // use the last day of the month
                    int year = 2000 + Convert.ToInt32(swiftData.Substring(0, 2));
                    int month = Convert.ToInt32(swiftData.Substring(2, 2));
                    int day = DateTime.DaysInMonth(year, month);

                    SWIFTTransaction.ValueDate = new DateTime(year, month, day);
                }

                swiftData = swiftData.Substring(6);

                // Optional: Posting date (MMDD)
                if (Regex.IsMatch(swiftData, @"^\d{4}"))
                {
                    int year = SWIFTTransaction.ValueDate.Year;
                    int month = Convert.ToInt32(swiftData.Substring(0, 2));
                    int day = Convert.ToInt32(swiftData.Substring(2, 2));

                    // Posting date 30 Dec 2020, Valuta date 1 Jan 2020
                    if (month > SWIFTTransaction.ValueDate.Month && month == SWIFTTransaction.ValueDate.AddMonths(-1).Month)
                    {
                        year--;
                    }
                    // Posting date 1 Jan 2020, Valuta date 30 Dec 2020
                    else if (month < SWIFTTransaction.ValueDate.Month && month == SWIFTTransaction.ValueDate.AddMonths(1).Month)
                    {
                        year++;
                    }

                    SWIFTTransaction.InputDate = new DateTime(year, month, day);

                    swiftData = swiftData.Length > 4 ? swiftData.Substring(4) : string.Empty;
                }

                // Amount - some characters followed by an 'N'
                if (Regex.IsMatch(swiftData, @"^.+N"))
                {
                    // Debit or credit, or storno debit or credit
                    int debitCreditIndicator = 0;
                    if (swiftData[0] == 'R')
                    {
                        // Storno means: reverse the debit credit flag
                        debitCreditIndicator = (swiftData[1] == 'D' ? 1 : -1);
                        swiftData = swiftData.Substring(2);
                    }
                    else
                    {
                        debitCreditIndicator = (swiftData[0] == 'D' ? -1 : 1);
                        swiftData = swiftData.Substring(1);
                    }

                    // Sometimes there is something about currency
                    if (char.IsLetter(swiftData[0]))
                    {
                        // Just skip it for the moment
                        swiftData = swiftData.Substring(1);
                    }

                    // The amount, finishing with N
                    SWIFTTransaction.Amount =
                        debitCreditIndicator * Convert.ToDecimal(swiftData.Substring(0, swiftData.IndexOf("N")).Replace(",",
                                Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator));

                    SWIFTStatement.EndBalance += SWIFTTransaction.Amount;

                    var constIdx = swiftData.IndexOf("N");
                    swiftData = swiftData.Length > constIdx ? swiftData.Substring(constIdx) : string.Empty;
                }
                else
                {
                    return;
                }

                // Buchungsschlüssel
                if (Regex.IsMatch(swiftData, @"^N[A-Z0-9]{3}"))
                {
                    SWIFTTransaction.TransactionTypeId = swiftData.Substring(0, 4);

                    swiftData = swiftData.Length > 4 ? swiftData.Substring(4) : string.Empty;
                }
                else
                {
                    return;
                }

                // customer reference
                if (Regex.IsMatch(swiftData, @"^.+"))
                {
                    int idxDelimiter = swiftData.IndexOf("//");
                    if (idxDelimiter > 0)
                        SWIFTTransaction.CustomerReference = swiftData.Substring(0, idxDelimiter);
                    else
                        SWIFTTransaction.CustomerReference = swiftData;

                    if (idxDelimiter > 0)
                        swiftData = swiftData.Length > idxDelimiter + 2 ? swiftData.Substring(idxDelimiter + 2) : string.Empty;
                    else
                        swiftData = string.Empty;
                }
                else
                {
                    return;
                }

                // Optional: bank reference; ends with CR/LF if followed by other data
                if (Regex.IsMatch(swiftData, @"^.+?\r\n", RegexOptions.Singleline))
                {
                    int lineBreakIdx = swiftData.IndexOf("\r\n");
                    if (lineBreakIdx > 0)
                    {
                        SWIFTTransaction.BankReference = swiftData.Substring(0, lineBreakIdx);
                        swiftData = swiftData.Substring(lineBreakIdx + 2);
                    }
                    else
                    {
                        SWIFTTransaction.BankReference = swiftData;
                        swiftData = string.Empty;
                    }
                }

                // Optional: other data
                if (!string.IsNullOrWhiteSpace(swiftData))
                {
                    SWIFTTransaction.OtherInformation = swiftData;
                }
            }
            else if (swiftTag == "86")
            {
                // Remove line breaks
                swiftData = swiftData.Replace("\r\n", string.Empty);

                SwiftTransaction SWIFTTransaction = SWIFTStatement.SwiftTransactions[SWIFTStatement.SwiftTransactions.Count - 1];

                // Geschaeftsvorfallcode
                SWIFTTransaction.TypeCode = swiftData.Substring(0, 3);

                swiftData = swiftData.Substring(3);

                if (swiftData.Length == 0)
                    return;

                char separator = swiftData[0];

                swiftData = swiftData.Substring(1);

                string[] elements = swiftData.Split(new char[] { separator });
                string lastDescriptionSubfield = string.Empty;
                foreach (string element in elements)
                {
                    int key = 0;
                    string value = element;

                    try
                    {
                        key = Convert.ToInt32(element.Substring(0, 2));
                        value = element.Substring(2);
                    }
                    catch
                    {
                        // If there is a question mark in the description, then we get here
                    }

                    if (key == 0)
                    {
                        // Buchungstext
                        SWIFTTransaction.Text = value;
                    }
                    else if (key == 10)
                    {
                        // Primanotennummer
                        SWIFTTransaction.Primanota = value;
                    }
                    else if ((key >= 11) && (key <= 19))
                    {
                        // Ignore
                        // Unknown meaning
                    }
                    else if ((key >= 20) && (key <= 29))
                    {
                        // No space between description lines
                        if (value.EndsWith(" "))
                            SWIFTTransaction.Description += value;
                        else
                            SWIFTTransaction.Description += value + " ";
                        AssignDescriptionSubField(SWIFTTransaction, value, ref lastDescriptionSubfield);
                        SWIFTTransaction.Description = SWIFTTransaction.Description.TrimEnd(' ');
                    }
                    else if (key == 30)
                    {
                        SWIFTTransaction.BankCode = value;
                    }
                    else if (key == 31)
                    {
                        SWIFTTransaction.AccountCode = value;
                    }
                    else if ((key == 32) || (key == 33))
                    {
                        SWIFTTransaction.PartnerName += value;
                    }
                    else if (key == 34)
                    {
                        // Textschlüsselergänzung
                        SWIFTTransaction.TextKeyAddition = value;
                    }
                    else if ((key == 35) || (key == 36))
                    {
                        // Empfängername
                        SWIFTTransaction.Description += value;
                    }
                    else if ((key >= 60) && (key <= 63))
                    {
                        SWIFTTransaction.Description += value;
                        AssignDescriptionSubField(SWIFTTransaction, value, ref lastDescriptionSubfield);
                    }
                    else
                    {
                        // Unknown key
                        return;
                    }
                }
            }
            else if (swiftTag.StartsWith("62")) // Schlusssaldo
            {
                // 62M: Finish page
                // 62F: Finish SWIFTStatement
                int debitCreditIndicator = (swiftData[0] == 'D' ? -1 : 1);
                swiftData = swiftData.Substring(1);

                // Posting date YYMMDD
                DateTime postingDate = new DateTime(2000 + Convert.ToInt32(swiftData.Substring(0, 2)),
                    Convert.ToInt32(swiftData.Substring(2, 2)),
                    Convert.ToInt32(swiftData.Substring(4, 2)));

                swiftData = swiftData.Substring(6);

                // Currency
                if (swiftData.Length > 3) // Assure that currency and end balance are valid
                {
                    swiftData = swiftData.Substring(3);

                    // Sometimes, this line is the last line, and it has -NULNULNUL at the end
                    if (swiftData.Contains("-\0"))
                    {
                        swiftData = swiftData.Substring(0, swiftData.IndexOf("-\0"));
                    }

                    // End balance
                    decimal endBalance = debitCreditIndicator * Convert.ToDecimal(swiftData.Replace(",",
                            Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator));
                    SWIFTStatement.EndBalance = endBalance;
                }

                if (swiftTag == "62F" || swiftTag == "62M")
                {
                    SWIFTStatement.EndDate = postingDate;
                    SWIFTStatements.Add(SWIFTStatement);

                    // Process missing input dates
                    foreach (var tx in SWIFTStatement.SwiftTransactions)
                    {
                        if (tx.InputDate == default)
                        {
                            tx.InputDate = SWIFTStatement.EndDate;
                        }
                    }

                    SWIFTStatement = null;
                }
            }
            else if (swiftTag == "64")
            {
                // Valutensaldo
            }
            else if (swiftTag == "65")
            {
                // Future valutensaldo
            }

            // Begin MT942
            else if (swiftTag == "34F")
            {
                if (swiftData.Length >= 3)
                {
                    SWIFTStatement.Currency = swiftData.Substring(0, 3);
                    swiftData = swiftData.Length > 3 ? swiftData.Substring(3) : string.Empty;
                }

                // Kleinster Betrag der gemeldeten Umsätze
                if (Regex.IsMatch(swiftData, @"D?\d+,\d*"))
                {
                    bool debit = swiftData.Substring(0, 1) == "D";
                    decimal amount = 0;
                    if (debit)
                    {
                        decimal.TryParse(swiftData.Substring(1), out amount);
                        amount = amount * -1;
                    }
                    else
                    {
                        decimal.TryParse(swiftData, out amount);
                    }

                    SWIFTStatement.SmallestAmount = amount;
                }
                // Kleinster Betrag der gemeldeten Haben-Umsätze
                else if (Regex.IsMatch(swiftData, @"C\d+,\d*"))
                {
                    decimal.TryParse(swiftData.Substring(1), out decimal amount);

                    SWIFTStatement.SmallestCreditAmount = amount;
                }

            }
            else if (swiftTag == "13") // Deutsche Bank
            {
                if (Regex.IsMatch(swiftData, @"\d{10}"))
                {
                    DateTime.TryParseExact(swiftData, "yyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime creationDate);

                    SWIFTStatement.CreationDate = creationDate;
                }
            }
            else if (swiftTag == "13D")
            {
                if (Regex.IsMatch(swiftData, @"\d{10}(\+|-)\d{4}"))
                {
                    // Easier parsing
                    // 1912090901+0100 -> 1912090901+01:00
                    var dateStr = swiftData.Substring(0, 13) + ":" + swiftData.Substring(13, 2);
                    DateTimeOffset.TryParseExact(dateStr, "yyMMddHHmmzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset dateTimeOffset);

                    SWIFTStatement.CreationDate = dateTimeOffset.DateTime;
                }
                else
                {
                    DateTime.TryParseExact(swiftData, "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime creationDate);

                    SWIFTStatement.CreationDate = creationDate;
                }
            }
            else if (swiftTag == "90D" || swiftTag == "90C")
            {
                bool debit = swiftTag == "90D";
                bool previousTag90d = !debit && SWIFTStatement == null; // Previous tag has been 90D

                if (previousTag90d)
                    SWIFTStatement = SWIFTStatements.LastOrDefault();

                if (SWIFTStatement == null)
                    return;

                int count = 0;
                decimal amount = 0;
                string currency = null;
                var match = Regex.Match(swiftData, @"(\d+)([A-Z]{3})(\d+(,\d+)?)");
                if (match.Success)
                {
                    int.TryParse(match.Groups[1].Value, out count);

                    currency = match.Groups[2].Value;

                    decimal.TryParse(match.Groups[3].Value, NumberStyles.Number | NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("de-DE"), out amount);
                }

                if (SWIFTStatement.Currency == null)
                    SWIFTStatement.Currency = currency;

                if (debit)
                {
                    SWIFTStatement.CountDebit = count;
                    SWIFTStatement.AmountDebit = amount * -1;
                }
                else
                {
                    SWIFTStatement.CountCredit = count;
                    SWIFTStatement.AmountCredit = amount;
                }

                if (debit)
                {
                    SWIFTStatements.Add(SWIFTStatement);
                    SWIFTStatement = null;
                }
                else
                {
                    if (!previousTag90d)
                        SWIFTStatements.Add(SWIFTStatement);
                    SWIFTStatement = null;
                }
            }
            // End MT942

            else
            {
                // Unknown tag
                return;
            }
        }

        private static bool SetDescriptionSubField(string designator, SwiftTransaction transaction, string value)
        {
            switch (designator)
            {
                case "ABWA": transaction.ABWA += value; break;
                case "EREF": transaction.EREF += value; break;
                case "KREF": transaction.KREF += value; break;
                case "MREF": transaction.MREF += value; break;
                case "BREF": transaction.BREF += value; break;
                case "RREF": transaction.RREF += value; break;
                case "CRED": transaction.CRED += value; break;
                case "DEBT": transaction.DEBT += value; break;
                case "COAM": transaction.COAM += value; break;
                case "OAMT": transaction.OAMT += value; break;
                case "SVWZ": transaction.SVWZ += value; break;
                case "ABWE": transaction.ABWE += value; break;
                case "IBAN": transaction.IBAN += value; break;
                case "BIC": transaction.BIC += value; break;
                default:
                    //something is wrong here
                    return false;
            }
            return true;
        }

        private static void AssignDescriptionSubField(SwiftTransaction transaction, string value, ref string lastSubfield)
        {
            string pattern = $@"^((?<designator>EREF|KREF|MREF|BREF|RREF|CRED|DEBT|COAM|OAMT|SVWZ|ABWA|ABWE|IBAN|BIC)\+)(?<content>.+)";
            Match result = Regex.Match(value, pattern);
            if (result.Success)
            {
                if (SetDescriptionSubField(result.Groups["designator"].Value, transaction, result.Groups["content"].Value))
                    lastSubfield = result.Groups["designator"].Value;
                else
                    lastSubfield = string.Empty;
            }
            else if (!string.IsNullOrEmpty(lastSubfield))
                SetDescriptionSubField(lastSubfield, transaction, value);
        }

        private static string Read(ref string Content)
        {
            Int32 counter;

            for (counter = 0; counter < Content.Length; counter++)
            {
                if ((Content[counter] == (char) 10) || (Content[counter] == (char) 13) || (Content[counter] == '@'))
                {
                    break;
                }
            }

            string line = Content.Substring(0, counter);

            if ((counter < Content.Length) && (Content[counter] == (char) 13))
            {
                counter++;
            }

            if ((counter < Content.Length) && (Content[counter] == (char) 10))
            {
                counter++;
            }

            while ((counter < Content.Length) && (Content[counter] == '@'))
            {
                counter++;
            }

            if (counter < Content.Length)
            {
                Content = Content.Substring(counter);
            }
            else
            {
                Content = String.Empty;
            }

            line = line.Replace("™", "Ö");
            line = line.Replace("š", "Ü");
            line = line.Replace("Ž", "Ä");
            line = line.Replace("á", "ß");
            line = line.Replace("\\", "Ö");
            line = line.Replace("]", "Ü");
            line = line.Replace("[", "Ä");
            line = line.Replace("~", "ß");

            return line;
        }

        public static List<SwiftStatement> Serialize(string STA, string Account, bool writeToFile = false, bool pending = false)
        {
            int LineCounter = 0;

            string swiftTag = "";
            string swiftData = "";

            SWIFTStatements = new List<SwiftStatement>();
            SWIFTStatement = null;

            if (STA == null || STA.Length == 0)
                return SWIFTStatements;

            string dir = null;
            if (writeToFile)
            {
                dir = FinTsGlobals.ProgramBaseDir;

                dir = Path.Combine(dir, "STA");

                string filename = Path.Combine(dir, Helper.MakeFilenameValid(Account + "_" + DateTime.Now + ".STA"));

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                // STA
                if (!File.Exists(filename))
                {
                    using (File.Create(filename))
                    { };

                    File.AppendAllText(filename, STA);
                }
                else
                    File.AppendAllText(filename, STA);
            }

            while (STA.Length > 0)
            {
                string line = Read(ref STA);

                LineCounter++;

                if (line.Trim() == "-") // end of block
                {
                    // Process previously read swift chunk
                    if (swiftTag.Length > 0)
                    {
                        Data(swiftTag, swiftData);
                    }

                    swiftTag = string.Empty;
                    swiftData = string.Empty;
                    continue;
                }

                if (line.Length > 0)
                {
                    // A swift chunk starts with a swiftTag, which is between colons
                    if (Regex.IsMatch(line, @"^:[\w]+:"))
                    {
                        // Process previously read swift chunk
                        if (swiftTag.Length > 0)
                        {
                            Data(swiftTag, swiftData);
                        }

                        int posColon = line.IndexOf(":", 2);

                        swiftTag = line.Substring(1, posColon - 1);
                        swiftData = line.Substring(posColon + 1);
                    }
                    else
                    {
                        // The swift chunk is spread over several lines
                        swiftData = swiftData + "\r\n" + line;
                    }
                }
            }

            if (swiftTag.Length > 0)
            {
                Data(swiftTag, swiftData);
            }

            // If there are remaining unprocessed statements - add them
            if (SWIFTStatement != null)
            {
                SWIFTStatements.Add(SWIFTStatement);

                // Process missing input dates
                foreach (var tx in SWIFTStatement.SwiftTransactions)
                {
                    if (tx.InputDate == default)
                    {
                        tx.InputDate = SWIFTStatement.EndDate;
                    }
                }

                SWIFTStatement = null;
            }

            // Set pending
            if (pending)
            {
                foreach (var stmt in SWIFTStatements)
                {
                    stmt.Pending = true;
                }
            }

            // Parse SEPA purposes
            foreach (var stmt in SWIFTStatements)
            {
                foreach (var tx in stmt.SwiftTransactions)
                {
                    if (string.IsNullOrWhiteSpace(tx.Description))
                        continue;

                    // Collect all occuring SEPA purposes ordered by their position
                    List<Tuple<int, SepaPurpose>> indices = new List<Tuple<int, SepaPurpose>>();
                    foreach (SepaPurpose sepaPurpose in Enum.GetValues(typeof(SepaPurpose)))
                    {
                        string prefix = $"{sepaPurpose}+";
                        var idx = tx.Description.IndexOf(prefix);
                        if (idx >= 0)
                        {
                            indices.Add(Tuple.Create(idx, sepaPurpose));
                        }
                    }
                    indices = indices.OrderBy(v => v.Item1).ToList();

                    // Then get the values
                    for (int i = 0; i < indices.Count; i++)
                    {
                        var beginIdx = indices[i].Item1 + $"{indices[i].Item2}+".Length;
                        var endIdx = i < indices.Count - 1 ? indices[i + 1].Item1 : tx.Description.Length;

                        var value = tx.Description.Substring(beginIdx, endIdx - beginIdx);
                        tx.SepaPurposes[indices[i].Item2] = value;
                    }
                }
            }

            if (Trace.Enabled)
            {
                foreach (SwiftStatement statement in SWIFTStatements)
                {
                    var ID = statement.Id;
                    var AccountCode = statement.AccountCode;
                    var BanksortCode = statement.BankCode;
                    var Currency = statement.Currency;
                    var StartDate = $"{statement.StartDate:d}";
                    var StartBalance = statement.StartBalance.ToString();
                    var EndDate = $"{statement.EndDate:d}";
                    var EndBalance = statement.EndBalance.ToString();

                    foreach (SwiftTransaction transaction in statement.SwiftTransactions)
                    {
                        var PartnerName = transaction.PartnerName;
                        var AccountCode_ = transaction.AccountCode;
                        var BankCode = transaction.BankCode;
                        var Description = transaction.Description;
                        var Text = transaction.Text;
                        var TypeCode = transaction.TypeCode;
                        var Amount = transaction.Amount.ToString();

                        var UMS = "++STARTUMS++" + "ID: " + ID + " ' " +
                            "AccountCode: " + AccountCode + " ' " +
                            "BanksortCode: " + BanksortCode + " ' " +
                            "Currency: " + Currency + " ' " +
                            "StartDate: " + StartDate + " ' " +
                            "StartBalance: " + StartBalance + " ' " +
                            "EndDate: " + EndDate + " ' " +
                            "EndBalance: " + EndBalance + " ' " +
                            "PartnerName: " + PartnerName + " ' " +
                            "BankCode: " + BankCode + " ' " +
                            "Description: " + Description + " ' " +
                            "Text: " + Text + " ' " +
                            "TypeCode: " + TypeCode + " ' " +
                            "Amount: " + Amount + " ' " + "++ENDUMS++";

                        dir = FinTsGlobals.ProgramBaseDir;
                        dir = Path.Combine(dir, "MT940");

                        string filename_ = Path.Combine(dir, Helper.MakeFilenameValid(Account + "_" + DateTime.Now + ".MT940"));

                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }

                        // MT940
                        if (!File.Exists(filename_))
                        {
                            using (File.Create(filename_))
                            { };

                            File.AppendAllText(filename_, UMS);
                        }
                        else
                            File.AppendAllText(filename_, UMS);
                    }
                }
            }

            return SWIFTStatements;
        }
    }
}
