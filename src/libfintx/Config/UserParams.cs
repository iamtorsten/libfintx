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
    public class UserParams
    {
        private static readonly Stateprinter _printer;
        
        public string HostId { get; set; }
        public string UserId { get; set; }
        public string PartnerId { get; set; }
        public string SystemId { get; set; }

        public SignKeyPair SignKeys { get; set; }
        public CryptKeyPair CryptKeys { get; set; }
        public AuthKeyPair AuthKeys { get; set; }

        static UserParams()
        {
            _printer = new Stateprinter();
            _printer.Configuration.SetNewlineDefinition("");
            _printer.Configuration.SetIndentIncrement(" ");
            _printer.Configuration.SetOutputFormatter(new JsonStyle(_printer.Configuration));
        }

        public override string ToString() => _printer.PrintObject(this);
    }
}