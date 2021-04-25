/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System.Xml.Linq;
using libfintx.EBICSConfig;

namespace libfintx.Xml
{
    public class EbicsNoPubKeyDigestsRequest : NamespaceAware, IXDocumentSerializer
    {
        public EbicsVersion Version { private get; set; }
        public EbicsRevision Revision { private get; set; }
        public IXElementSerializer StaticHeader { private get; set; }
        public IXElementSerializer MutableHeader { private get; set; }
        public IXElementSerializer Body { private get; set; }

        public XDocument Serialize()
        {
            XNamespace nsEbics = Namespaces.Ebics;

            return new XDocument(
                new XElement(nsEbics + XmlNames.ebicsNoPubKeyDigestsRequest,
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
