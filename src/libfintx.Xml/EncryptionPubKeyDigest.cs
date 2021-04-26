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
    public class EncryptionPubKeyDigest : NamespaceAware, IXElementSerializer
    {
        public string DigestAlgorithm { private get; set; }
        public BankParams Bank { private get; set; }

        public XElement Serialize()
        {
            XNamespace nsEbics = Namespaces.Ebics;

            return new XElement(nsEbics + XmlNames.EncryptionPubKeyDigest,
                new XAttribute(XmlNames.Algorithm, DigestAlgorithm),
                new XAttribute(XmlNames.Version, Bank.CryptKeys.Version.ToString()),
                Convert.ToBase64String(Bank.CryptKeys.Digest)
            );
        }
    }
}
