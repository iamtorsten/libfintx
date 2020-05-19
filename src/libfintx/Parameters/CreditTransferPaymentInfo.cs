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
    public class CreditTransferPaymentInfo
    {
        public string DebtorName { get; set; }
        public string DebtorAccount { get; set; }
        public string DebtorAgent { get; set; }
        public string ExecutionDate { get; set; }
        public bool BatchBooking { get; set; }
        public IEnumerable<CreditTransferTransactionInfo> CreditTransferTransactionInfos { get; set; }
    }
}