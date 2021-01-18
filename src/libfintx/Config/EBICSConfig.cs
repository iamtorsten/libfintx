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
    public class EbicsConfig
    {
        
        
        public string Address { get; set; }
        public bool TLS { get; set; }
        public bool Insecure { get; set; }
        public UserParams User { get; set; }
        public BankParams Bank { get; set; }
        public EbicsVersion Version { get; set; } = EbicsVersion.H004;
        public EbicsRevision Revision { get; set; } = EbicsRevision.Rev1;


        static EbicsConfig()
        {

        }

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
