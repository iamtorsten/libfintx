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
using System.Security.Cryptography;
using System.Text;

namespace libfintx
{
    static class RdhxPassport
    {
        // KEYS

        /* https://thomashundley.com/2011/04/21/emulating-javas-pbewithmd5anddes-encryption-with-net/ */

        #region TripleDES

        public static string Encrypt_PBEWithMD5AndDES(string clearText, string passPhrase)
        {
            if (string.IsNullOrEmpty(clearText))
            {
                return clearText;
            }

            byte[] salt = { 0xc7, 0x73, 0x21, 0x8c, 0x7e, 0xc8, 0xee, 0x99 };

            // NOTE: The keystring, salt, and iterations must be the same as what is used in the Demo java system.
            PkcsKeyGenerator crypto = new PkcsKeyGenerator(passPhrase, salt, 20, 1);

            ICryptoTransform cryptoTransform = crypto.Encryptor;

            var cipherBytes = cryptoTransform.TransformFinalBlock(Encoding.GetEncoding("iso8859-1").GetBytes(clearText), 0, clearText.Length);

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
            PkcsKeyGenerator crypto = new PkcsKeyGenerator(passPhrase, salt, 20, 1);

            ICryptoTransform cryptoTransform = crypto.Decryptor;

            var cipherBytes = Convert.FromBase64String(cipherText);
            var clearBytes = cryptoTransform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.GetEncoding("iso8859-1").GetString(clearBytes);
        }

        #endregion

        #region RSA

        public static string Encrypt_RSA_PBEWithMD5AndDES(string clearText, string passPhrase)
        {
            if (string.IsNullOrEmpty(clearText))
            {
                return clearText;
            }

            byte[] salt = { 0xc7, 0x73, 0x21, 0x8c, 0x7e, 0xc8, 0xee, 0x99 };

            // NOTE: The keystring, salt, and iterations must be the same as what is used in the Demo java system.
            PkcsKeyGenerator crypto = new PkcsKeyGenerator(passPhrase, salt, 20, 1);

            ICryptoTransform cryptoTransform = crypto.Encryptor;
            var cipherBytes = cryptoTransform.TransformFinalBlock(Encoding.UTF8.GetBytes(clearText), 0, clearText.Length);
            return Convert.ToBase64String(cipherBytes);
        }

        public static string Decrypt_RSA_PBEWithMD5AndDES(string cipherText, string passPhrase)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return cipherText;
            }

            byte[] salt = { 0xc7, 0x73, 0x21, 0x8c, 0x7e, 0xc8, 0xee, 0x99 };

            // NOTE: The keystring, salt, and iterations must be the same as what is used in the Demo java system.
            PkcsKeyGenerator crypto = new PkcsKeyGenerator(passPhrase, salt, 20, 1);

            ICryptoTransform cryptoTransform = crypto.Decryptor;
            var cipherBytes = Convert.FromBase64String(cipherText);
            var clearBytes = cryptoTransform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(clearBytes);
        }

        #endregion
    }
}