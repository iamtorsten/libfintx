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
    public class UserSignatureData : NamespaceAware, IXElementSerializer
    {
        public IXElementSerializer OrderSignatureData { private get; set; }

        public XElement Serialize()
        {
            XNamespace ns = Namespaces.SignatureData;
            return new XElement(ns + XmlNames.UserSignatureData,
                OrderSignatureData.Serialize()
            );
        }
    }
}