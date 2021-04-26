/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2018 Bjoern Kuensting
 *  
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 3 of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software Foundation,
 *  Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 * 	
 */

using System.Xml.Linq;
using libfintx.EBICSConfig;

namespace libfintx.Xml
{
    public class EbicsRequest : NamespaceAware, IXDocumentSerializer
    {
        public EbicsVersion Version { set; private get; }
        public EbicsRevision Revision { set; private get; }
        public IXElementSerializer StaticHeader { private get; set; }
        public IXElementSerializer MutableHeader { private get; set; }
        public IXElementSerializer Body { private get; set; }

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
