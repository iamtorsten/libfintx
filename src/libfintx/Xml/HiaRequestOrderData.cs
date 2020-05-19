/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System.Xml.Linq;

namespace libfintx.Xml
{
    internal class HiaRequestOrderData : NamespaceAware, IXElementSerializer
    {
        internal string PartnerId { private get; set; }
        internal string UserId { private get; set; }
        internal IXElementSerializer AuthInfo { private get; set; }
        internal IXElementSerializer EncInfo { private get; set; }

        public XElement Serialize()
        {
            XNamespace nsEbics = Namespaces.Ebics;

            return new XElement(nsEbics + XmlNames.HIARequestOrderData,
                AuthInfo.Serialize(),
                EncInfo.Serialize(),
                new XElement(nsEbics + XmlNames.PartnerID, PartnerId),
                new XElement(nsEbics + XmlNames.UserID, UserId)
            );
        }
    }
}