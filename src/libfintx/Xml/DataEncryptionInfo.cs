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
    internal class DataEncryptionInfo : NamespaceAware, IXElementSerializer
    {
        internal string TransactionKey { private get; set; }
        internal IXElementSerializer EncryptionPubKeyDigest { private get; set; }

        public XElement Serialize()
        {
            XNamespace nsEbics = Namespaces.Ebics;

            return new XElement(nsEbics + XmlNames.DataEncryptionInfo,
                new XAttribute(XmlNames.authenticate, true),
                EncryptionPubKeyDigest.Serialize(),
                new XElement(nsEbics + XmlNames.TransactionKey, TransactionKey)
            );
        }
    }
}