/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using Newtonsoft.Json;

namespace libfintx.Config
{
    internal class NamespaceConfig
    {
        
        
        internal string Ebics { get; set; }

        internal string EbicsPrefix => "urn";

        internal string XmlDsig { get; set; }

        internal string XmlDsigPrefix => "ds";

        internal string Cct { get; set; }
        internal string Cdd { get; set; }

        internal string SignatureData { get; set; }
        
        static NamespaceConfig()
        {

        }

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
