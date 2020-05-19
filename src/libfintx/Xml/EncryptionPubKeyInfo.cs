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
    internal class EncryptionPubKeyInfo : NamespaceAware, IXElementSerializer
    {
        internal CryptKeyPair CryptKeys { private get; set; }
        internal bool UseEbicsDefaultNamespace { private get; set; }

        public XElement Serialize()
        {
            XNamespace nsEbics = UseEbicsDefaultNamespace ? Namespaces.Ebics : Namespaces.SignatureData;
            XNamespace nsDsig = Namespaces.XmlDsig;

            var rsaParams = CryptKeys.PublicKey.ExportParameters(false);
            var b64Mod = Convert.ToBase64String(rsaParams.Modulus);
            var b64Exp = Convert.ToBase64String(rsaParams.Exponent);

            var elem = new XElement(nsEbics + XmlNames.EncryptionPubKeyInfo,
                new XElement(nsEbics + XmlNames.PubKeyValue,
                    new XElement(nsDsig + XmlNames.RSAKeyValue,
                        new XElement(nsDsig + XmlNames.Modulus, b64Mod),
                        new XElement(nsDsig + XmlNames.Exponent, b64Exp)
                    )
                ),
                new XElement(nsEbics + XmlNames.EncryptionVersion, CryptKeys.Version.ToString())
            );

            if (CryptKeys.TimeStamp.HasValue)
            {
                elem.Descendants(nsEbics + XmlNames.PubKeyValue).FirstOrDefault()
                    .Add(new XElement(nsEbics + XmlNames.TimeStamp, CryptoUtils.FormatUtcTime(CryptKeys.TimeStamp)));
            }

            return elem;
        }
    }
}