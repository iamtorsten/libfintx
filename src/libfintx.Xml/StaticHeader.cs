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