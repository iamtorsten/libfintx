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

using System;
using System.Xml.Linq;
using libfintx.EBICSConfig;

namespace libfintx.Xml
{
    public class OrderSignatureData : NamespaceAware, IXElementSerializer
    {
        public SignKeyPair SignKeys { private get; set; }
        public byte[] SignatureValue { private get; set; }
        public string PartnerId { private get; set; }
        public string UserId { private get; set; }

        public XElement Serialize()
        {
            XNamespace ns = Namespaces.SignatureData;
            return new XElement(ns + XmlNames.OrderSignatureData,
                new XElement(ns + XmlNames.SignatureVersion, SignKeys.Version.ToString()),
                new XElement(ns + XmlNames.SignatureValue, Convert.ToBase64String(SignatureValue)),
                new XElement(ns + XmlNames.PartnerID, PartnerId),
                new XElement(ns + XmlNames.UserID, UserId)
            );
        }
    }
}
