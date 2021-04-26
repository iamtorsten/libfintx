/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2018 Bjoern Kuensting
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

namespace libfintx.Xml
{
    public static class XmlNames
    {
        public const string ebicsNoPubKeyDigestsRequest = "ebicsNoPubKeyDigestsRequest";
        public const string Version = "Version";
        public const string Revision = "Revision";
        public const string header = "header";
        public const string authenticate = "authenticate";
        public const string staticHeader = "static";
        public const string Nonce = "Nonce";
        public const string HostID = "HostID";
        public const string Timestamp = "Timestamp";
        public const string TimeStamp = "TimeStamp";
        public const string PartnerID = "PartnerID";
        public const string UserID = "UserID";
        public const string OrderDetails = "OrderDetails";
        public const string OrderType = "OrderType";
        public const string OrderAttribute = "OrderAttribute";
        public const string SecurityMedium = "SecurityMedium";
        public const string mutable = "mutable";
        public const string AuthSignature = "AuthSignature";
        public const string body = "body";
        public const string OrderID = "OrderID";
        public const string TimestampBankParameter = "TimestampBankParameter";
        public const string ReturnCode = "ReturnCode";
        public const string ReportText = "ReportText";
        public const string DataTransfer = "DataTransfer";
        public const string OrderData = "OrderData";
        public const string DataEncryptionInfo = "DataEncryptionInfo";
        public const string EncryptionPubKeyDigest = "EncryptionPubKeyDigest";
        public const string TransactionKey = "TransactionKey";
        public const string AuthenticationPubKeyInfo = "AuthenticationPubKeyInfo";
        public const string X509 = "X509";
        public const string PubKeyValue = "PubKeyValue";
        public const string EncryptionPubKeyInfo = "EncryptionPubKeyInfo";
        public const string AuthenticationVersion = "AuthenticationVersion";
        public const string RSAKeyValue = "RSAKeyValue";
        public const string Modulus = "Modulus";
        public const string Exponent = "Exponent";
        public const string EncryptionVersion = "EncryptionVersion";
        public const string ebicsRequest = "ebicsRequest";
        public const string StandardOrderParams = "StandardOrderParams";
        public const string DateRange = "DateRange";
        public const string Start = "Start";
        public const string End = "End";
        public const string BankPubKeyDigests = "BankPubKeyDigests";
        public const string Authentication = "Authentication";
        public const string Algorithm = "Algorithm";
        public const string Encryption = "Encryption";
        public const string SegmentNumber = "SegmentNumber";
        public const string lastSegment = "lastSegment";
        public const string SignatureData = "SignatureData";
        public const string TransactionPhase = "TransactionPhase";
        public const string TransactionID = "TransactionID";
        public const string NumSegments = "NumSegments";
        public const string Signature = "Signature";
        public const string Reference = "Reference";
        public const string TransferReceipt = "TransferReceipt";
        public const string ReceiptCode = "ReceiptCode";
        public const string SignedInfo = "SignedInfo";
        public const string CanonicalizationMethod = "CanonicalizationMethod";
        public const string SignatureMethod = "SignatureMethod";
        public const string URI = "URI";
        public const string Transforms = "Transforms";
        public const string Transform = "Transform";
        public const string DigestMethod = "DigestMethod";
        public const string DigestValue = "DigestValue";
        public const string SignatureValue = "SignatureValue";

        public const string CdtTrfTxInf = "CdtTrfTxInf";
        public const string PmtId = "PmtId";
        public const string Amt = "Amt";
        public const string InstdAmt = "InstdAmt";
        public const string Ccy = "Ccy";
        public const string RmtInf = "RmtInf";
        public const string Ustrd = "Ustrd";
        public const string EndToEndId = "EndToEndId";
        public const string Cdtr = "Cdtr";
        public const string Nm = "Nm";
        public const string CdtrAcct = "CdtrAcct";
        public const string Id = "Id";
        public const string IBAN = "IBAN";
        public const string CdtrAgt = "CdtrAgt";
        public const string FinInstnId = "FinInstnId";
        public const string BIC = "BIC";
        public const string PmtInf = "PmtInf";
        public const string PmtInfId = "PmtInfId";
        public const string PmtMtd = "PmtMtd";
        public const string BtchBookg = "BtchBookg";
        public const string NbOfTxs = "NbOfTxs";
        public const string CtrlSum = "CtrlSum";
        public const string PmtTpInf = "PmtTpInf";
        public const string SvcLvl = "SvcLvl";
        public const string Cd = "Cd";
        public const string ReqdExctnDt = "ReqdExctnDt";
        public const string Dbtr = "Dbtr";
        public const string DbtrAcct = "DbtrAcct";
        public const string DbtrAgt = "DbtrAgt";
        public const string ChrgBr = "ChrgBr";
        public const string Document = "Document";
        public const string CstmrCdtTrfInitn = "CstmrCdtTrfInitn";
        public const string GrpHdr = "GrpHdr";
        public const string MsgId = "MsgId";
        public const string CreDtTm = "CreDtTm";
        public const string InitgPty = "InitgPty";

        public const string UserSignatureData = "UserSignatureData";
        public const string OrderSignatureData = "OrderSignatureData";
        public const string SignatureVersion = "SignatureVersion";
        public const string DataDigest = "DataDigest";

        public const string ebicsUnsecuredRequest = "ebicsUnsecuredRequest";
        public const string SignaturePubKeyOrderData = "SignaturePubKeyOrderData";
        public const string SignaturePubKeyInfo = "SignaturePubKeyInfo";

        public const string HIARequestOrderData = "HIARequestOrderData";

        public const string DrctDbtTxInf = "DrctDbtTxInf";
        public const string DrctDbtTx = "DrctDbtTx";
        public const string MndtRltdInf = "MndtRltdInf";
        public const string MndtId = "MndtId";
        public const string DtOfSgntr = "DtOfSgntr";
        public const string CstmrDrctDbtInitn = "CstmrDrctDbtInitn";
        public const string LclInstrm = "LclInstrm";
        public const string SeqTp = "SeqTp";
        public const string CdtrSchmeId = "CdtrSchmeId";
        public const string PrvtId = "PrvtId";
        public const string Othr = "Othr";
        public const string SchmeNm = "SchmeNm";
        public const string Prtry = "Prtry";
        public const string ReqdColltnDt = "ReqdColltnDt";
        public const string AmdmntInd = "AmdmntInd";
    }
}
