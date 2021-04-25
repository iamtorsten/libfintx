/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System;
using System.Xml.Linq;
using libfintx.EBICSConfig;

namespace libfintx.Xml
{
    public class BankPubKeyDigests : NamespaceAware, IXElementSerializer
    {
        public string DigestAlgorithm { private get; set; }
        public BankParams Bank { private get; set; }

        public XElement Serialize()
        {
            XNamespace nsEBICS = Namespaces.Ebics;

            return new XElement(nsEBICS + XmlNames.BankPubKeyDigests,
                new XElement(nsEBICS + XmlNames.Authentication,
                    new XAttribute(XmlNames.Algorithm, DigestAlgorithm),
                    new XAttribute(XmlNames.Version, Bank.AuthKeys.Version.ToString()),
                    Convert.ToBase64String(Bank.AuthKeys.Digest)
                ),
                new XElement(nsEBICS + XmlNames.Encryption,
                    new XAttribute(XmlNames.Algorithm, DigestAlgorithm),
                    new XAttribute(XmlNames.Version, Bank.CryptKeys.Version.ToString()),
                    Convert.ToBase64String(Bank.CryptKeys.Digest)
                )
            );
        }
    }
}
