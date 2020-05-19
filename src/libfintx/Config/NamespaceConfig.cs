/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using StatePrinting;
using StatePrinting.OutputFormatters;

namespace libfintx.Config
{
    internal class NamespaceConfig
    {
        private static readonly Stateprinter _printer;
        
        internal string Ebics { get; set; }

        internal string EbicsPrefix => "urn";

        internal string XmlDsig { get; set; }

        internal string XmlDsigPrefix => "ds";

        internal string Cct { get; set; }
        internal string Cdd { get; set; }

        internal string SignatureData { get; set; }
        
        static NamespaceConfig()
        {
            _printer = new Stateprinter();
            _printer.Configuration.SetNewlineDefinition("");
            _printer.Configuration.SetIndentIncrement(" ");
            _printer.Configuration.SetOutputFormatter(new JsonStyle(_printer.Configuration));
        }

        public override string ToString() => _printer.PrintObject(this);
    }
}