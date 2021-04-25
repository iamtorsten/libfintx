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
    public class TransferReceipt : NamespaceAware, IXElementSerializer
    {
        public string ReceiptCode { private get; set; }

        public XElement Serialize()
        {
            XNamespace nsEbics = Namespaces.Ebics;

            return new XElement(nsEbics + XmlNames.TransferReceipt,
                new XAttribute(XmlNames.authenticate, "true"),
                new XElement(nsEbics + XmlNames.ReceiptCode, ReceiptCode)
            );
        }
    }
}