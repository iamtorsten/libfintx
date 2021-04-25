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
    public class DataTransfer : NamespaceAware, IXElementSerializer
    {
        public string OrderData { private get; set; }
        public IXElementSerializer DataEncryptionInfo { private get; set; }
        public IXElementSerializer SignatureData { private get; set; }

        public XElement Serialize()
        {
            XNamespace nsEbics = Namespaces.Ebics;

            var x = new XElement(nsEbics + XmlNames.DataTransfer);

            if (OrderData != null)
            {
                x.Add(new XElement(nsEbics + XmlNames.OrderData, OrderData));
            }

            if (DataEncryptionInfo != null)
            {
                x.Add(DataEncryptionInfo.Serialize());
            }

            if (SignatureData != null)
            {
                x.Add(SignatureData.Serialize());
            }

            return x;
        }
    }
}
