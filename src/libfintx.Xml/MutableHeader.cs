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

namespace libfintx.Xml
{
    public class MutableHeader : NamespaceAware, IXElementSerializer
    {
        public string TransactionPhase { private get; set; }
        public int? SegmentNumber { private get; set; }
        public bool LastSegment { private get; set; }

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
