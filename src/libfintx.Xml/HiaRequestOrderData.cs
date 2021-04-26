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
    public class HiaRequestOrderData : NamespaceAware, IXElementSerializer
    {
        public string PartnerId { private get; set; }
        public string UserId { private get; set; }
        public IXElementSerializer AuthInfo { private get; set; }
        public IXElementSerializer EncInfo { private get; set; }

        public XElement Serialize()
        {
            XNamespace nsEbics = Namespaces.Ebics;

            return new XElement(nsEbics + XmlNames.HIARequestOrderData,
                AuthInfo.Serialize(),
                EncInfo.Serialize(),
                new XElement(nsEbics + XmlNames.PartnerID, PartnerId),
                new XElement(nsEbics + XmlNames.UserID, UserId)
            );
        }
    }
}
