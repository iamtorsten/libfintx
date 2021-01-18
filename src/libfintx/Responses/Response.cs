/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using Newtonsoft.Json;

namespace libfintx.Responses
{
    public class Response
    {
        public int TechnicalReturnCode { get; set; }
        public int BusinessReturnCode { get; set; }
        public string ReportText { get; set; }

        static Response()
        {

        }

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
