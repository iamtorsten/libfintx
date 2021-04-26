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
    public class DataEncryptionInfo : NamespaceAware, IXElementSerializer
    {
        public string TransactionKey { private get; set; }
        public IXElementSerializer EncryptionPubKeyDigest { private get; set; }

        public XElement Serialize()
        {
            XNamespace nsEbics = Namespaces.Ebics;

            return new XElement(nsEbics + XmlNames.DataEncryptionInfo,
                new XAttribute(XmlNames.authenticate, true),
                EncryptionPubKeyDigest.Serialize(),
                new XElement(nsEbics + XmlNames.TransactionKey, TransactionKey)
            );
        }
    }
}
