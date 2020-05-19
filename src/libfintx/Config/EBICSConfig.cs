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
    public class EbicsConfig
    {
        private static readonly Stateprinter _printer;
        
        public string Address { get; set; }
        public bool TLS { get; set; }
        public bool Insecure { get; set; }
        public UserParams User { get; set; }
        public BankParams Bank { get; set; }
        public EbicsVersion Version { get; set; } = EbicsVersion.H004;
        public EbicsRevision Revision { get; set; } = EbicsRevision.Rev1;


        static EbicsConfig()
        {
            _printer = new Stateprinter();
            _printer.Configuration.SetNewlineDefinition("");
            _printer.Configuration.SetIndentIncrement(" ");
            _printer.Configuration.SetOutputFormatter(new JsonStyle(_printer.Configuration));
        }

        public override string ToString() => _printer.PrintObject(this);
    }
}