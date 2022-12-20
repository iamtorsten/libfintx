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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using libfintx.Logger.Log;

namespace libfintx.FinTS.Camt.Camt052
{
    /// <summary>
    /// parses bank statement files (ISO 20022 CAMT.052) in Germany;
    /// </summary>
    public class Camt052Parser
    {
        /// <summary>
        /// the parsed bank statements
        /// </summary>
        public List<CamtStatement> statements;

        public void ProcessDocument(string xmlDocument, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            Type documentType;
            if (xmlDocument.Contains("urn:iso:std:iso:20022:tech:xsd:camt.052.001.08"))
            {
                documentType = typeof(Camt052_008.Document);
            }
            else
            {
                // Fallback
                documentType = typeof(Camt052_002.Document);
            }

            // Ungültige Datumsformate korrigieren (2022-08-15T24:00:00.000, Hypo Vereinsbank)
            var matchCollection = Regex.Matches(xmlDocument, @"(\d{4}-\d{2}-\d{2})(T24:)(\d{2}:\d{2}.\d{3})");
            foreach (Match match in matchCollection)
            {
                var hourPart = match.Groups[2];
                var minuteSecondPart = match.Groups[3];

                xmlDocument = xmlDocument.Substring(0, hourPart.Index) + "T00:" + xmlDocument.Substring(minuteSecondPart.Index, xmlDocument.Length - minuteSecondPart.Index);
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(documentType);
                byte[] bytes = encoding.GetBytes(xmlDocument);
                object document;
                using (var stream = new MemoryStream(bytes))
                    document = serializer.Deserialize(stream);

                if (documentType == typeof(Camt052_008.Document))
                {
                    ProcessDocument(document as Camt052_008.Document);
                }
                else
                {
                    ProcessDocument(document as Camt052_002.Document);
                }
            }
            catch (Exception e)
            {
                throw new Exception("problem with file " + e.Message + Environment.NewLine + e.StackTrace, e);
            }
        }
        public void ProcessFile(string filename, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            var xmlDocument = File.ReadAllText(filename, encoding);
            ProcessDocument(xmlDocument, encoding);
        }

        /// <summary>
        /// processing CAMT file
        /// </summary>
        /// <param name="filename"></param>
        public void ProcessFile(string filename)
        {
            Log.Write("Read file " + filename);

            Type documentType;
            if (File.ReadAllText(filename).Contains("urn:iso:std:iso:20022:tech:xsd:camt.052.001.08"))
            {
                documentType = typeof(Camt052_008.Document);
            }
            else
            {
                // Fallback
                documentType = typeof(Camt052_002.Document);
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(documentType);
                object document;
                using (var stream = new FileStream(filename, FileMode.Open))
                    document = serializer.Deserialize(stream);

                if (documentType == typeof(Camt052_008.Document))
                {
                    ProcessDocument(document as Camt052_008.Document);
                }
                else
                {
                    ProcessDocument(document as Camt052_002.Document);
                }
            }
            catch (Exception e)
            {
                throw new Exception("problem with file " + filename + "; " + e.Message + Environment.NewLine + e.StackTrace, e);
            }
        }

        private void ProcessDocument(Camt052_002.Document document)
        {
            statements = new List<CamtStatement>();

            var stmts = document.BkToCstmrAcctRpt.Rpt;

            foreach (Camt052_002.AccountReport11 accReport in stmts)
            {
                CamtStatement stmt = new CamtStatement();

                stmt.Id = accReport.Id;
                stmt.ElctrncSeqNb = accReport.ElctrncSeqNb.ToString();

                object accReportAccount = accReport.Acct?.Id?.Item;
                if (accReportAccount is string)
                    stmt.AccountCode = (string) accReportAccount;
                else if (accReportAccount is Camt052_002.GenericAccountIdentification1)
                    stmt.AccountCode = ((Camt052_002.GenericAccountIdentification1) accReportAccount).Id;

                stmt.BankCode = accReport.Acct?.Svcr?.FinInstnId?.BIC;
                stmt.Currency = accReport.Acct?.Ccy;

                stmt.SeveralYears = false;
                string nm = accReport.Acct?.Ownr?.Nm;
                string ownName = nm ?? "AccountNameFor" + stmt.BankCode + "/" + stmt.AccountCode;

                Camt052_002.CashBalance3[] balances = accReport.Bal;
                if (balances != null)
                {
                    foreach (Camt052_002.CashBalance3 balance in balances)
                    {
                        // PRCD: PreviouslyClosedBooked
                        if (balance.Tp?.CdOrPrtry?.Item?.ToString() == "PRCD")
                        {
                            stmt.StartBalance = balance.Amt.Value;

                            // CreditDebitIndicator: CRDT or DBIT for credit or debit
                            if (balance.CdtDbtInd == Camt052_002.CreditDebitCode.DBIT)
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
                            if (balance.CdtDbtInd == Camt052_002.CreditDebitCode.DBIT)
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

                Camt052_002.ReportEntry2[] entries = accReport.Ntry;
                if (entries != null)
                {
                    foreach (Camt052_002.ReportEntry2 entry in entries)
                    {
                        bool pending = entry.Sts == Camt052_002.EntryStatus2Code.PDNG;
                        if (!pending && entry.Amt.Ccy != stmt.Currency)
                        {
                            throw new Exception($"Transaction currency '{entry.Amt.Ccy}' does not match the bank statement currency '{stmt.Currency}'.");
                        }

                        CamtTransaction tr = new CamtTransaction();
                        tr.Pending = pending;

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

                        // Betrag/Soll/Haben
                        tr.Amount = entry.Amt.Value;
                        bool debit = entry.CdtDbtInd == Camt052_002.CreditDebitCode.DBIT;
                        if (debit)
                        {
                            tr.Amount *= -1.0m;
                        }

                        tr.Storno = entry.RvslInd;

                        Camt052_002.EntryDetails1 entryDetails = entry.NtryDtls?.FirstOrDefault();
                        Camt052_002.EntryTransaction2 txDetails = entryDetails?.TxDtls?.FirstOrDefault();

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
                            tr.AccountCode = (string) account;
                        else if (account is Camt052_002.GenericAccountIdentification1)
                            tr.AccountCode = ((Camt052_002.GenericAccountIdentification1) account).Id;

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
                        if (entryDetails.TxDtls?.Count() > 1)
                        {
                            tr.PartnerName = string.Empty;
                            tr.Description = string.Format("SEPA Sammel-Basislastschrift mit {0} Lastschriften", entryDetails.TxDtls?.Count());
                        }

                        stmt.Transactions.Add(tr);

                        Log.Write("count : " + stmt.Transactions.Count.ToString());
                    }
                }

                statements.Add(stmt);
            }
        }

        private void ProcessDocument(Camt052_008.Document document)
        {
            statements = new List<CamtStatement>();

            var stmts = document.BkToCstmrAcctRpt.Rpt;

            foreach (Camt052_008.AccountReport25 accReport in stmts)
            {
                CamtStatement stmt = new CamtStatement();

                stmt.Id = accReport.Id;
                stmt.ElctrncSeqNb = accReport.ElctrncSeqNb.ToString();

                object accReportAccount = accReport.Acct?.Id?.Item;
                if (accReportAccount is string)
                    stmt.AccountCode = (string) accReportAccount;
                else if (accReportAccount is Camt052_002.GenericAccountIdentification1)
                    stmt.AccountCode = ((Camt052_002.GenericAccountIdentification1) accReportAccount).Id;

                stmt.BankCode = accReport.Acct?.Svcr?.FinInstnId?.BICFI;
                stmt.Currency = accReport.Acct?.Ccy;

                stmt.SeveralYears = false;
                string nm = accReport.Acct?.Ownr?.Nm;
                string ownName = nm ?? "AccountNameFor" + stmt.BankCode + "/" + stmt.AccountCode;

                Camt052_008.CashBalance8[] balances = accReport.Bal;
                if (balances != null)
                {
                    foreach (Camt052_008.CashBalance8 balance in balances)
                    {
                        // PRCD: PreviouslyClosedBooked
                        if (balance.Tp?.CdOrPrtry?.Item?.ToString() == "PRCD")
                        {
                            stmt.StartBalance = balance.Amt.Value;

                            // CreditDebitIndicator: CRDT or DBIT for credit or debit
                            if (balance.CdtDbtInd == Camt052_008.CreditDebitCode.DBIT)
                            {
                                stmt.StartBalance *= -1.0m;
                            }

                            stmt.StartDate = balance.Dt.Item;
                        }
                        // OPBD: OpeningBooked
                        if (balance.Tp?.CdOrPrtry?.Item?.ToString() == "OPBD")
                        {
                            stmt.StartBalance = balance.Amt.Value;

                            // CreditDebitIndicator: CRDT or DBIT for credit or debit
                            if (balance.CdtDbtInd == Camt052_008.CreditDebitCode.DBIT)
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
                            if (balance.CdtDbtInd == Camt052_008.CreditDebitCode.DBIT)
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

                Camt052_008.ReportEntry10[] entries = accReport.Ntry;
                if (entries != null)
                {
                    foreach (Camt052_008.ReportEntry10 entry in entries)
                    {
                        bool pending = entry.Sts.Item == "PDNG";
                        if (!pending && entry.Amt.Ccy != stmt.Currency)
                        {
                            throw new Exception($"Transaction currency '{entry.Amt.Ccy}' does not match the bank statement currency '{stmt.Currency}'.");
                        }

                        CamtTransaction tr = new CamtTransaction();
                        tr.Pending = pending;

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

                        // Betrag/Soll/Haben
                        tr.Amount = entry.Amt.Value;
                        bool debit = entry.CdtDbtInd == Camt052_008.CreditDebitCode.DBIT;
                        if (debit)
                        {
                            tr.Amount *= -1.0m;
                        }

                        tr.Storno = entry.RvslInd;

                        Camt052_008.EntryDetails9 entryDetails = entry.NtryDtls?.FirstOrDefault();
                        Camt052_008.EntryTransaction10 txDetails = entryDetails?.TxDtls?.FirstOrDefault();

                        // Verwendungszweck
                        if (txDetails?.RmtInf?.Ustrd != null)
                        {
                            tr.Description = string.Join(string.Empty, txDetails.RmtInf.Ustrd.Select(s => s?.Trim()));
                        }

                        tr.Text = entry.AddtlNtryInf?.Trim();

                        tr.BankCode = debit ?
                            txDetails?.RltdAgts?.CdtrAgt?.FinInstnId?.BICFI :
                            txDetails?.RltdAgts?.DbtrAgt?.FinInstnId?.BICFI;

                        object partnerIdentification = debit ?
                            txDetails?.RltdPties?.Cdtr?.Item :
                            txDetails?.RltdPties?.Dbtr?.Item;
                        if (partnerIdentification is Camt052_008.PartyIdentification135)
                            tr.PartnerName = (partnerIdentification as Camt052_008.PartyIdentification135)?.Nm;

                        object account = debit ?
                            txDetails?.RltdPties?.CdtrAcct?.Id?.Item :
                            txDetails?.RltdPties?.DbtrAcct?.Id?.Item;
                        if (account is string)
                            tr.AccountCode = (string) account;
                        else if (account is Camt052_008.GenericAccountIdentification1)
                            tr.AccountCode = ((Camt052_008.GenericAccountIdentification1) account).Id;

                        var cdtrItem = txDetails?.RltdPties?.Cdtr?.Item;
                        string CrdtName = (cdtrItem as Camt052_008.PartyIdentification135)?.Nm;
                        var dbtrItem = txDetails?.RltdPties?.Dbtr?.Item;
                        string DbtrName = (dbtrItem as Camt052_008.PartyIdentification135)?.Nm;

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

                        if (txDetails?.Refs != null)
                        {
                            tr.EndToEndId = txDetails?.Refs.EndToEndId;
                            tr.MessageId = txDetails?.Refs.MsgId;
                            tr.PaymentInformationId = txDetails?.Refs.PmtInfId;
                            tr.MandateId = txDetails?.Refs.MndtId;
                            if (txDetails.Refs.Prtry != null && txDetails.Refs.Prtry.Length > 0)
                                tr.ProprietaryRef = txDetails?.Refs.Prtry[0].Ref;
                        }

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
                        if (entryDetails.TxDtls?.Count() > 1)
                        {
                            tr.PartnerName = string.Empty;
                            tr.Description = string.Format("SEPA Sammel-Basislastschrift mit {0} Lastschriften", entryDetails.TxDtls?.Count());
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
