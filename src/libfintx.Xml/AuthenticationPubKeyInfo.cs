/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System;
using System.Linq;
using System.Xml.Linq;
using libfintx.EBICSConfig;
using libfintx.Security;

namespace libfintx.Xml
{
    public class AuthenticationPubKeyInfo : NamespaceAware, IXElementSerializer
    {
        public AuthKeyPair AuthKeys { private get; set; }
        public bool UseEbicsDefaultNamespace { private get; set; }

        public XElement Serialize()
        {
            XNamespace nsEBICS = UseEbicsDefaultNamespace ? Namespaces.Ebics : Namespaces.SignatureData;

            XNamespace nsDsig = Namespaces.XmlDsig;

            var rsaParams = AuthKeys.PublicKey.ExportParameters(false);
            var b64Mod = Convert.ToBase64String(rsaParams.Modulus);
            var b64Exp = Convert.ToBase64String(rsaParams.Exponent);

            var elem = new XElement(nsEBICS + XmlNames.AuthenticationPubKeyInfo,
                new XElement(nsEBICS + XmlNames.PubKeyValue,
                    new XElement(nsDsig + XmlNames.RSAKeyValue,
                        new XElement(nsDsig + XmlNames.Modulus, b64Mod),
                        new XElement(nsDsig + XmlNames.Exponent, b64Exp)
                    )
                ),
                new XElement(nsEBICS + XmlNames.AuthenticationVersion, AuthKeys.Version.ToString())
            );

            if (AuthKeys.TimeStamp.HasValue)
            {
                elem.Descendants(nsEBICS + XmlNames.PubKeyValue).FirstOrDefault()
                    ?.Add(new XElement(nsEBICS + XmlNames.TimeStamp, CryptoUtils.FormatUtcTime(AuthKeys.TimeStamp)));
            }

            return elem;
        }
    }
}
