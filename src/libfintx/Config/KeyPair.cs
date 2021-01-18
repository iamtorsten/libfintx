/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;

namespace libfintx.Config
{
    public abstract class KeyPair<T> : IDisposable
    {
        
        
        protected RSA _publicKey;
        protected RSA _privateKey;
        protected X509Certificate2 _cert;

        public X509Certificate2 Certificate
        {
            get => _cert;
            set
            {
                _cert = value;

                if (_cert == null)
                {
                    return;
                }

                _publicKey = _cert.GetRSAPublicKey();
                _privateKey = _cert.GetRSAPrivateKey();
            }
        }

        public RSA PrivateKey
        {
            get => _privateKey;
            set => _privateKey = value;
        }

        public RSA PublicKey
        {
            get => _publicKey;
            set => _publicKey = value;
        }

        public byte[] Digest
        {
            get
            {
                if (_publicKey == null)
                {
                    return null;
                }

                var p = _publicKey.ExportParameters(false);
                var hexExp = BitConverter.ToString(p.Exponent).Replace("-", string.Empty).ToLower()
                    .TrimStart('0');
                var hexMod = BitConverter.ToString(p.Modulus).Replace("-", string.Empty).ToLower()
                    .TrimStart('0');
                var hashInput = Encoding.ASCII.GetBytes(string.Format("{0} {1}", hexExp, hexMod));

                using (var sha256 = SHA256.Create())
                {
                    return sha256.ComputeHash(hashInput);
                }
            }
        }

        public DateTime? TimeStamp { get; set; }

        public T Version { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _publicKey?.Dispose();
                _privateKey?.Dispose();
                _cert?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        static KeyPair()
        {
            
        }

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
