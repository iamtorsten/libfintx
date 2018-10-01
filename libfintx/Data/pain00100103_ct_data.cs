using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace libfintx
{
    public class pain00100103_ct_data
    {
        public string Initiator { get; set; }
        public int NumberOfTransactions { get; set; }
        public decimal? ControlSum { get; set; }
        public List<PaymentInfo> Payments { get; set; }
        public class PaymentInfo
        {
            public DateTime RequestedExecutionDate { get; set; }
            public string Debtor { get; set; }
            /// <summary>
            /// IBAN
            /// </summary>
            public string DebtorAccount { get; set; }
            /// <summary>
            /// BIC
            /// </summary>
            public string DebtorAgent { get; set; }
            public List<CreditTransferTransactionInfo> CreditTxInfos { get; set; }
        }
        public class CreditTransferTransactionInfo
        {
            public decimal Amount { get; set; }
            public string Creditor { get; set; }
            /// <summary>
            /// BIC
            /// </summary>
            public string CreditorAgent { get; set; }
            /// <summary>
            /// IBAN
            /// </summary>
            public string CreditorAccount { get; set; }
            /// <summary>
            /// Verwendungszweck
            /// </summary>
            public string RemittanceInformation { get; set; }
        }
        public static pain00100103_ct_data Create(string xml)
        {
            XmlSerializer ser = new XmlSerializer(typeof(pain_001_001_03.Document), new XmlRootAttribute
            {
                ElementName = "Document",
                Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.001.001.03",
            });
            using (TextReader reader = new StringReader(xml))
            {
                return Create((pain_001_001_03.Document)ser.Deserialize(reader));
            }
        }
        public static pain00100103_ct_data Create(libfintx.pain_001_001_03.Document xml)
        {
            var result = new pain00100103_ct_data();
            result.Initiator = xml.CstmrCdtTrfInitn?.GrpHdr?.InitgPty?.Nm;
            result.NumberOfTransactions = Convert.ToInt32(xml.CstmrCdtTrfInitn?.GrpHdr?.NbOfTxs);
            result.ControlSum = xml.CstmrCdtTrfInitn?.GrpHdr?.CtrlSum;
            foreach (var pmtInf in xml.CstmrCdtTrfInitn.PmtInf)
            {
                if (result.Payments == null)
                    result.Payments = new List<PaymentInfo>();
                var paymentInfo = new PaymentInfo();
                paymentInfo.RequestedExecutionDate = pmtInf.ReqdExctnDt;
                paymentInfo.Debtor = pmtInf.Dbtr?.Nm;
                paymentInfo.DebtorAccount = pmtInf.DbtrAcct?.Id?.Item?.ToString();
                paymentInfo.DebtorAgent = pmtInf.DbtrAgt?.FinInstnId?.BIC;
                result.Payments.Add(paymentInfo);
                foreach (var cdtTrfTxInf in pmtInf.CdtTrfTxInf)
                {
                    if (paymentInfo.CreditTxInfos == null)
                        paymentInfo.CreditTxInfos = new List<CreditTransferTransactionInfo>();
                    var creditTxInfo = new CreditTransferTransactionInfo();
                    creditTxInfo.Amount = ((libfintx.pain_001_001_03.ActiveOrHistoricCurrencyAndAmount)cdtTrfTxInf.Amt?.Item).Value;
                    creditTxInfo.Creditor = cdtTrfTxInf.Cdtr?.Nm;
                    creditTxInfo.CreditorAccount = cdtTrfTxInf.CdtrAcct?.Id?.Item?.ToString();
                    creditTxInfo.CreditorAgent = cdtTrfTxInf.CdtrAgt?.FinInstnId?.BIC;
                    if (cdtTrfTxInf.RmtInf?.Ustrd != null)
                        creditTxInfo.RemittanceInformation = string.Join(", ", cdtTrfTxInf.RmtInf?.Ustrd);
                    paymentInfo.CreditTxInfos.Add(creditTxInfo);
                }
            }
            return result;
        }
    }
}