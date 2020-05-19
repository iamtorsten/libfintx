/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

namespace libfintx.Parameters
{
    public class CreditTransferTransactionInfo
    {
        public string CreditorName { get; set; }
        public string CreditorAccount { get; set; }
        public string CreditorAgent { get; set; }
        public string Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string RemittanceInfo { get; set; }
        public string EndToEndId { get; set; }
    }
}