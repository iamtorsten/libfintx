/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using StatePrinting;
using StatePrinting.OutputFormatters;

namespace libfintx.Responses
{
    public class Response
    {
        static readonly Stateprinter _printer;

        public int TechnicalReturnCode { get; set; }
        public int BusinessReturnCode { get; set; }
        public string ReportText { get; set; }

        static Response()
        {
            _printer = new Stateprinter();
            //_printer.Configuration.SetNewlineDefinition("");
            _printer.Configuration.SetIndentIncrement(" ");
            _printer.Configuration.SetOutputFormatter(new JsonStyle(_printer.Configuration));
        }

        public override string ToString() => _printer.PrintObject(this);
    }
}