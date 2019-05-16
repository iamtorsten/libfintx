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

/*
 *
 *	Based on Timotheus Pokorra's C# implementation of OpenPetraPlugin_BankimportMT940,
 *	available at https://github.com/SolidCharity/OpenPetraPlugin_BankimportMT940/blob/master/Client/ParseMT940.cs
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace libfintx
{
    /// <summary>
    /// MT940 account statement
    /// </summary>
    public static class MT940
    {
        public static List<SWIFTStatement> SWIFTStatements;
        private static SWIFTStatement SWIFTStatement = null;

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
                SWIFTStatement.lines.Add(new SWIFTLine(swiftTag, swiftData));
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
                    SWIFTStatement = new SWIFTStatement() { type = swiftData };
                    SWIFTStatement.lines.Add(new SWIFTLine(swiftTag, swiftData));
                }
            }
            else if (swiftTag == "25")
            {
                int posSlash = swiftData.IndexOf("/");

                SWIFTStatement.bankCode = swiftData.Substring(0, posSlash);
                SWIFTStatement.accountCode = LTrim(swiftData.Substring(posSlash + 1));
            }
            else if (swiftTag.StartsWith("60"))
            {
                // 60M is the start balance on each page of the SWIFTStatement.
                // 60F is the start balance of the whole SWIFTStatement.

                // First character is D or C
                int DebitCreditIndicator = (swiftData[0] == 'D' ? -1 : +1);

                // Next 6 characters: YYMMDD
                // Next 3 characters: Currency
                // Last characters: Balance with comma for decimal point
                SWIFTStatement.currency = swiftData.Substring(7, 3);
                decimal balance = DebitCreditIndicator * Convert.ToDecimal(swiftData.Substring(10).Replace(",",
                        Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator));

                // Use first start balance. If missing, use intermediate balance.
                if (swiftTag == "60F" || SWIFTStatement.startBalance == 0 && swiftTag == "60M")
                {
                    SWIFTStatement.startBalance = balance;
                    SWIFTStatement.endBalance = balance;
                }
            }
            else if (swiftTag == "28C")
            {
                // this contains the number of the SWIFTStatement and the number of the page
                // only use for first page
                if (SWIFTStatement.SWIFTTransactions.Count == 0)
                {
                    if (swiftData.IndexOf("/") != -1)
                    {
                        SWIFTStatement.id = swiftData.Substring(0, swiftData.IndexOf("/"));
                    }
                    else
                    {
                        // Realtime SWIFTStatement.
                        // Not use SWIFTStatement number 0, because Sparkasse has 0/1 for valid SWIFTStatements
                        SWIFTStatement.id = string.Empty;
                    }
                }
            }
            else if (swiftTag == "61")
            {
                // If there is no SWIFTStatement available, create one
                if (SWIFTStatement == null)
                {
                    SWIFTStatement = new SWIFTStatement();
                    SWIFTStatement.lines.Add(new SWIFTLine(swiftTag, swiftData));
                }

                SWIFTTransaction SWIFTTransaction = new SWIFTTransaction();

                // Valuta date (YYMMDD)
                try
                {
                    SWIFTTransaction.valueDate = new DateTime(2000 + Convert.ToInt32(swiftData.Substring(0, 2)),
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

                    SWIFTTransaction.valueDate = new DateTime(year, month, day);
                }

                swiftData = swiftData.Substring(6);

                if (char.IsDigit(swiftData[0]))
                {
                    // Posting date (MMDD)
                    int year = SWIFTTransaction.valueDate.Year;
                    int month = Convert.ToInt32(swiftData.Substring(0, 2));
                    int day = Convert.ToInt32(swiftData.Substring(2, 2));

                    // Posting date 30 Dec 2017, Valuta date 1 Jan 2018
                    if (month > SWIFTTransaction.valueDate.Month && month == SWIFTTransaction.valueDate.AddMonths(-1).Month)
                    {
                        year--;
                    }
                    // Posting date 1 Jan 2018, Valuta date 30 Dec 2017
                    else if (month < SWIFTTransaction.valueDate.Month && month == SWIFTTransaction.valueDate.AddMonths(1).Month)
                    {
                        year++;
                    }

                    SWIFTTransaction.inputDate = new DateTime(year, month, day);

                    swiftData = swiftData.Substring(4);
                }
                else
                {
                    SWIFTTransaction.inputDate = SWIFTTransaction.valueDate;
                }

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
                if (Char.IsLetter(swiftData[0]))
                {
                    // Just skip it for the moment
                    swiftData = swiftData.Substring(1);
                }

                // The amount, finishing with N
                SWIFTTransaction.amount =
                    debitCreditIndicator * Convert.ToDecimal(swiftData.Substring(0, swiftData.IndexOf("N")).Replace(",",
                            Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator));

                SWIFTStatement.endBalance += SWIFTTransaction.amount;
                swiftData = swiftData.Substring(swiftData.IndexOf("N"));

                // Geschaeftsvorfallcode
                swiftData = swiftData.Substring(4);

                // The following sub fields are ignored
                // Optional: customer reference; ends with //
                // Optional: bank reference; ends with CR/LF
                // Something else about original currency and SWIFTTransaction fees

                SWIFTStatement.SWIFTTransactions.Add(SWIFTTransaction);
            }
            else if (swiftTag == "86")
            {
                SWIFTTransaction SWIFTTransaction = SWIFTStatement.SWIFTTransactions[SWIFTStatement.SWIFTTransactions.Count - 1];

                // Geschaeftsvorfallcode
                SWIFTTransaction.typecode = swiftData.Substring(0, 3);

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
                        SWIFTTransaction.text = value;
                    }
                    else if (key == 10)
                    {
                        // Primanotennummer
                    }
                    else if ((key >= 11) && (key <= 19))
                    {
                        // Ignore
                        // Unknown meaning
                    }
                    else if ((key >= 20) && (key <= 29))
                    {
                        // No space between description lines
                        SWIFTTransaction.description += value;
						AssignDescriptionSubField(SWIFTTransaction, value, ref lastDescriptionSubfield);
					}
                    else if (key == 30)
                    {
                        SWIFTTransaction.bankCode = value;
                    }
                    else if (key == 31)
                    {
                        SWIFTTransaction.accountCode = value;
                    }
                    else if ((key == 32) || (key == 33))
                    {
                        SWIFTTransaction.partnerName += value;
                    }
                    else if (key == 34)
                    {
                        // Textschlüsselergänzung
                    }
                    else if ((key == 35) || (key == 36))
                    {
                        // Empfängername
                        SWIFTTransaction.description += value;
                    }
                    else if ((key >= 60) && (key <= 63))
                    {
                        SWIFTTransaction.description += value;
						AssignDescriptionSubField(SWIFTTransaction, value, ref lastDescriptionSubfield);
					}
                    else
                    {
                        // Unknown key
                        return;
                    }
                }
            }
            else if (swiftTag.StartsWith("62"))
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
                swiftData = swiftData.Substring(3);

                // Sometimes, this line is the last line, and it has -NULNULNUL at the end
                if (swiftData.Contains("-\0"))
                {
                    swiftData = swiftData.Substring(0, swiftData.IndexOf("-\0"));
                }

                // End balance
                decimal endBalance = debitCreditIndicator * Convert.ToDecimal(swiftData.Replace(",",
                        Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator));
                SWIFTStatement.endBalance = endBalance;

                if (swiftTag == "62F" || swiftTag == "62M")
                {
                    SWIFTStatement.date = postingDate;
                    SWIFTStatements.Add(SWIFTStatement);
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
            else
            {
                // Unknown tag
                return;
            }
        }

		private static bool SetDescriptionSubField(string designator, SWIFTTransaction transaction, string value)
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
		private static void AssignDescriptionSubField(SWIFTTransaction transaction, string value, ref string lastSubfield)
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
                if ((Content[counter] == (char)10) || (Content[counter] == (char)13))
                {
                    break;
                }
            }

            string line = Content.Substring(0, counter);

            if ((counter < Content.Length) && (Content[counter] == (char)13))
            {
                counter++;
            }

            if ((counter < Content.Length) && (Content[counter] == (char)10))
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

        public static List<SWIFTStatement> Serialize(string STA, string Account, bool writeToFile = true)
        {
            int LineCounter = 0;

            string swiftTag = "";
            string swiftData = "";

            SWIFTStatements = new List<SWIFTStatement>();
            SWIFTStatement = null;

            if (STA == null || STA.Length == 0)
                return SWIFTStatements;

            string dir = null;
            string documents = null;
            if (writeToFile)
            {
                documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dir = Path.Combine(documents, Program.Buildname);

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

                if ((line.Length > 0) && (!line.StartsWith("-")))
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
                        swiftData = swiftData + line;
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
                SWIFTStatement = null;
            }

            // Parse SEPA purposes
            foreach (var stmt in SWIFTStatements)
            {
                foreach (var tx in stmt.SWIFTTransactions)
                {
                    if (string.IsNullOrWhiteSpace(tx.description))
                        continue;

                    // Collect all occuring SEPA purposes ordered by their position
                    List<Tuple<int, SWIFTTransaction.SEPAPurpose>> indices = new List<Tuple<int, SWIFTTransaction.SEPAPurpose>>();
                    foreach (SWIFTTransaction.SEPAPurpose sepaPurpose in Enum.GetValues(typeof(SWIFTTransaction.SEPAPurpose)))
                    {
                        string prefix = $"{sepaPurpose}+";
                        var idx = tx.description.IndexOf(prefix);
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
                        var endIdx = i < indices.Count - 1 ? indices[i + 1].Item1 : tx.description.Length;

                        var value = tx.description.Substring(beginIdx, endIdx - beginIdx);
                        tx.SEPAPurposes[indices[i].Item2] = value;
                    }
                }
            }

            if (Trace.Enabled)
            {
                foreach (SWIFTStatement statement in SWIFTStatements)
                {
                    var ID = statement.id;
                    var Date = statement.date.ToShortDateString();
                    var AccountCode = statement.accountCode;
                    var BanksortCode = statement.bankCode;
                    var Currency = statement.currency;
                    var StartBalance = statement.startBalance.ToString();
                    var EndBalance = statement.endBalance.ToString();

                    foreach (SWIFTTransaction transaction in statement.SWIFTTransactions)
                    {
                        var PartnerName = transaction.partnerName;
                        var AccountCode_ = transaction.accountCode;
                        var BankCode = transaction.bankCode;
                        var Description = transaction.description;
                        var Text = transaction.text;
                        var TypeCode = transaction.typecode;
                        var Amount = transaction.amount.ToString();

                        var UMS = "++STARTUMS++" + "ID: " + ID + " ' " +
                            "Date: " + Date + " ' " +
                            "AccountCode: " + AccountCode + " ' " +
                            "BanksortCode: " + BanksortCode + " ' " +
                            "Currency: " + Currency + " ' " +
                            "StartBalance: " + StartBalance + " ' " +
                            "EndBalance: " + EndBalance + " ' " +
                            "PartnerName: " + PartnerName + " ' " +
                            "BankCode: " + BankCode + " ' " +
                            "Description: " + Description + " ' " +
                            "Text: " + Text + " ' " +
                            "TypeCode: " + TypeCode + " ' " +
                            "Amount: " + Amount + " ' " + "++ENDUMS++";

                        dir = Path.Combine(documents, Program.Buildname);
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
