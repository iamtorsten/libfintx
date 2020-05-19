/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using libfintx.Config;

namespace libfintx.Xml
{
    internal class XPathHelper
    {
        private XmlNamespaceManager _nm;
        private NamespaceConfig _nsc;
        private XDocument _doc;

        internal string Xml => _doc?.ToString();

        public XPathHelper(XDocument doc, NamespaceConfig nsc)
        {
            _doc = doc;
            _nsc = nsc;
            var r = _doc.CreateReader();
            _nm = new XmlNamespaceManager(r.NameTable);
            _nm.AddNamespace(_nsc.EbicsPrefix, _nsc.Ebics);
            _nm.AddNamespace(_nsc.XmlDsigPrefix, _nsc.XmlDsig);
        }

        private string DNS(string name)
        {
            return $"{_nsc.EbicsPrefix}:{name}";
        }

        private string SNS(string name)
        {
            return $"{_nsc.XmlDsigPrefix}:{name}";
        }

        internal XElement GetTechReturnCode()
        {
            return _doc.XPathSelectElement(
                $"/*/{DNS(XmlNames.header)}/{DNS(XmlNames.mutable)}/{DNS(XmlNames.ReturnCode)}",
                _nm);
        }

        internal XElement GetBusReturnCode()
        {
            return _doc.XPathSelectElement($"/*/{DNS(XmlNames.body)}/{DNS(XmlNames.ReturnCode)}", _nm);
        }

        internal XElement GetOrderID()
        {
            return _doc.XPathSelectElement(
                $"/*/{DNS(XmlNames.header)}/{DNS(XmlNames.staticHeader)}/{DNS(XmlNames.OrderID)}", _nm);
        }

        internal XElement GetTimestampBankParameter()
        {
            return _doc.XPathSelectElement($"/*/{DNS(XmlNames.body)}/{DNS(XmlNames.TimestampBankParameter)}", _nm);
        }

        internal XElement GetReportText()
        {
            //var s = $"{DNS(XMLNames.header)}/{DNS(XMLNames.mutable)}/{DNS(XMLNames.ReportText)}";
            var elem = _doc.XPathSelectElement(
                $"/*/{DNS(XmlNames.header)}/{DNS(XmlNames.mutable)}/{DNS(XmlNames.ReportText)}",
                _nm);
            return elem;
        }

        internal XElement GetOrderData()
        {
            return _doc.XPathSelectElement(
                $"/*/{DNS(XmlNames.body)}/{DNS(XmlNames.DataTransfer)}/{DNS(XmlNames.OrderData)}", _nm);
        }

        internal XAttribute GetEncryptionPubKeyDigestVersion()
        {
            var elem = _doc.XPathSelectElement(
                $"/*/{DNS(XmlNames.body)}/{DNS(XmlNames.DataTransfer)}/{DNS(XmlNames.DataEncryptionInfo)}/{DNS(XmlNames.EncryptionPubKeyDigest)}",
                _nm);
            return elem?.Attribute(XmlNames.Version);
        }

        internal XElement GetEncryptionPubKeyDigest()
        {
            return _doc.XPathSelectElement(
                $"/*/{DNS(XmlNames.body)}/{DNS(XmlNames.DataTransfer)}/{DNS(XmlNames.DataEncryptionInfo)}/{DNS(XmlNames.EncryptionPubKeyDigest)}",
                _nm);
        }

        internal XElement GetTransactionKey()
        {
            return _doc.XPathSelectElement(
                $"/*/{DNS(XmlNames.body)}/{DNS(XmlNames.DataTransfer)}/{DNS(XmlNames.DataEncryptionInfo)}/{DNS(XmlNames.TransactionKey)}",
                _nm);
        }

        internal XElement GetAuthenticationPubKeyInfoX509Data()
        {
            return _doc.XPathSelectElement($"/*/{DNS(XmlNames.AuthenticationPubKeyInfo)}/{DNS(XmlNames.X509)}", _nm);
        }

        internal XElement GetEncryptionPubKeyInfoX509Data()
        {
            return _doc.XPathSelectElement($"/*/{DNS(XmlNames.EncryptionPubKeyInfo)}/{DNS(XmlNames.X509)}", _nm);
        }

        internal XElement GetEncryptionPubKeyInfoPubKeyValue()
        {
            return _doc.XPathSelectElement($"/*/{DNS(XmlNames.EncryptionPubKeyInfo)}/{DNS(XmlNames.PubKeyValue)}",
                _nm);
        }

        internal XElement GetAuthenticationPubKeyInfoPubKeyValue()
        {
            return _doc.XPathSelectElement($"/*/{DNS(XmlNames.AuthenticationPubKeyInfo)}/{DNS(XmlNames.PubKeyValue)}",
                _nm);
        }

        internal XElement GetAuthenticationPubKeyInfoAuthenticationVersion()
        {
            return _doc.XPathSelectElement(
                $"/*/{DNS(XmlNames.AuthenticationPubKeyInfo)}/{DNS(XmlNames.AuthenticationVersion)}", _nm);
        }

        internal XElement GetEncryptionPubKeyInfoEncryptionVersion()
        {
            return _doc.XPathSelectElement(
                $"/*/{DNS(XmlNames.EncryptionPubKeyInfo)}/{DNS(XmlNames.EncryptionVersion)}", _nm);
        }

        internal XElement GetAuthenticationPubKeyInfoModulus()
        {
            return _doc.XPathSelectElement(
                $"/*/{DNS(XmlNames.AuthenticationPubKeyInfo)}/{DNS(XmlNames.PubKeyValue)}/{SNS(XmlNames.RSAKeyValue)}/{SNS(XmlNames.Modulus)}",
                _nm);
        }

        internal XElement GetAuthenticationPubKeyInfoExponent()
        {
            return _doc.XPathSelectElement(
                $"/*/{DNS(XmlNames.AuthenticationPubKeyInfo)}/{DNS(XmlNames.PubKeyValue)}/{SNS(XmlNames.RSAKeyValue)}/{SNS(XmlNames.Exponent)}",
                _nm);
        }

        internal XElement GetEncryptionPubKeyInfoModulus()
        {
            return _doc.XPathSelectElement(
                $"/*/{DNS(XmlNames.EncryptionPubKeyInfo)}/{DNS(XmlNames.PubKeyValue)}/{SNS(XmlNames.RSAKeyValue)}/{SNS(XmlNames.Modulus)}",
                _nm);
        }

        internal XElement GetEncryptionPubKeyInfoExponent()
        {
            return _doc.XPathSelectElement(
                $"/*/{DNS(XmlNames.EncryptionPubKeyInfo)}/{DNS(XmlNames.PubKeyValue)}/{SNS(XmlNames.RSAKeyValue)}/{SNS(XmlNames.Exponent)}",
                _nm);
        }

        internal XElement GetTransactionID()
        {
            return _doc.XPathSelectElement(
                $"/*/{DNS(XmlNames.header)}/{DNS(XmlNames.staticHeader)}/{DNS(XmlNames.TransactionID)}", _nm);
        }
        
        internal XElement GetNumSegments()
        {
            return _doc.XPathSelectElement(
                $"/*/{DNS(XmlNames.header)}/{DNS(XmlNames.staticHeader)}/{DNS(XmlNames.NumSegments)}", _nm);
        }
        
        internal XElement GetTransactionPhase()
        {
            return _doc.XPathSelectElement(
                $"/*/{DNS(XmlNames.header)}/{DNS(XmlNames.mutable)}/{DNS(XmlNames.TransactionPhase)}", _nm);
        }
        
        internal XElement GetSegmentNumber()
        {
            return _doc.XPathSelectElement(
                $"/*/{DNS(XmlNames.header)}/{DNS(XmlNames.mutable)}/{DNS(XmlNames.SegmentNumber)}", _nm);
        }
    }
}