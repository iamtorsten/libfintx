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
    public class OrderDetails : NamespaceAware, IXElementSerializer
    {
        public string OrderType { set; private get; }
        public string OrderAttribute { set; private get; }
        public IXElementSerializer StandardOrderParams { private get; set; }

        public XElement Serialize()
        {
            XNamespace nsEbics = Namespaces.Ebics;

            var x = new XElement(nsEbics + XmlNames.OrderDetails,
                new XElement(nsEbics + XmlNames.OrderType, OrderType),
                new XElement(nsEbics + XmlNames.OrderAttribute, OrderAttribute)
            );

            if (StandardOrderParams != null)
            {
                x.Add(StandardOrderParams.Serialize());
            }

            return x;
        }
    }
}