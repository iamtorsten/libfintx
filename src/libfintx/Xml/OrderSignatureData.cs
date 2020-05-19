/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System;
using System.Xml.Linq;
using libfintx.Config;

namespace libfintx.Xml
{
    internal class OrderSignatureData : NamespaceAware, IXElementSerializer
    {
        internal SignKeyPair SignKeys { private get; set; }
        internal byte[] SignatureValue { private get; set; }
        internal string PartnerId { private get; set; }
        internal string UserId { private get; set; }

        public XElement Serialize()
        {
            XNamespace ns = Namespaces.SignatureData;
            return new XElement(ns + XmlNames.OrderSignatureData,
                new XElement(ns + XmlNames.SignatureVersion, SignKeys.Version.ToString()),
                new XElement(ns + XmlNames.SignatureValue, Convert.ToBase64String(SignatureValue)),
                new XElement(ns + XmlNames.PartnerID, PartnerId),
                new XElement(ns + XmlNames.UserID, UserId)
            );
        }
    }
}