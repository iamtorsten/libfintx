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
using System.Collections.Generic;

namespace libfintx
{
    public static class pain00800202
    {
        /// <summary>
        /// Create pain version 00800202
        /// </summary>
        /// <param name="Accountholder"></param>
        /// <param name="AccountholderIBAN"></param>
        /// <param name="AccountholderBIC"></param>
        /// <param name="Payer"></param>
        /// <param name="PayerIBAN"></param>
        /// <param name="PayerBIC"></param>
        /// <param name="Amount"></param>
        /// <param name="Usage"></param>
        /// <param name="SettlementDate"></param>
        /// <param name="MandateNumber"></param>
        /// <param name="MandateDate"></param>
        /// <param name="CeditorIDNumber"></param>
        /// <returns></returns>
        public static string Create(string Accountholder, string AccountholderIBAN, string AccountholderBIC, string Payer, string PayerIBAN, string PayerBIC, decimal Amount, string Usage, DateTime SettlementDate, string MandateNumber, DateTime MandateDate, string CeditorIDNumber)
        {
            var RndNr = Guid.NewGuid().ToString();

            if (RndNr.Length > 20)
                RndNr = RndNr.Substring(0, 20);

            var RndNr_ = Guid.NewGuid().ToString();

            if (RndNr_.Length > 20)
                RndNr_ = RndNr_.Substring(0, 20);

            DateTime datetime = DateTime.Now;
            var datetime_ = string.Format("{0:s}", datetime);

            var Amount_ = Amount.ToString().Replace(",", ".");

            string Message = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<Document xmlns=\"urn:iso:std:iso:20022:tech:xsd:pain.008.002.02\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"urn:iso:std:iso:20022:tech:xsd:pain.008.002.02 pain.008.002.02.xsd\">" +
				"<CstmrDrctDbtInitn>" +
				"<GrpHdr>" +
				"<MsgId>" + Program.Buildname + "-" + RndNr.ToString().Replace("-", "") + "</MsgId>" +
				"<CreDtTm>" + datetime_ + "</CreDtTm>" +
				"<NbOfTxs>1</NbOfTxs>" +
				"<CtrlSum>" + Amount_ + "</CtrlSum>" +
				"<InitgPty>" +
				"<Nm>" + Accountholder + "</Nm>" +
				"</InitgPty>" +
				"</GrpHdr>" +
				"<PmtInf>" +
                "<PmtInfId>" + Program.Buildname + "-" + RndNr_.ToString().Replace("-", "") + "</PmtInfId>" +
				"<PmtMtd>DD</PmtMtd>" +
				"<NbOfTxs>1</NbOfTxs>" +
				"<CtrlSum>" + Amount_ + "</CtrlSum>" +
				"<PmtTpInf>" +
				"<SvcLvl>" +
				"<Cd>SEPA</Cd>" +
				"</SvcLvl>" +
				"<LclInstrm>" +
				"<Cd>CORE</Cd>" +
				"</LclInstrm>" +
				"<SeqTp>OOFF</SeqTp>" +
				"</PmtTpInf>" +
				"<ReqdColltnDt>" + SettlementDate.ToString("yyyy-MM-dd") + "</ReqdColltnDt>" +
				"<Cdtr>" +
				"<Nm>" + Accountholder + "</Nm>" +
				"</Cdtr>" +
				"<CdtrAcct>" +
				"<Id>" +
				"<IBAN>" + AccountholderIBAN + "</IBAN>" +
				"</Id>" +
				"</CdtrAcct>" +
				"<CdtrAgt>" +
				"<FinInstnId>" +
				"<BIC>" + AccountholderBIC + "</BIC>" +
				"</FinInstnId>" +
				"</CdtrAgt>" +
				"<ChrgBr>SLEV</ChrgBr>" +
				"<DrctDbtTxInf>" +
				"<PmtId>" +
				"<EndToEndId>NOTPROVIDED</EndToEndId>" +
				"</PmtId>" +
				"<InstdAmt Ccy=\"EUR\">" + Amount_ + "</InstdAmt>" +
				"<DrctDbtTx>" +
				"<MndtRltdInf>" +
				"<MndtId>" + MandateNumber + "</MndtId>" +
				"<DtOfSgntr>" + MandateDate.ToString("yyyy-MM-dd") + "</DtOfSgntr>" +
				"</MndtRltdInf>" +
				"<CdtrSchmeId>" +
				"<Id>" +
				"<PrvtId>" +
				"<Othr>" +
				"<Id>" + CeditorIDNumber + "</Id>" +
				"<SchmeNm>" +
				"<Prtry>SEPA</Prtry>" +
				"</SchmeNm>" +
				"</Othr>" +
				"</PrvtId>" +
				"</Id>" +
				"</CdtrSchmeId>" +
				"</DrctDbtTx>" +
				"<DbtrAgt>" +
				"<FinInstnId>" +
				"<BIC>" + PayerBIC + "</BIC>" +
				"</FinInstnId>" +
				"</DbtrAgt>" +
				"<Dbtr>" +
				"<Nm>" + Payer + "</Nm>" +
				"</Dbtr>" + "<DbtrAcct>" +
				"<Id>" +
				"<IBAN>" + PayerIBAN + "</IBAN>" +
				"</Id>" +
				"</DbtrAcct>" +
				"<RmtInf>" +
				"<Ustrd>" + Usage + "</Ustrd>" +
				"</RmtInf>" +
				"</DrctDbtTxInf>" +
				"</PmtInf>" +
				"</CstmrDrctDbtInitn>" +
				"</Document>" +
				"'";

            return Message;
        }

        /// <summary>
        /// Create pain version 00800202
        /// Collective -> approximately 1.000 payments in the order are possible -> This depends on the bank
        /// </summary>
        /// <param name="Accountholder"></param>
        /// <param name="AccountholderIBAN"></param>
        /// <param name="AccountholderBIC"></param>
        /// <param name="SettlementDate"></param>
        /// <param name="PainData"></param>
        /// <param name="NumberofTransactions"></param>
        /// <param name="TotalAmount"></param>
        /// <returns></returns>
        public static string Create(string Accountholder, string AccountholderIBAN, string AccountholderBIC, DateTime SettlementDate, List<pain00800202_cc_data> PainData, string NumberofTransactions, decimal TotalAmount)
        {
            var RndNr = Guid.NewGuid().ToString();

            if (RndNr.Length > 20)
                RndNr = RndNr.Substring(0, 20);

            var RndNr_ = Guid.NewGuid().ToString();

            if (RndNr_.Length > 20)
                RndNr_ = RndNr_.Substring(0, 20);

            DateTime datetime = DateTime.Now;
            var datetime_ = string.Format("{0:s}", datetime);

            var Amount_ = TotalAmount.ToString().Replace(",", ".");

            string Message = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<Document xmlns=\"urn:iso:std:iso:20022:tech:xsd:pain.008.002.02\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"urn:iso:std:iso:20022:tech:xsd:pain.008.002.02 pain.008.002.02.xsd\">" +
                "<CstmrDrctDbtInitn>" +
                "<GrpHdr>" +
                "<MsgId>" + Program.Buildname + "-" + RndNr.ToString().Replace("-", "") + "</MsgId>" +
                "<CreDtTm>" + datetime_ + "</CreDtTm>" +
                "<NbOfTxs>" + NumberofTransactions + "</NbOfTxs>" +
                "<CtrlSum>" + Amount_ + "</CtrlSum>" +
                "<InitgPty>" +
                "<Nm>" + Accountholder + "</Nm>" +
                "</InitgPty>" +
                "</GrpHdr>" +
                "<PmtInf>" +
                "<PmtInfId>" + Program.Buildname + "-" + RndNr_.ToString().Replace("-", "") + "</PmtInfId>" +
                "<PmtMtd>DD</PmtMtd>" +
                "<PmtTpInf>" +
                "<SvcLvl>" +
                "<Cd>SEPA</Cd>" +
                "</SvcLvl>" +
                "<LclInstrm>" +
                "<Cd>CORE</Cd>" +
                "</LclInstrm>" +
                "<SeqTp>OOFF</SeqTp>" +
                "</PmtTpInf>" +
                "<ReqdColltnDt>" + SettlementDate.ToString("yyyy-MM-dd") + "</ReqdColltnDt>" +
                "<Cdtr>" +
                "<Nm>" + Accountholder + "</Nm>" +
                "</Cdtr>" +
                "<CdtrAcct>" +
                "<Id>" +
                "<IBAN>" + AccountholderIBAN + "</IBAN>" +
                "</Id>" +
                "</CdtrAcct>" +
                "<CdtrAgt>" +
                "<FinInstnId>" +
                "<BIC>" + AccountholderBIC + "</BIC>" +
                "</FinInstnId>" +
                "</CdtrAgt>" +
                "<ChrgBr>SLEV</ChrgBr>" +
                "<DrctDbtTxInf>";

                foreach (var transaction in PainData)
                {
                    var Amount__ = transaction.Amount.ToString().Replace(",", ".");

                    string Message_ = "<PmtId>" +
                        "<EndToEndId>NOTPROVIDED</EndToEndId>" +
                        "</PmtId>" +
                        "<InstdAmt Ccy=\"EUR\">" + Amount__ + "</InstdAmt>" +
                        "<DrctDbtTx>" +
                        "<MndtRltdInf>" +
                        "<MndtId>" + transaction.MandateNumber + "</MndtId>" +
                        "<DtOfSgntr>" + transaction.MandateDate + "</DtOfSgntr>" +
                        "</MndtRltdInf>" +
                        "<CdtrSchmeId>" +
                        "<Id>" +
                        "<PrvtId>" +
                        "<Othr>" +
                        "<Id>" + transaction.CeditorIDNumber + "</Id>" +
                        "<SchmeNm>" +
                        "<Prtry>SEPA</Prtry>" +
                        "</SchmeNm>" +
                        "</Othr>" +
                        "</PrvtId>" +
                        "</Id>" +
                        "</CdtrSchmeId>" +
                        "</DrctDbtTx>" +
                        "<DbtrAgt>" +
                        "<FinInstnId>" +
                        "<BIC>" + transaction.PayerBIC + "</BIC>" +
                        "</FinInstnId>" +
                        "</DbtrAgt>" +
                        "<Dbtr>" +
                        "<Nm>" + transaction.Payer + "</Nm>" +
                        "</Dbtr>" + "<DbtrAcct>" +
                        "<Id>" +
                        "<IBAN>" + transaction.PayerIBAN + "</IBAN>" +
                        "</Id>" +
                        "</DbtrAcct>" +
                        "<RmtInf>" +
                        "<Ustrd>" + transaction.Usage + "</Ustrd>" +
                        "</RmtInf>" +
                        "</DrctDbtTxInf>" +
                        "</PmtInf>";

                    Message = Message + Message_;
                }

                string Message__ = "</CstmrDrctDbtInitn>" +
                        "</Document>" +
                        "'";

                Message = Message + Message__;

            return Message;
        }
    }
}
