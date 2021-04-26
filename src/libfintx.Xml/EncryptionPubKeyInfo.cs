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
using System.Linq;
using System.Xml.Linq;
using libfintx.EBICSConfig;
using libfintx.Security;

namespace libfintx.Xml
{
    public class EncryptionPubKeyInfo : NamespaceAware, IXElementSerializer
    {
        public CryptKeyPair CryptKeys { private get; set; }
        public bool UseEbicsDefaultNamespace { private get; set; }

        public XElement Serialize()
        {
            XNamespace nsEbics = UseEbicsDefaultNamespace ? Namespaces.Ebics : Namespaces.SignatureData;
            XNamespace nsDsig = Namespaces.XmlDsig;

            var rsaParams = CryptKeys.PublicKey.ExportParameters(false);
            var b64Mod = Convert.ToBase64String(rsaParams.Modulus);
            var b64Exp = Convert.ToBase64String(rsaParams.Exponent);

            var elem = new XElement(nsEbics + XmlNames.EncryptionPubKeyInfo,
                new XElement(nsEbics + XmlNames.PubKeyValue,
                    new XElement(nsDsig + XmlNames.RSAKeyValue,
                        new XElement(nsDsig + XmlNames.Modulus, b64Mod),
                        new XElement(nsDsig + XmlNames.Exponent, b64Exp)
                    )
                ),
                new XElement(nsEbics + XmlNames.EncryptionVersion, CryptKeys.Version.ToString())
            );

            if (CryptKeys.TimeStamp.HasValue)
            {
                elem.Descendants(nsEbics + XmlNames.PubKeyValue).FirstOrDefault()
                    .Add(new XElement(nsEbics + XmlNames.TimeStamp, CryptoUtils.FormatUtcTime(CryptKeys.TimeStamp)));
            }

            return elem;
        }
    }
}
