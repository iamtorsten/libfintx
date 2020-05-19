/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System.Collections.Generic;

namespace libfintx.Parameters
{
    public class DirectDebitPaymentInfo
    {
        public string CreditorName { get; set; }
        public string CreditorAccount { get; set; }
        public string CreditorAgent { get; set; }
        public string CollectionDate { get; set; }
        public bool BatchBooking { get; set; }
        public string CreditorId { get; set; }
        public IEnumerable<DirectDebitTransactionInfo> DirectDebitTransactionInfos { get; set; }
    }
}