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
using System.Security;
using libfintx.Globals;

namespace libfintx.Sepa
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
            DateTime datetime = DateTime.Now;
            var creDtTm = $"{datetime:s}";
            var msgId = $"{datetime:yyyy-MM-dd HH:mm:ss.fff}";
            var pmtInfId = msgId;

            var Amount_ = Amount.ToString().Replace(",", ".");

            var Accountholder_ = SecurityElement.Escape(Accountholder);
            var Receiver_ = SecurityElement.Escape(Receiver);
            var Usage_ = SecurityElement.Escape(Usage);

            string Message = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<Document xmlns=\"urn:iso:std:iso:20022:tech:xsd:pain.001.002.03\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"urn:iso:std:iso:20022:tech:xsd:pain.001.002.03 pain.001.002.03.xsd\">" +
                    "<CstmrCdtTrfInitn>" +
                        "<GrpHdr>" +
                        "<MsgId>" + msgId + "</MsgId>" +
                        "<CreDtTm>" + creDtTm + "</CreDtTm>" +
                        "<NbOfTxs>1</NbOfTxs>" +
                        "<CtrlSum>" + Amount_ + "</CtrlSum>" +
                        "<InitgPty>" +
                        "<Nm>" + Accountholder_ + "</Nm>" +
                        "</InitgPty>" +
                        "</GrpHdr>" +
                            "<PmtInf>" +
                            "<PmtInfId>" + pmtInfId + "</PmtInfId>" +
                            "<PmtMtd>TRF</PmtMtd>" +
                            "<NbOfTxs>1</NbOfTxs>" +
                            "<CtrlSum>" + Amount_ + "</CtrlSum>" +
                            "<PmtTpInf>" +
                            "<SvcLvl>" +
                            "<Cd>SEPA</Cd>" +
                            "</SvcLvl>" +
                            "</PmtTpInf>" +
                            "<ReqdExctnDt>" + $"{ExecutionDay:yyyy-MM-dd}" + "</ReqdExctnDt>" +
                            "<Dbtr>" +
                                "<Nm>" + Accountholder_ + "</Nm>" +
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
                                    "<Nm>" + Receiver_ + "</Nm>" +
                                "</Cdtr>" +
                                "<CdtrAcct>" +
                                    "<Id>" +
                                        "<IBAN>" + ReceiverIBAN + "</IBAN>" +
                                    "</Id>" + "</CdtrAcct>" +
                                "<RmtInf>" +
                                    "<Ustrd>" + Usage_ + "</Ustrd>" +
                                "</RmtInf>" +
                            "</CdtTrfTxInf>" +
                        "</PmtInf>" +
                    "</CstmrCdtTrfInitn>" +
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
        public static string Create(string Accountholder, string AccountholderIBAN, string AccountholderBIC, List<Pain00100203CtData> PainData, string NumberofTransactions, decimal TotalAmount, DateTime ExecutionDay)
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
                 /*-1-*/ "<CstmrCdtTrfInitn>" +
                 /*--2--*/ "<GrpHdr>" +
                 /*---3---*/ "<MsgId>" + FinTsGlobals.Buildname + "-" + RndNr.ToString().Replace("-", "") + "</MsgId>" +
                 /*---3---*/ "<CreDtTm>" + datetime_ + "</CreDtTm>" +
                 /*---3---*/ "<NbOfTxs>" + NumberofTransactions + "</NbOfTxs>" +
                 /*---3---*/ "<CtrlSum>" + Amount_ + "</CtrlSum>" +
                 /*---3---*/ "<InitgPty>" +
                 /*----4----*/ "<Nm>" + Accountholder + "</Nm>" +
                 /*---3---*/ "</InitgPty>" +
                 /*--2--*/ "</GrpHdr>" +
                 /*--2--*/"<PmtInf>" +
                 /*---3---*/ "<PmtInfId>" + FinTsGlobals.Buildname + "-" + RndNr_.ToString().Replace("-", "") + "</PmtInfId>" +
                 /*---3---*/ "<PmtMtd>TRF</PmtMtd>" +
                 /*---3---*/ "<NbOfTxs>" + NumberofTransactions + "</NbOfTxs>" +
                 /*---3---*/ "<PmtTpInf>" +
                 /*----4----*/ "<SvcLvl>" +
                 /*-----5-----*/ "<Cd>SEPA</Cd>" +
                 /*----4----*/ "</SvcLvl>" +
                 /*---3---*/ "</PmtTpInf>" +
                 /*---3---*/ "<ReqdExctnDt>" + ExecutionDay.ToString("yyyy-MM-dd") + "</ReqdExctnDt>" +
                 /*---3---*/ "<Dbtr>" +
                 /*----4----*/ "<Nm>" + Accountholder + "</Nm>" +
                 /*---3---*/ "</Dbtr>" +
                 /*---3---*/ "<DbtrAcct>" +
                 /*----4----*/ "<Id>" +
                 /*-----5-----*/ "<IBAN>" + AccountholderIBAN + "</IBAN>" +
                 /*----4----*/ "</Id>" +
                 /*---3---*/ "</DbtrAcct>" +
                 /*---3---*/ "<DbtrAgt>" +
                 /*----4----*/ "<FinInstnId>" +
                 /*-----5-----*/ "<BIC>" + AccountholderBIC + "</BIC>" +
                 /*----4----*/ "</FinInstnId>" +
                 /*---3---*/ "</DbtrAgt>" +
                 /*---3---*/ "<ChrgBr>SLEV</ChrgBr>";

            foreach (var transaction in PainData)
            {
                var Amount__ = transaction.Amount.ToString().Replace(",", ".");

                string Message_ =
                    /**/ "<CdtTrfTxInf>" +
                    /*-1-*/ "<PmtId>" +
                    /*--2--*/ "<EndToEndId>NOTPROVIDED</EndToEndId>" +
                    /*-1-*/ "</PmtId>" +
                    /*-1-*/ "<Amt>" +
                    /*--2--*/ "<InstdAmt Ccy=\"EUR\">" + Amount__ + "</InstdAmt>" +
                    /*-1-*/ "</Amt>" +
                    /*-1-*/ "<CdtrAgt>" +
                    /*--2--*/ "<FinInstnId>" +
                    /*---3---*/ "<BIC>" + transaction.ReceiverBic + "</BIC>" +
                    /*--2--*/ "</FinInstnId>" +
                    /*-1-*/ "</CdtrAgt>" +
                    /*-1-*/ "<Cdtr>" +
                    /*--2--*/ "<Nm>" + transaction.Receiver + "</Nm>" +
                    /*-1-*/ "</Cdtr>" +
                    /*-1-*/ "<CdtrAcct>" +
                    /*--2--*/ "<Id>" +
                    /*---3---*/ "<IBAN>" + transaction.ReceiverIban + "</IBAN>" +
                    /*--2--*/ "</Id>" +
                    /*-1-*/ "</CdtrAcct>" +
                    /*-1-*/ "<RmtInf>" +
                    /*--2--*/ "<Ustrd>" + transaction.Usage + "</Ustrd>" +
                    /*-1-*/ "</RmtInf>" +
                    /**/ "</CdtTrfTxInf>";

                Message = Message + Message_;
            }
            Message = Message + "</PmtInf></CstmrCdtTrfInitn></Document>'";

            return Message;
        }
    }
}
