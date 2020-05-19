/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System;
using libfintx.Config;

namespace libfintx.Responses
{
    public class HpbResponse : Response
    {
        public string OrderId { get; internal set; }
        public DateTime TimestampBankParameter { get; internal set; }
        public BankParams Bank { get; internal set; }
    }
}