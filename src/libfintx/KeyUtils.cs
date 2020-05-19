/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;

namespace libfintx
{
    public static class KeyUtils
    {
        public static AsymmetricCipherKeyPair ReadPrivateKeyFromPem(string fileName)
        {
            using (var sr = new StringReader(File.ReadAllText(fileName).Trim()))
            {
                var pr = new PemReader(sr);
                return (AsymmetricCipherKeyPair) pr.ReadObject();
            }
        }

        public static X509Certificate2 CreateX509Certificate2(AsymmetricCipherKeyPair kp)
        {
            var random = new SecureRandom();
            var sf = new Asn1SignatureFactory(HashAlg.SHA256withRSA.ToString(), kp.Private, random);
            
            var gen = new X509V3CertificateGenerator();
            gen.SetSerialNumber(BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random));
            var subject = new X509Name("CN=" + "ebics.org");
            gen.SetSubjectDN(subject);
            gen.SetIssuerDN(subject);
            var notBefore = DateTime.UtcNow.Date;
            var notAfter = notBefore.AddYears(10);
            gen.SetNotBefore(notBefore);
            gen.SetNotAfter(notAfter); 
            gen.SetPublicKey(kp.Public); 
            var bouncyCert = gen.Generate(sf);            
            
            var authorityKeyIdentifier = new AuthorityKeyIdentifier(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(kp.Public), new GeneralNames(new GeneralName(subject)), bouncyCert.SerialNumber);
            gen.AddExtension(X509Extensions.AuthorityKeyIdentifier.Id, false, authorityKeyIdentifier);
            var subjectKeyIdentifier = new SubjectKeyIdentifier(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(kp.Public));
            gen.AddExtension(X509Extensions.SubjectKeyIdentifier.Id, false, subjectKeyIdentifier);
            gen.AddExtension(X509Extensions.BasicConstraints.Id, true, new BasicConstraints(false));
            
            var store = new Pkcs12Store();
            var certificateEntry = new X509CertificateEntry(bouncyCert);
            store.SetCertificateEntry(bouncyCert.SubjectDN.ToString(), certificateEntry);
            store.SetKeyEntry(bouncyCert.SubjectDN.ToString(), new AsymmetricKeyEntry(kp.Private), new[] { certificateEntry });
            const string pwd = "password";
            var stream = new MemoryStream();
            store.Save(stream, pwd.ToCharArray(), random);
            var msCert = new X509Certificate2(stream.ToArray(), pwd, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

            return msCert;
        }

        public static AsymmetricCipherKeyPair GenerateRSAKeyPair(int strength)
        {
            var gen = GeneratorUtilities.GetKeyPairGenerator("RSA");
            gen.Init(new KeyGenerationParameters(new SecureRandom(), strength));
            return gen.GenerateKeyPair();            
        }
    }
}