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
using System.Numerics;
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

        static byte[] sessionKey;

        public static void Encrypt(string Message, out byte[] encSessionKey, out byte[] encMsg)
        {
            if (DEBUG.Enabled)
                DEBUG.Write("Plain message before encryption: " + Message);

            encSessionKey = encryptKey(Encoding.Default.GetBytes(RDH_KEYSTORE.KEY_ENCRYPTION_PUBLIC_BANK));

            encMsg = encryptMessage(Message);
        }

        static byte[] encryptKey(byte[] Key)
        {
            using (TripleDES des = TripleDES.Create())
            {
                sessionKey = des.Key;

                if (DEBUG.Enabled)
                    DEBUG.Write("3DES random key: " + libfintx.Converter.ByteArrayToString(des.Key));
            }

            if (DEBUG.Enabled)
                DEBUG.Write("Public key length: " + Key.Length);

            int cryptDataSize = Key.Length;
            byte[] plainText = new byte[cryptDataSize];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainText[i] = (byte)0x00;
            }

            Array.Copy(sessionKey, 0, plainText, plainText.Length - 16, 16);

            BigInteger m = new BigInteger(plainText);

            RSAParameters parameters = new RSAParameters();
            parameters.Modulus = Key;
            parameters.Exponent = new byte[] { 1, 0, 1 };

            BigInteger ex = new BigInteger(parameters.Exponent);
            BigInteger mo = new BigInteger(parameters.Modulus);

            BigInteger c = BigInteger.ModPow(m, ex, mo);

            byte[] result = c.ToByteArray();

            if (DEBUG.Enabled)
                DEBUG.Write("Encrypted session key: " + libfintx.Converter.ByteArrayToString(result));

            if (DEBUG.Enabled)
                DEBUG.Write("Encrypted session key length: " + libfintx.Converter.ByteArrayToString(result).Length);

            return result;
        }

        static byte[] encryptMessage(string msg)
        {
            byte[] result = null;

            byte[] iv = new byte[] { (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00 };

            byte[] plainmsg = Encoding.Default.GetBytes(msg);

            using (TripleDES des = TripleDES.Create())
            {
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.ANSIX923;

                result = des.CreateEncryptor(sessionKey, iv).TransformFinalBlock(plainmsg, 0, plainmsg.Length);
            }

            if (DEBUG.Enabled)
                DEBUG.Write("Encrypted message: " + libfintx.Converter.ByteArrayToString(result));

            if (DEBUG.Enabled)
                DEBUG.Write("Encrypted message length: " + result.Length);

            return result;
        }
    }
}
