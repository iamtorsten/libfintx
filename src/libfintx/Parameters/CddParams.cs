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
    public class CddParams : Params
    {
        public IEnumerable<DirectDebitPaymentInfo> PaymentInfos { get; set; }
        public string InitiatingParty { get; set; }        
    }
}