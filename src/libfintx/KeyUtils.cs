/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2018 Bjoern Kuensting
 *  
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *  
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU Affero General Public License for more details.
 *  
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program. If not, see <http://www.gnu.org/licenses/>.
 * 	
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
