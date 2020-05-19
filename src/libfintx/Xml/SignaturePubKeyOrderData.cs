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
using libfintx.Config;

namespace libfintx.Xml
{
    internal class SignaturePubKeyOrderData : NamespaceAware, IXElementSerializer
    {
        internal string PartnerId { private get; set; }
        internal string UserId { private get; set; }
        internal SignKeyPair SignKeys { private get; set; }

        public XElement Serialize()
        {
            XNamespace nsEbics = Namespaces.SignatureData;
            XNamespace nsDsig = Namespaces.XmlDsig;

            var rsaParams = SignKeys.PublicKey.ExportParameters(false);
            var b64Mod = Convert.ToBase64String(rsaParams.Modulus);
            var b64Exp = Convert.ToBase64String(rsaParams.Exponent);

            var elem = new XElement(nsEbics + XmlNames.SignaturePubKeyOrderData,
                new XElement(nsEbics + XmlNames.SignaturePubKeyInfo,
                    new XElement(nsEbics + XmlNames.PubKeyValue,
                        new XElement(nsDsig + XmlNames.RSAKeyValue,
                            new XElement(nsDsig + XmlNames.Modulus, b64Mod),
                            new XElement(nsDsig + XmlNames.Exponent, b64Exp)
                        )
                    ),
                    new XElement(nsEbics + XmlNames.SignatureVersion, SignKeys.Version.ToString())
                ),
                new XElement(nsEbics + XmlNames.PartnerID, PartnerId),
                new XElement(nsEbics + XmlNames.UserID, UserId)
            );

            if (SignKeys.TimeStamp.HasValue)
            {
                elem.Descendants(nsEbics + XmlNames.PubKeyValue).FirstOrDefault()
                    ?.Add(new XElement(nsEbics + XmlNames.TimeStamp, CryptoUtils.FormatUtcTime(SignKeys.TimeStamp)));
            }

            return elem;
        }
    }
}