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
 *	Based on Timotheus Pokorra's C# implementation of OpenPetraPlugin_BankimportCAMT
 *	available at https://github.com/SolidCharity/OpenPetraPlugin_BankimportCAMT/blob/master/Client/ParseCAMT.cs
 *
 */

using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Threading;
using System.Text;
using System.Xml.Serialization;
using libfintx.camt_053_001_02;

namespace libfintx
{
    /// <summary>
    /// parses bank statement files (ISO 20022 CAMT.053) in Germany;
    /// for the structure of the file see
    /// https://www.rabobank.com/nl/images/Format%20description%20CAMT.053.pdf
    /// http://www.national-bank.de/fileadmin/user_upload/nationalbank/Service_Center/Electronic_Banking_Center/Downloads/Handbuecher_und_Bedingungen/SRZ-Anlage_5b_Kontoauszug_ISO_20022_camt_2010-06-15b.pdf
    /// http://www.hettwer-beratung.de/sepa-spezialwissen/sepa-technische-anforderungen/camt-format-camt-053/
    /// </summary>
    public class TCAM053TParser
    {
        /// <summary>
        /// the parsed bank statements
        /// </summary>
        public List<TStatement> statements;

        public void ProcessDocument(string xmlDocument, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Document));
                byte[] bytes = encoding.GetBytes(xmlDocument);
                Document document;
                using (var stream = new MemoryStream(bytes))
                    document = (Document)serializer.Deserialize(stream);

                ProcessDocument(document);
            }
            catch (Exception e)
            {
                throw new Exception("problem with file " + e.Message + Environment.NewLine + e.StackTrace, e);
            }
        }

        /// <summary>
        /// processing CAMT file
        /// </summary>
        /// <param name="filename"></param>
        public void ProcessFile(string filename)
        {
            Log.Write("Read file " + filename);

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Document));
                Document document;
                using (var stream = new FileStream(filename, FileMode.Open))
                    document = (Document)serializer.Deserialize(stream);
            }
            catch (Exception e)
            {
                throw new Exception(
                    "problem with file " + filename + "; " + e.Message + Environment.NewLine + e.StackTrace);
            }
        }

        private void ProcessDocument(Document document)
        {
            statements = new List<TStatement>();

            AccountStatement2[] stmts = document.BkToCstmrStmt.Stmt;

            foreach (AccountStatement2 accStatement in stmts)
            {
                TStatement stmt = new TStatement();

                stmt.id = accStatement.Id;
                stmt.elctrncSeqNb = accStatement.ElctrncSeqNb.ToString();

                object accReportAccount = accStatement.Acct?.Id?.Item;
                if (accReportAccount is string)
                    stmt.accountCode = (string)accReportAccount;
                else if (accReportAccount is GenericAccountIdentification1)
                    stmt.accountCode = ((GenericAccountIdentification1)accReportAccount).Id;

                stmt.bankCode = accStatement.Acct?.Svcr?.FinInstnId?.BIC;
                stmt.currency = accStatement.Acct?.Ccy;

                stmt.severalYears = false;
                string nm = accStatement.Acct?.Ownr?.Nm;
                string ownName = nm ?? "AccountNameFor" + stmt.bankCode + "/" + stmt.accountCode;

                CashBalance3[] balances = accStatement.Bal;
                if (balances != null)
                {
                    foreach (CashBalance3 balance in balances)
                    {
                        // PRCD: PreviouslyClosedBooked
                        if (balance.Tp?.CdOrPrtry?.Item?.ToString() == "PRCD")
                        {
                            stmt.startBalance = balance.Amt.Value;

                            // CreditDebitIndicator: CRDT or DBIT for credit or debit
                            if (balance.CdtDbtInd == CreditDebitCode.DBIT)
                            {
                                stmt.startBalance *= -1.0m;
                            }

                            stmt.startDate = balance.Dt.Item;
                        }
                        // CLBD: ClosingBooked
                        else if (balance.Tp?.CdOrPrtry?.Item?.ToString() == "CLBD")
                        {
                            stmt.endBalance = balance.Amt.Value;

                            // CreditDebitIndicator: CRDT or DBIT for credit or debit
                            if (balance.CdtDbtInd == CreditDebitCode.DBIT)
                            {
                                stmt.endBalance *= -1.0m;
                            }

                            stmt.endDate = balance.Dt.Item;
                        }

                        // ITBD: InterimBooked
                        // CLAV: ClosingAvailable
                        // FWAV: ForwardAvailable
                    }
                }

                //string strDiffBalance = "DiffBalanceFor" + stmt.bankCode + "/" + stmt.accountCode;
                //Decimal DiffBalance = 0.0m;
                //if (Decimal.TryParse(strDiffBalance, out DiffBalance))
                //{
                //    stmt.startBalance += DiffBalance;
                //    stmt.endBalance += DiffBalance;
                //}
                //else
                //{
                //    Log.Write("problem parsing decimal from configuration setting DiffBalanceFor" + stmt.bankCode + "/" + stmt.accountCode);
                //}

                //string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);

                //// if the file has already been split and moved, use the statement date from the file name (if it is on the last of december)
                //if (!filenameWithoutExtension.Contains("_C53_")
                //    && filenameWithoutExtension.EndsWith("1231")
                //    && stmt.date.Month != 12
                //    && stmt.date.Day != 31)
                //{
                //    stmt.date = new DateTime(stmt.date.Year - 1, 12, 31);
                //}

                ReportEntry2[] entries = accStatement.Ntry;
                if (entries != null)
                {
                    foreach (ReportEntry2 entry in entries)
                    {
                        if (entry.Amt.Ccy != stmt.currency)
                        {
                            throw new Exception("transaction currency " + entry.Amt.Ccy + " does not match the bank statement currency");
                        }

                        TTransaction tr = new TTransaction();

                        tr.pending = entry.Sts == EntryStatus2Code.PDNG;

                        if (entry.BookgDt != null)
                        {
                            tr.inputDate = entry.BookgDt.Item;
                        }
                        if (entry.ValDt != null)
                        {
                            tr.valueDate = entry.ValDt.Item;
                            if (tr.valueDate.Year != stmt.startDate.Year)
                            {
                                stmt.severalYears = true;
                            }
                        }

                        // Soll/Haben
                        tr.amount = entry.Amt.Value;
                        bool debit = entry.CdtDbtInd == CreditDebitCode.DBIT;
                        if (debit)
                        {
                            tr.amount *= -1.0m;
                        }

                        tr.storno = entry.RvslInd;

                        EntryDetails1 entryDetails = entry.NtryDtls?.FirstOrDefault();
                        EntryTransaction2 txDetails = entryDetails?.TxDtls?.FirstOrDefault();

                        // Verwendungszweck
                        if (txDetails?.RmtInf?.Ustrd != null)
                        {
                            tr.description = string.Join(string.Empty, txDetails.RmtInf.Ustrd.Select(s => s?.Trim()));
                        }

                        tr.text = entry.AddtlNtryInf?.Trim();

                        tr.bankCode = debit ?
                            txDetails?.RltdAgts?.CdtrAgt?.FinInstnId?.BIC :
                            txDetails?.RltdAgts?.DbtrAgt?.FinInstnId?.BIC;

                        tr.partnerName = debit ?
                            txDetails?.RltdPties?.Cdtr?.Nm :
                            txDetails?.RltdPties?.Dbtr?.Nm;

                        object account = debit ?
                            txDetails?.RltdPties?.CdtrAcct?.Id?.Item :
                            txDetails?.RltdPties?.DbtrAcct?.Id?.Item;
                        if (account is string)
                            tr.accountCode = (string)account;
                        else if (account is GenericAccountIdentification1)
                            tr.accountCode = ((GenericAccountIdentification1)account).Id;

                        string CrdtName = txDetails?.RltdPties?.Cdtr?.Nm;
                        string DbtrName = txDetails?.RltdPties?.Dbtr?.Nm;

                        if ((CrdtName != null) && (CrdtName != ownName))
                        {
                            if ((DbtrName != null) && (DbtrName == ownName))
                            {
                                // we are the debitor
                            }
                            else if (ownName != string.Empty)
                            {
                                // sometimes donors write the project or recipient in the field where the organisation is supposed to be
                                Log.Write("CrdtName is not like expected: " + tr.description + " --- " + CrdtName);
                            }
                        }

                        tr.endToEndId = txDetails?.Refs?.EndToEndId;

                        tr.messageId = txDetails?.Refs?.MsgId;
                        tr.paymentInformationId = txDetails?.Refs?.PmtInfId;
                        tr.mandateId = txDetails?.Refs?.MndtId;
                        tr.proprietaryRef = txDetails?.Refs?.Prtry?.Ref;

                        tr.customerRef = entry.AcctSvcrRef;

                        if (txDetails?.BkTxCd.Prtry.Cd != null)
                        {
                            // NTRF+177+9310+997
                            // NSTO+152+00900. look for SEPA Geschäftsvorfallcodes
                            // see the codes: https://www.hettwer-beratung.de/business-portfolio/zahlungsverkehr/elektr-kontoinformationen-swift-mt-940/
                            string[] GVCCode = txDetails?.BkTxCd?.Prtry?.Cd?.Split(new char[] { '+' });
                            if (GVCCode.Length > 0)
                                tr.transactionTypeId = GVCCode[0];
                            if (GVCCode.Length > 1)
                                tr.typecode = GVCCode[1];
                            if (GVCCode.Length > 2)
                                tr.primanota = GVCCode[2];
                            if (GVCCode.Length > 3)
                                tr.textKeyAddition = GVCCode[3];
                        }

                        // for SEPA direct debit batches, there are multiple TxDtls records
                        if (entryDetails?.TxDtls?.Count() > 1)
                        {
                            tr.partnerName = string.Empty;
                            tr.description = string.Format("SEPA Sammel-Basislastschrift mit {0} Lastschriften", entryDetails?.TxDtls?.Count());
                        }

                        stmt.transactions.Add(tr);

                        Log.Write("count : " + stmt.transactions.Count.ToString());
                    }
                }

                statements.Add(stmt);
            }
        }
    }
}