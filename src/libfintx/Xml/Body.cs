/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System.Xml.Linq;
using libfintx.Config;

namespace libfintx.Xml
{
    internal class Body : NamespaceAware, IXElementSerializer
    {
        internal IXElementSerializer TransferReceipt { private get; set; }
        internal IXElementSerializer DataTransfer { private get; set; }

        public XElement Serialize()
        {
            XNamespace nsEBICS = Namespaces.Ebics;

            var x = new XElement(nsEBICS + XmlNames.body);

            if (DataTransfer != null)
            {
                x.Add(DataTransfer.Serialize());
            }

            if (TransferReceipt != null)
            {
                x.Add(TransferReceipt.Serialize());
            }

            return x;
        }
    }
}