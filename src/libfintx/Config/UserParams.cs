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
    public class UserParams
    {
        
        
        public string HostId { get; set; }
        public string UserId { get; set; }
        public string PartnerId { get; set; }
        public string SystemId { get; set; }

        public SignKeyPair SignKeys { get; set; }
        public CryptKeyPair CryptKeys { get; set; }
        public AuthKeyPair AuthKeys { get; set; }

        static UserParams()
        {

        }

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
