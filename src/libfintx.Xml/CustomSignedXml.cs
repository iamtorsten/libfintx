/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using libfintx.EBICSConfig;

namespace libfintx.Xml
{
    public class CustomSignedXml
    {
        private XmlDocument _doc;
        private byte[] _digestValue;
        private byte[] _signatureValue;

        private const string _digestAlg = "http://www.w3.org/2001/04/xmlenc#sha256";
        private const string _signAlg = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
        private const string _canonAlg = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";

        public const string DefaultReferenceUri = "#xpointer(//*[@authenticate='true'])";

        public string ReferenceUri { private get; set; }
        public RSA SignatureKey { private get; set; }
        public RSASignaturePadding SignaturePadding { private get; set; }

        public string CanonicalizationAlgorithm { private get; set; }
        public string DigestAlgorithm { private get; set; }
        public string SignatureAlgorithm { private get; set; }

        public byte[] DigestValue => _digestValue;
        public byte[] SignatureValue => _signatureValue;

        public XmlNamespaceManager NamespaceManager { private get; set; }
        public NamespaceConfig Namespaces { private get; set; }

        public CustomSignedXml(XmlDocument doc)
        {
            _doc = doc;
        }

        private byte[] CanonicalizeAndDigest(IEnumerable nodes)
        {
            var transform = new XmlDsigC14NTransform {Algorithm = CanonicalizationAlgorithm};

            var sb = new StringBuilder();
            foreach (XmlNode node in nodes)
            {
                var tmpDoc = new XmlDocument();
                tmpDoc.AppendChild(tmpDoc.ImportNode(node, true));
                transform.LoadInput(tmpDoc);
                using (var stream = (Stream) transform.GetOutput(typeof(Stream)))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        sb.Append(reader.ReadToEnd());
                    }
                }
            }

            using (var hash = SHA256.Create())
            {
                return hash.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
            }
        }

        private XDocument CreateDoc()
        {
            XNamespace ds = "http://www.w3.org/2000/09/xmldsig#";
            return new XDocument(
                new XElement(ds + XmlNames.SignedInfo,
                    new XElement(ds + XmlNames.CanonicalizationMethod,
                        new XAttribute(XmlNames.Algorithm, CanonicalizationAlgorithm)
                    ),
                    new XElement(ds + XmlNames.SignatureMethod,
                        new XAttribute(XmlNames.Algorithm, SignatureAlgorithm)
                    ),
                    new XElement(ds + XmlNames.Reference,
                        new XAttribute(XmlNames.URI, ReferenceUri),
                        new XElement(ds + XmlNames.Transforms,
                            new XElement(ds + XmlNames.Transform,
                                new XAttribute(XmlNames.Algorithm, CanonicalizationAlgorithm)
                            )
                        ),
                        new XElement(ds + XmlNames.DigestMethod,
                            new XAttribute(XmlNames.Algorithm, DigestAlgorithm)
                        ),
                        new XElement(ds + XmlNames.DigestValue, Convert.ToBase64String(_digestValue))
                    )
                )
            );
        }

        public XmlElement GetXml()
        {
            XNamespace ds = "http://www.w3.org/2000/09/xmldsig#";
            var signature = new XDocument(
                new XElement("Signature",
                    CreateDoc().Elements()
                )
            );
            signature.Descendants("Signature").FirstOrDefault()?
                .Add(new XElement(ds + XmlNames.SignatureValue, Convert.ToBase64String(_signatureValue)));
            var doc = signature.ToXmlDocument(true);
            return (XmlElement) doc.FirstChild;
        }

        public void ComputeSignature()
        {
            if (DigestAlgorithm != _digestAlg)
            {
                throw new CryptographicException($"Digest algorithm not supported. Use: {_digestAlg}");
            }

            if (SignatureAlgorithm != _signAlg)
            {
                throw new CryptographicException($"Signature algorithm not supported. Use: {_signAlg}");
            }

            if (CanonicalizationAlgorithm != _canonAlg)
            {
                throw new CryptographicException($"Canonicalization algorithm not supported. Use: {_canonAlg}");
            }

            if (SignatureKey == null)
            {
                throw new CryptographicException($"{nameof(SignatureKey)} is null");
            }

            if (ReferenceUri == null)
            {
                throw new CryptographicException($"{nameof(ReferenceUri)} is null");
            }

            var _ref = ReferenceUri;
            if (ReferenceUri.StartsWith("#xpointer("))
            {
                var customXPath = ReferenceUri.TrimEnd(')');
                _ref = customXPath.Substring(customXPath.IndexOf('(') + 1);
            }

            var nodes = NamespaceManager == null ? _doc.SelectNodes(_ref) : _doc.SelectNodes(_ref, NamespaceManager);

            if (nodes.Count == 0)
            {
                throw new CryptographicException("No references found");
            }

            _digestValue = CanonicalizeAndDigest(nodes);

            var signedInfo = CreateDoc();

            nodes = signedInfo.ToXmlDocument().SelectNodes("*");
            var signedInfoDigest = CanonicalizeAndDigest(nodes);
            _signatureValue =
                SignatureKey.SignHash(signedInfoDigest, HashAlgorithmName.SHA256, SignaturePadding);
        }
    }
}
