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
    internal class EbicsRequest : NamespaceAware, IXDocumentSerializer
    {
        internal EbicsVersion Version { set; private get; }
        internal EbicsRevision Revision { set; private get; }
        internal IXElementSerializer StaticHeader { private get; set; }
        internal IXElementSerializer MutableHeader { private get; set; }
        internal IXElementSerializer Body { private get; set; }

        public XDocument Serialize()
        {
            XNamespace nsEbics = Namespaces.Ebics;

            return new XDocument(
                new XElement(nsEbics + XmlNames.ebicsRequest,
                    new XAttribute(XmlNames.Version, Version.ToString()),
                    new XAttribute(XmlNames.Revision, Revision.ToString().TrimStart("Rev".ToCharArray())),
                    new XElement(nsEbics + XmlNames.header,
                        new XAttribute(XmlNames.authenticate, "true"),
                        StaticHeader.Serialize(),
                        MutableHeader.Serialize()
                    ),
                    new XElement(nsEbics + XmlNames.AuthSignature),
                    Body.Serialize()
                )
            );
        }
    }
}