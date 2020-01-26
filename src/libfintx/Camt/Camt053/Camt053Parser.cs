/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2020 Torsten Klinger
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace libfintx.Camt.Camt053
{
    /// <summary>
    /// parses bank statement files (ISO 20022 CAMT.053) in Germany;
    /// for the structure of the file see
    /// https://www.rabobank.com/nl/images/Format%20description%20CAMT.053.pdf
    /// http://www.national-bank.de/fileadmin/user_upload/nationalbank/Service_Center/Electronic_Banking_Center/Downloads/Handbuecher_und_Bedingungen/SRZ-Anlage_5b_Kontoauszug_ISO_20022_camt_2010-06-15b.pdf
    /// http://www.hettwer-beratung.de/sepa-spezialwissen/sepa-technische-anforderungen/camt-format-camt-053/
    /// </summary>
    public class Camt053Parser
    {
        /// <summary>
        /// the parsed bank statements
        /// </summary>
        public List<CamtStatement> statements;

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
            statements = new List<CamtStatement>();

            AccountStatement2[] stmts = document.BkToCstmrStmt.Stmt;

            foreach (AccountStatement2 accStatement in stmts)
            {
                CamtStatement stmt = new CamtStatement();

                stmt.Id = accStatement.Id;
                stmt.ElctrncSeqNb = accStatement.ElctrncSeqNb.ToString();

                object accReportAccount = accStatement.Acct?.Id?.Item;
                if (accReportAccount is string)
                    stmt.AccountCode = (string)accReportAccount;
                else if (accReportAccount is GenericAccountIdentification1)
                    stmt.AccountCode = ((GenericAccountIdentification1)accReportAccount).Id;

                stmt.BankCode = accStatement.Acct?.Svcr?.FinInstnId?.BIC;
                stmt.Currency = accStatement.Acct?.Ccy;

                stmt.SeveralYears = false;
                string nm = accStatement.Acct?.Ownr?.Nm;
                string ownName = nm ?? "AccountNameFor" + stmt.BankCode + "/" + stmt.AccountCode;

                CashBalance3[] balances = accStatement.Bal;
                if (balances != null)
                {
                    foreach (CashBalance3 balance in balances)
                    {
                        // PRCD: PreviouslyClosedBooked
                        if (balance.Tp?.CdOrPrtry?.Item?.ToString() == "PRCD")
                        {
                            stmt.StartBalance = balance.Amt.Value;

                            // CreditDebitIndicator: CRDT or DBIT for credit or debit
                            if (balance.CdtDbtInd == CreditDebitCode.DBIT)
                            {
                                stmt.StartBalance *= -1.0m;
                            }

                            stmt.StartDate = balance.Dt.Item;
                        }
                        // CLBD: ClosingBooked
                        else if (balance.Tp?.CdOrPrtry?.Item?.ToString() == "CLBD")
                        {
                            stmt.EndBalance = balance.Amt.Value;

                            // CreditDebitIndicator: CRDT or DBIT for credit or debit
                            if (balance.CdtDbtInd == CreditDebitCode.DBIT)
                            {
                                stmt.EndBalance *= -1.0m;
                            }

                            stmt.EndDate = balance.Dt.Item;
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
                        if (entry.Amt.Ccy != stmt.Currency)
                        {
                            throw new Exception("transaction currency " + entry.Amt.Ccy + " does not match the bank statement currency");
                        }

                        CamtTransaction tr = new CamtTransaction();

                        tr.Pending = entry.Sts == EntryStatus2Code.PDNG;

                        if (entry.BookgDt != null)
                        {
                            tr.InputDate = entry.BookgDt.Item;
                        }
                        if (entry.ValDt != null)
                        {
                            tr.ValueDate = entry.ValDt.Item;
                            if (tr.ValueDate.Year != stmt.StartDate.Year)
                            {
                                stmt.SeveralYears = true;
                            }
                        }

                        // Soll/Haben
                        tr.Amount = entry.Amt.Value;
                        bool debit = entry.CdtDbtInd == CreditDebitCode.DBIT;
                        if (debit)
                        {
                            tr.Amount *= -1.0m;
                        }

                        tr.Storno = entry.RvslInd;

                        EntryDetails1 entryDetails = entry.NtryDtls?.FirstOrDefault();
                        EntryTransaction2 txDetails = entryDetails?.TxDtls?.FirstOrDefault();

                        // Verwendungszweck
                        if (txDetails?.RmtInf?.Ustrd != null)
                        {
                            tr.Description = string.Join(string.Empty, txDetails.RmtInf.Ustrd.Select(s => s?.Trim()));
                        }

                        tr.Text = entry.AddtlNtryInf?.Trim();

                        tr.BankCode = debit ?
                            txDetails?.RltdAgts?.CdtrAgt?.FinInstnId?.BIC :
                            txDetails?.RltdAgts?.DbtrAgt?.FinInstnId?.BIC;

                        tr.PartnerName = debit ?
                            txDetails?.RltdPties?.Cdtr?.Nm :
                            txDetails?.RltdPties?.Dbtr?.Nm;

                        object account = debit ?
                            txDetails?.RltdPties?.CdtrAcct?.Id?.Item :
                            txDetails?.RltdPties?.DbtrAcct?.Id?.Item;
                        if (account is string)
                            tr.AccountCode = (string)account;
                        else if (account is GenericAccountIdentification1)
                            tr.AccountCode = ((GenericAccountIdentification1)account).Id;

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
                                Log.Write("CrdtName is not like expected: " + tr.Description + " --- " + CrdtName);
                            }
                        }

                        tr.EndToEndId = txDetails?.Refs?.EndToEndId;

                        tr.MessageId = txDetails?.Refs?.MsgId;
                        tr.PaymentInformationId = txDetails?.Refs?.PmtInfId;
                        tr.MandateId = txDetails?.Refs?.MndtId;
                        tr.ProprietaryRef = txDetails?.Refs?.Prtry?.Ref;

                        tr.CustomerRef = entry.AcctSvcrRef;

                        if (txDetails?.BkTxCd.Prtry.Cd != null)
                        {
                            // NTRF+177+9310+997
                            // NSTO+152+00900. look for SEPA Geschäftsvorfallcodes
                            // see the codes: https://www.hettwer-beratung.de/business-portfolio/zahlungsverkehr/elektr-kontoinformationen-swift-mt-940/
                            string[] GVCCode = txDetails?.BkTxCd?.Prtry?.Cd?.Split(new char[] { '+' });
                            if (GVCCode.Length > 0)
                                tr.TransactionTypeId = GVCCode[0];
                            if (GVCCode.Length > 1)
                                tr.TypeCode = GVCCode[1];
                            if (GVCCode.Length > 2)
                                tr.Primanota = GVCCode[2];
                            if (GVCCode.Length > 3)
                                tr.TextKeyAddition = GVCCode[3];
                        }

                        // for SEPA direct debit batches, there are multiple TxDtls records
                        if (entryDetails?.TxDtls?.Count() > 1)
                        {
                            tr.PartnerName = string.Empty;
                            tr.Description = string.Format("SEPA Sammel-Basislastschrift mit {0} Lastschriften", entryDetails?.TxDtls?.Count());
                        }

                        stmt.Transactions.Add(tr);

                        Log.Write("count : " + stmt.Transactions.Count.ToString());
                    }
                }

                statements.Add(stmt);
            }
        }
    }
}