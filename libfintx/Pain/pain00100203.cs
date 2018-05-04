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
    public static class pain00100203
    {
        /// <summary>
        /// Create pain version 00100203
        /// </summary>
        /// <param name="Accountholder"></param>
        /// <param name="AccountholderIBAN"></param>
        /// <param name="AccountholderBIC"></param>
        /// <param name="Receiver"></param>
        /// <param name="ReceiverIBAN"></param>
        /// <param name="ReceiverBIC"></param>
        /// <param name="Amount"></param>
        /// <param name="Usage"></param>
        /// <param name="ExecutionDay"></param>
        /// <returns></returns>
        public static string Create(string Accountholder, string AccountholderIBAN, string AccountholderBIC, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime ExecutionDay)
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
                "<Document xmlns=\"urn:iso:std:iso:20022:tech:xsd:pain.001.002.03\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"urn:iso:std:iso:20022:tech:xsd:pain.001.002.03 pain.001.002.03.xsd\">" +
                "<CstmrCdtTrfInitn>" + 
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
                "<PmtMtd>TRF</PmtMtd>" + 
                "<NbOfTxs>1</NbOfTxs>" + 
                "<CtrlSum>" + Amount_ + "</CtrlSum>" +
                "<PmtTpInf>" + 
                "<SvcLvl>" + 
                "<Cd>SEPA</Cd>" + 
                "</SvcLvl>" + 
                "</PmtTpInf>" + 
                "<ReqdExctnDt>" + ExecutionDay.ToString("yyyy-MM-dd") + "</ReqdExctnDt>" + 
                "<Dbtr>" +
                "<Nm>" + Accountholder + "</Nm>" + 
                "</Dbtr>" +
                "<DbtrAcct>" +
                "<Id>" +
                "<IBAN>" + AccountholderIBAN + "</IBAN>" + 
                "</Id>" + 
                "</DbtrAcct>" +
                "<DbtrAgt>" +
                "<FinInstnId>" +
                "<BIC>" + AccountholderBIC + "</BIC>" + 
                "</FinInstnId>" +
                "</DbtrAgt>" + 
                "<ChrgBr>SLEV</ChrgBr>" + 
                "<CdtTrfTxInf>" + 
                "<PmtId>" + 
                "<EndToEndId>NOTPROVIDED</EndToEndId>" + 
                "</PmtId>" +
                "<Amt>" +
                "<InstdAmt Ccy=\"EUR\">" + Amount_ + "</InstdAmt>" + 
                "</Amt>" +
                "<CdtrAgt>" +
                "<FinInstnId>" +
                "<BIC>" + ReceiverBIC + "</BIC>" + 
                "</FinInstnId>" +
                "</CdtrAgt>" +
                "<Cdtr>" +
                "<Nm>" + Receiver + "</Nm>" + 
                "</Cdtr>" + 
                "<CdtrAcct>" +
                "<Id>" + 
                "<IBAN>" + ReceiverIBAN + "</IBAN>" + 
                "</Id>" + "</CdtrAcct>" +
                "<RmtInf>" +
                "<Ustrd>" + Usage + "</Ustrd>" +
                "</RmtInf>" +
                "</CdtTrfTxInf>" +
                "</PmtInf>" +
                "</CstmrCdtTrfInitn>"+
                "</Document>" +
                "'";

            return Message;
        }

        /// <summary>
        /// Create pain version 00100203
        /// Collective -> approximately 1.000 payments in the order are possible -> This depends on the bank
        /// </summary>
        /// <param name="Accountholder"></param>
        /// <param name="AccountholderIBAN"></param>
        /// <param name="AccountholderBIC"></param>
        /// <param name="PainData"></param>
        /// <param name="NumberofTransactions"></param>
        /// <param name="TotalAmount"></param>
        /// <param name="ExecutionDay"></param>
        /// <returns></returns>
        public static string Create(string Accountholder, string AccountholderIBAN, string AccountholderBIC, List<pain00100203_ct_data> PainData, string NumberofTransactions, decimal TotalAmount, DateTime ExecutionDay)
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
                "<Document xmlns=\"urn:iso:std:iso:20022:tech:xsd:pain.001.002.03\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"urn:iso:std:iso:20022:tech:xsd:pain.001.002.03 pain.001.002.03.xsd\">" +
                "<CstmrCdtTrfInitn>" +
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
                "<PmtMtd>TRF</PmtMtd>" +
                "<Cd>SEPA</Cd>" +
                "</SvcLvl>" +
                "</PmtTpInf>" +
                "<ReqdExctnDt>" + ExecutionDay.ToString("yyyy-MM-dd") + "</ReqdExctnDt>" +
                "<Dbtr>" +
                "<Nm>" + Accountholder + "</Nm>" +
                "</Dbtr>" +
                "<DbtrAcct>" +
                "<Id>" +
                "<IBAN>" + AccountholderIBAN + "</IBAN>" +
                "</Id>" +
                "</DbtrAcct>" +
                "<DbtrAgt>" +
                "<FinInstnId>" +
                "<BIC>" + AccountholderBIC + "</BIC>" +
                "</FinInstnId>" +
                "</DbtrAgt>" +
                "<ChrgBr>SLEV</ChrgBr>" +
                "<CdtTrfTxInf>";
                
                foreach (var transaction in PainData)
                {
                    var Amount__ = transaction.Amount.ToString().Replace(",", ".");

                    string Message_ = "<PmtId>" +
                        "<EndToEndId>NOTPROVIDED</EndToEndId>" +
                        "</PmtId>" +
                        "<Amt>" +
                        "<InstdAmt Ccy=\"EUR\">" + Amount__ + "</InstdAmt>" +
                        "</Amt>" +
                        "<CdtrAgt>" +
                        "<FinInstnId>" +
                        "<BIC>" + transaction.ReceiverBIC + "</BIC>" +
                        "</FinInstnId>" +
                        "</CdtrAgt>" +
                        "<Cdtr>" +
                        "<Nm>" + transaction.Receiver + "</Nm>" +
                        "</Cdtr>" +
                        "<CdtrAcct>" +
                        "<Id>" +
                        "<IBAN>" + transaction.ReceiverIBAN + "</IBAN>" +
                        "</Id>" + "</CdtrAcct>" +
                        "<RmtInf>" +
                        "<Ustrd>" + transaction.Usage + "</Ustrd>" +
                        "</RmtInf>" +
                        "</CdtTrfTxInf>" +
                        "</PmtInf>";

                    Message = Message + Message_;
                }

                string Message__ = "</CstmrCdtTrfInitn>" +
                    "</Document>" +
                    "'";

                Message = Message + Message__;

            return Message;
        }
    }
}
