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
    internal class MutableHeader : NamespaceAware, IXElementSerializer
    {
        internal string TransactionPhase { private get; set; }
        internal int? SegmentNumber { private get; set; }
        internal bool LastSegment { private get; set; }

        public XElement Serialize()
        {
            XNamespace nsEbics = Namespaces.Ebics;

            var x = new XElement(nsEbics + XmlNames.mutable);

            if (TransactionPhase != null)
            {
                x.Add(new XElement(nsEbics + XmlNames.TransactionPhase, TransactionPhase));
            }

            if (SegmentNumber != null)
            {
                x.Add(new XElement(nsEbics + XmlNames.SegmentNumber,
                    new XAttribute(XmlNames.lastSegment, LastSegment),
                    SegmentNumber
                ));
            }

            return x;
        }
    }
}