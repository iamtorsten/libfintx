/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2017 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 * 	
 * 	libfintx is free software; you can redistribute it and/or
 *	modify it under the terms of the GNU Lesser General Public
 * 	License as published by the Free Software Foundation; either
 * 	version 2.1 of the License, or (at your option) any later version.
 *	
 * 	libfintx is distributed in the hope that it will be useful,
 * 	but WITHOUT ANY WARRANTY; without even the implied warranty of
 * 	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * 	Lesser General Public License for more details.
 *	
 * 	You should have received a copy of the GNU Lesser General Public
 * 	License along with libfintx; if not, write to the Free Software
 * 	Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 * 	
 */

/* https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp */

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace libfintx
{
    static class Crypt
    {
        // KEYS

        /* https://thomashundley.com/2011/04/21/emulating-javas-pbewithmd5anddes-encryption-with-net/ */
        public static string Encrypt_PBEWithMD5AndDES(string clearText, string passPhrase)
        {
            // TODO: Parameterize the Password, Salt, and Iterations.  They should be encrypted with the machine key and stored in the registry
            if (string.IsNullOrEmpty(clearText))
            {
                return clearText;
            }

            byte[] salt = { 0xc7, 0x73, 0x21, 0x8c, 0x7e, 0xc8, 0xee, 0x99 };

            // NOTE: The keystring, salt, and iterations must be the same as what is used in the Demo java system.
            PKCSKeyGenerator crypto = new PKCSKeyGenerator(passPhrase, salt, 20, 1);

            ICryptoTransform cryptoTransform = crypto.Encryptor;
            var cipherBytes = cryptoTransform.TransformFinalBlock(Encoding.UTF8.GetBytes(clearText), 0, clearText.Length);
            return Convert.ToBase64String(cipherBytes);
        }

        public static string Decrypt_PBEWithMD5AndDES(string cipherText, string passPhrase)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return cipherText;
            }

            byte[] salt = { 0xc7, 0x73, 0x21, 0x8c, 0x7e, 0xc8, 0xee, 0x99 };

            // NOTE: The keystring, salt, and iterations must be the same as what is used in the Demo java system.
            PKCSKeyGenerator crypto = new PKCSKeyGenerator(passPhrase, salt, 20, 1);

            ICryptoTransform cryptoTransform = crypto.Decryptor;
            var cipherBytes = Convert.FromBase64String(cipherText);
            var clearBytes = cryptoTransform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(clearBytes);
        }

        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // TODO: Encryption isnt working

        /* MESSAGE */

        /* 
         * Documentation/RDH-10*.jpg 
         */

        /* https://de.wikipedia.org/wiki/RSA-DES-Hybridverfahren */

        static byte[] SessionKey;

        public static void Encrypt(string secretMessage, out byte[] encryptedSessionKey, out byte[] encryptedMessage)
        {
            if (DEBUG.Enabled)
                DEBUG.Write("Plain message before encryption: " + secretMessage);

            using (TripleDES des = TripleDES.Create())
            {
                des.KeySize = 128;
                SessionKey = des.Key;

                if (DEBUG.Enabled)
                    DEBUG.Write("3DES random key: " + libfintx.Converter.ByteArrayToString(des.Key));
            }

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                byte[] publicKey = Encoding.Default.GetBytes(RDH_KEYSTORE.KEY_ENCRYPTION_PUBLIC_BANK);
                byte[] Exponent = { 1, 0, 1 };

                RSAParameters RSAKeyInfo = new RSAParameters();

                //Set RSAKeyInfo to the public key values. 
                RSAKeyInfo.Modulus = publicKey;
                RSAKeyInfo.Exponent = Exponent;

                rsa.ImportParameters(RSAKeyInfo);

                encryptedSessionKey = rsa.Encrypt(SessionKey, false);

                if (DEBUG.Enabled)
                    DEBUG.Write("Encrypted session key: " + libfintx.Converter.ByteArrayToString(encryptedSessionKey));
            }

            using (TripleDES des = TripleDES.Create())
            {
                des.Padding = PaddingMode.ANSIX923;
                des.Mode = CipherMode.CBC;

                des.Key = SessionKey;
                des.GenerateIV();

                //Encrypt the message
                using (MemoryStream ciphertext = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ciphertext, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] plaintextMessage = Encoding.Default.GetBytes(secretMessage);

                    if (DEBUG.Enabled)
                        DEBUG.Write("Plaintext message length: " + plaintextMessage.Length);

                    cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                    cs.Flush();
                    cs.FlushFinalBlock();

                    cs.Close();

                    encryptedMessage = ciphertext.ToArray();

                    if (DEBUG.Enabled)
                        DEBUG.Write("Encrypted message: " + libfintx.Converter.ByteArrayToString(encryptedMessage));

                    if (DEBUG.Enabled)
                        DEBUG.Write("Encrypted message length: " + encryptedMessage.Length);
                }
            }
        }

        
    }
}
