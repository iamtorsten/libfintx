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


using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Threading;
using System.Text;

namespace libfintx
{
    /// <summary>
    /// parses bank statement files (ISO 20022 CAMT.052) in Germany;
    /// </summary>
    public class TCAM052TParser
    {
        /// <summary>
        /// the parsed bank statements
        /// </summary>
        public List<TStatement> statements;
        private TStatement currentStatement = null;

        private static string WithoutLeadingZeros(string ACode)
        {
            // cut off leading zeros
            try
            {
                return Convert.ToInt64(ACode).ToString();
            }
            catch (Exception)
            {
                // IBAN or BIC
                return ACode;
            }
        }

        /// <summary>
        /// processing CAMT file
        /// </summary>
        /// <param name="filename"></param>
        public void ProcessFile(string filename)
        {
            Console.WriteLine("Read file " + filename);
            statements = new List<TStatement>();

            CultureInfo backupCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);
                string namespaceName = "urn:iso:std:iso:20022:tech:xsd:camt.052.001.02";
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("camt", namespaceName);

                XmlNode nodeDocument = doc.DocumentElement;

                if ((nodeDocument == null) || (nodeDocument.Attributes["xmlns"].Value != namespaceName))
                {
                    throw new Exception("expecting xmlns = '" + namespaceName + "'");
                }

                XmlNodeList stmts = nodeDocument.SelectNodes("camt:BkToCstmrAcctRpt/camt:Rpt", nsmgr);
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                foreach (XmlNode nodeStatement in stmts)
                {
                    TStatement stmt = new TStatement();
                    currentStatement = stmt;

                    stmt.id = nodeStatement.SelectSingleNode("camt:ElctrncSeqNb", nsmgr).InnerText;
                    stmt.accountCode = nodeStatement.SelectSingleNode("camt:Acct/camt:Id/camt:IBAN", nsmgr).InnerText;
                    stmt.bankCode = nodeStatement.SelectSingleNode("camt:Acct/camt:Svcr/camt:FinInstnId/camt:BIC", nsmgr).InnerText;
                    stmt.currency = nodeStatement.SelectSingleNode("camt:Acct/camt:Ccy", nsmgr).InnerText;

                    stmt.severalYears = false;
                    XmlNode nm = nodeStatement.SelectSingleNode("camt:Acct/camt:Ownr/camt:Nm", nsmgr);
                    string ownName = nm != null ? nm.InnerText :
                        "AccountNameFor" + stmt.bankCode + "/" + stmt.accountCode;
                    XmlNodeList nodeBalances = nodeStatement.SelectNodes("camt:Bal", nsmgr);

                    foreach (XmlNode nodeBalance in nodeBalances)
                    {
                        // PRCD: PreviouslyClosedBooked
                        if (nodeBalance.SelectSingleNode("camt:Tp/camt:CdOrPrtry/camt:Cd", nsmgr).InnerText == "PRCD")
                        {
                            stmt.startBalance = Decimal.Parse(nodeBalance.SelectSingleNode("camt:Amt", nsmgr).InnerText);

                            // CreditDebitIndicator: CRDT or DBIT for credit or debit
                            if (nodeBalance.SelectSingleNode("camt:CdtDbtInd", nsmgr).InnerText == "DBIT")
                            {
                                stmt.startBalance *= -1.0m;
                            }

                            stmt.date = DateTime.Parse(nodeBalance.SelectSingleNode("camt:Dt", nsmgr).InnerText);
                        }
                        // CLBD: ClosingBooked
                        else if (nodeBalance.SelectSingleNode("camt:Tp/camt:CdOrPrtry/camt:Cd", nsmgr).InnerText == "CLBD")
                        {
                            stmt.endBalance = Decimal.Parse(nodeBalance.SelectSingleNode("camt:Amt", nsmgr).InnerText);

                            // CreditDebitIndicator: CRDT or DBIT for credit or debit
                            if (nodeBalance.SelectSingleNode("camt:CdtDbtInd", nsmgr).InnerText == "DBIT")
                            {
                                stmt.endBalance *= -1.0m;
                            }

                            stmt.date = DateTime.Parse(nodeBalance.SelectSingleNode("camt:Dt", nsmgr).InnerText);
                        }

                        // ITBD: InterimBooked
                        // CLAV: ClosingAvailable
                        // FWAV: ForwardAvailable
                    }

                    string strDiffBalance = "DiffBalanceFor" + stmt.bankCode + "/" + stmt.accountCode;
                    Decimal DiffBalance = 0.0m;
                    if (Decimal.TryParse(strDiffBalance, out DiffBalance))
                    {
                        stmt.startBalance += DiffBalance;
                        stmt.endBalance += DiffBalance;
                    }
                    else
                    {
                        Log.Write("problem parsing decimal from configuration setting DiffBalanceFor" + stmt.bankCode + "/" + stmt.accountCode);
                    }

                    string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);

                    // if the file has already been split and moved, use the statement date from the file name (if it is on the last of december)
                    if (!filenameWithoutExtension.Contains("_C53_")
                        && filenameWithoutExtension.EndsWith("1231")
                        && stmt.date.Month != 12
                        && stmt.date.Day != 31)
                    {
                        stmt.date = new DateTime(stmt.date.Year - 1, 12, 31);
                    }

                    XmlNodeList nodeEntries = nodeStatement.SelectNodes("camt:Ntry", nsmgr);

                    foreach (XmlNode nodeEntry in nodeEntries)
                    {
                        TTransaction tr = new TTransaction();
                        tr.inputDate = DateTime.Parse(nodeEntry.SelectSingleNode("camt:BookgDt/camt:Dt", nsmgr).InnerText);
                        tr.valueDate = DateTime.Parse(nodeEntry.SelectSingleNode("camt:ValDt/camt:Dt", nsmgr).InnerText);

                        if (tr.valueDate.Year != stmt.date.Year)
                        {
                            // ignore transactions that are in a different year than the statement
                            stmt.severalYears = true;
                            continue;
                        }

                        tr.amount = Decimal.Parse(nodeEntry.SelectSingleNode("camt:Amt", nsmgr).InnerText, NumberStyles.Currency);

                        if (nodeEntry.SelectSingleNode("camt:Amt", nsmgr).Attributes["Ccy"].Value != stmt.currency)
                        {
                            throw new Exception("transaction currency " + nodeEntry.SelectSingleNode("camt:Amt",
                                    nsmgr).Attributes["Ccy"].Value + " does not match the bank statement currency");
                        }

                        if (nodeEntry.SelectSingleNode("camt:CdtDbtInd", nsmgr).InnerText == "DBIT")
                        {
                            tr.amount *= -1.0m;
                        }

                        XmlNode desc = nodeEntry.SelectSingleNode("camt:NtryDtls/camt:TxDtls/camt:RmtInf/camt:Ustrd", nsmgr);
                        tr.description = desc != null ? desc.InnerText : String.Empty;
                        XmlNode partnerName = nodeEntry.SelectSingleNode("camt:NtryDtls/camt:TxDtls/camt:RltdPties/camt:Cdtr/camt:Nm", nsmgr);

                        if (partnerName != null)
                        {
                            tr.partnerName = partnerName.InnerText;
                        }

                        XmlNode accountCode = nodeEntry.SelectSingleNode("camt:NtryDtls/camt:TxDtls/camt:RltdPties/camt:DbtrAcct/camt:Id/camt:IBAN",
                            nsmgr);

                        if (accountCode != null)
                        {
                            tr.accountCode = accountCode.InnerText;
                        }

                        XmlNode CrdtName = nodeEntry.SelectSingleNode("camt:NtryDtls/camt:TxDtls/camt:RltdPties/camt:Cdtr/camt:Nm", nsmgr);
                        XmlNode DbtrName = nodeEntry.SelectSingleNode("camt:NtryDtls/camt:TxDtls/camt:RltdPties/camt:Dbtr/camt:Nm", nsmgr);

                        if ((CrdtName != null) && (CrdtName.InnerText != ownName))
                        {
                            if ((DbtrName != null) && (DbtrName.InnerText == ownName))
                            {
                                // we are the debitor
                            }
                            else if (ownName != String.Empty)
                            {
                                // sometimes donors write the project or recipient in the field where the organisation is supposed to be
                                Log.Write("CrdtName is not like expected: " + tr.description + " --- " + CrdtName.InnerText);
                                tr.description += " " + CrdtName.InnerText;
                            }
                        }

                        XmlNode EndToEndId = nodeEntry.SelectSingleNode("camt:NtryDtls/camt:TxDtls/camt:Refs/camt:EndToEndId", nsmgr);

                        if ((EndToEndId != null)
                            && (EndToEndId.InnerText != "NOTPROVIDED")
                            && !EndToEndId.InnerText.StartsWith("LS-")
                            && !EndToEndId.InnerText.StartsWith("ZV")
                            && !EndToEndId.InnerText.StartsWith("IZV-DISPO"))
                        {
                            // sometimes donors write the project or recipient in unexpected field
                            Log.Write("EndToEndId: " + tr.description + " --- " + EndToEndId.InnerText);
                            tr.description += " " + EndToEndId.InnerText;
                        }

                        // eg NSTO+152+00900. look for SEPA Geschäftsvorfallcodes
                        // see the codes: https://www.wgzbank.de/export/sites/wgzbank/de/wgzbank/downloads/produkte_leistungen/firmenkunden/zv_aktuelles/Uebersicht-GVC-und-Buchungstexte-WGZ-BANK_V062015.pdf
                        string[] GVCCode =
                            nodeEntry.SelectSingleNode("camt:NtryDtls/camt:TxDtls/camt:BkTxCd/camt:Prtry/camt:Cd", nsmgr).InnerText.Split(
                                new char[] { '+' });
                        tr.typecode = GVCCode[1];

                        // for SEPA direct debit batches, there are multiple TxDtls records
                        XmlNodeList transactionDetails = nodeEntry.SelectNodes("camt:NtryDtls/camt:TxDtls", nsmgr);

                        if (transactionDetails.Count > 1)
                        {
                            tr.partnerName = String.Empty;
                            tr.description = String.Format("SEPA Sammel-Basislastschrift mit {0} Lastschriften",
                                transactionDetails.Count);
                        }

                        stmt.transactions.Add(tr);

                        Log.Write("count : " + stmt.transactions.Count.ToString());
                    }

                    statements.Add(stmt);
                }
            }
            catch (Exception e)
            {
                throw new Exception(
                    "problem with file " + filename + "; " + e.Message + Environment.NewLine + e.StackTrace);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = backupCulture;
            }
        }
    }
}