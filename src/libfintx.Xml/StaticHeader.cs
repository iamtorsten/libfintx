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
    public class StaticHeader : NamespaceAware, IXElementSerializer
    {
        public string HostId { set; private get; }
        public string PartnerId { set; private get; }
        public string UserId { set; private get; }
        public string SecurityMedium { set; private get; }
        public string Nonce { private get; set; }
        public string Timestamp { private get; set; }
        public string TransactionId { private get; set; }
        public int? NumSegments { private get; set; }
        public IXElementSerializer OrderDetails { private get; set; }
        public IXElementSerializer BankPubKeyDigests { private get; set; }

        public XElement Serialize()
        {
            XNamespace nsEbics = Namespaces.Ebics;

            var x = new XElement(nsEbics + XmlNames.staticHeader);

            if (HostId != null)
            {
                x.Add(new XElement(nsEbics + XmlNames.HostID, HostId));
            }

            if (TransactionId != null)
            {
                x.Add(new XElement(nsEbics + XmlNames.TransactionID, TransactionId));
            }

            if (Nonce != null)
            {
                x.Add(new XElement(nsEbics + XmlNames.Nonce, Nonce));
            }

            if (Timestamp != null)
            {
                x.Add(new XElement(nsEbics + XmlNames.Timestamp, Timestamp));
            }

            if (PartnerId != null)
            {
                x.Add(new XElement(nsEbics + XmlNames.PartnerID, PartnerId));
            }

            if (UserId != null)
            {
                x.Add(new XElement(nsEbics + XmlNames.UserID, UserId));
            }

            if (OrderDetails != null)
            {
                x.Add(OrderDetails.Serialize());
            }

            if (BankPubKeyDigests != null)
            {
                x.Add(BankPubKeyDigests.Serialize());
            }

            if (SecurityMedium != null)
            {
                x.Add(new XElement(nsEbics + XmlNames.SecurityMedium, SecurityMedium));
            }

            if (NumSegments != null)
            {
                x.Add(new XElement(nsEbics + XmlNames.NumSegments, NumSegments));
            }

            return x;
        }
    }
}
