/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace libfintx.Xml
{
    public static class XDocumentExtensions
    {
        public static XmlDocument ToXmlDocument(this XDocument xdoc, bool omitXmlDeclaration = false)
        {
            var sb = new StringBuilder();
            using (var s = new Utf8StringWriter(sb))
            {
                xdoc.Save(s);
            }

            var doc = new XmlDocument();
            doc.LoadXml(sb.ToString());
            
            if (!omitXmlDeclaration)
            {
                return doc;
            }

            if (doc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                doc.RemoveChild(doc.FirstChild);

            return doc;
        }
    }
}