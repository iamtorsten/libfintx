// *************************************************************************
// &lt;copyright file=&quot;DemoEncryption.cs&quot; company=&quot;Elegant Software Solutions, LLC&quot;&gt;
//     Copyright (C) 2011 Elegant Software Solutions, LLC.  All rights reserved worldwide.
// &lt;/copyright&gt;
// *

// PKCSKeyGenerator.cs
// Derive key material using PKCS #1 v1.5 algorithm with MD5 hash
//
// Portions Copyright (C) 2005.  Michel I. Gallant
// Portions copyright 2006 Richard Smith
// Adapted from http://www.jensign.com/JavaScience/dotnet/DeriveKeyM/index.html
//
// *************************************************************************
//
//  DeriveKeyM.cs
//
//  Derive a key from a pswd and Salt using MD5 and PKCS #5 v1.5 approach
//   see also:   http://www.openssl.org/docs/crypto/EVP_BytesToKey.html
//   see also:   http://java.sun.com/j2se/1.5.0/docs/guide/security/jce/JCERefGuide.html#PBE
//
// **************************************************************************

using System;
using System.Security.Cryptography;
using System.Text;

namespace libfintx
{
    /// &lt;summary&gt;
    /// This class is used to emulate the Java based PBEWithMD5AndDES functionality of the Demo system.
    /// &lt;/summary&gt;
    public class PkcsKeyGenerator
    {
        /// &lt;summary&gt;
        /// DES Provider used in the encryption algorythm.
        /// &lt;/summary&gt;
        private DESCryptoServiceProvider des = new DESCryptoServiceProvider();

        /// &lt;summary&gt;
        /// Initializes a new instance of the PKCSKeyGenerator class.
        /// &lt;/summary&gt;
        public PkcsKeyGenerator()
        {
            Key = new byte[8];
            IV = new byte[8];
        }

        /// &lt;summary&gt;
        /// Initializes a new instance of the PKCSKeyGenerator class.
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;keystring&quot;&gt;This is the same as the &quot;password&quot; of the PBEWithMD5AndDES method.&lt;/param&gt;
        /// &lt;param name=&quot;salt&quot;&gt;This is the salt used to provide extra security to the algorythim.&lt;/param&gt;
        /// &lt;param name=&quot;iterationsMd5&quot;&gt;Fill out iterationsMd5 later.&lt;/param&gt;
        /// &lt;param name=&quot;segments&quot;&gt;Fill out segments later.&lt;/param&gt;
        public PkcsKeyGenerator(string keyString, byte[] salt, int iterationsMd5, int segments) : this()
        {
            Generate(keyString, salt, iterationsMd5, segments);
        }

        /// &lt;summary&gt;
        /// Gets the asymetric Key used in the encryption algorythm.  Note that this is read only and is an empty byte array.
        /// &lt;/summary&gt;
        public byte[] Key { get; }

        /// &lt;summary&gt;
        /// Gets the initialization vector used in in the encryption algorythm.  Note that this is read only and is an empty byte array.
        /// &lt;/summary&gt;
        public byte[] IV { get; }

        /// &lt;summary&gt;
        /// Gets an ICryptoTransform interface for encryption
        /// &lt;/summary&gt;
        public ICryptoTransform Encryptor => des.CreateEncryptor(Key, IV);

        /// &lt;summary&gt;
        /// Gets an ICryptoTransform interface for decryption
        /// &lt;/summary&gt;
        public ICryptoTransform Decryptor => des.CreateDecryptor(Key, IV);

        /// &lt;summary&gt;
        /// Returns the ICryptoTransform interface used to perform the encryption.
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;keystring&quot;&gt;This is the same as the &quot;password&quot; of the PBEWithMD5AndDES method.&lt;/param&gt;
        /// &lt;param name=&quot;salt&quot;&gt;This is the salt used to provide extra security to the algorythim.&lt;/param&gt;
        /// &lt;param name=&quot;iterationsMd5&quot;&gt;Fill out iterationsMd5 later.&lt;/param&gt;
        /// &lt;param name=&quot;segments&quot;&gt;Fill out segments later.&lt;/param&gt;
        /// &lt;returns&gt;ICryptoTransform interface used to perform the encryption.&lt;/returns&gt;
        public ICryptoTransform Generate(string keyString, byte[] salt, int iterationsMd5, int segments)
        {
            // MD5 bytes
            int hashLength = 16;

            // to store contatenated Mi hashed results
            byte[] keyMaterial = new byte[hashLength * segments];

            // --- get secret password bytes ----
            byte[] passwordBytes;
            passwordBytes = Encoding.UTF8.GetBytes(keyString);

            // --- contatenate salt and pswd bytes into fixed data array ---
            byte[] data00 = new byte[passwordBytes.Length + salt.Length];

            // copy the pswd bytes
            Array.Copy(passwordBytes, data00, passwordBytes.Length);

            // concatenate the salt bytes
            Array.Copy(salt, 0, data00, passwordBytes.Length, salt.Length);

            // ---- do multi-hashing and contatenate results  D1, D2 ...  into keymaterial bytes ----
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = null;

            // fixed length initial hashtarget
            byte[] hashtarget = new byte[hashLength + data00.Length];

            for (int j = 0; j < segments; j++)
            {
                // ----  Now hash consecutively for iterationsMd5 times ------
                if (j == 0)
                {
                    // initialize
                    result = data00;
                }
                else
                {
                    Array.Copy(result, hashtarget, result.Length);
                    Array.Copy(data00, 0, hashtarget, result.Length, data00.Length);
                    result = hashtarget;
                }

                for (int i = 0; i < iterationsMd5; i++)
                {
                    result = md5.ComputeHash(result);
                }

                // contatenate to keymaterial
                Array.Copy(result, 0, keyMaterial, j * hashLength, result.Length);
            }

            Array.Copy(keyMaterial, 0, Key, 0, 8);
            Array.Copy(keyMaterial, 8, IV, 0, 8);

            return this.Encryptor;
        }
    }
}