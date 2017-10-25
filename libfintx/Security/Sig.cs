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

using System.Security.Cryptography;
using System.Text;

namespace libfintx
{
    public class Sig
    {
        public static string SECFUNC_HBCI_SIG_RDH = "1";
        public static string SECFUNC_HBCI_SIG_DDV = "2";

        public static string SECFUNC_FINTS_SIG_DIG = "1";
        public static string SECFUNC_FINTS_SIG_SIG = "2";

        public static string SECFUNC_SIG_PT_1STEP = "999";
        public static string SECFUNC_SIG_PT_2STEP_MIN = "900";
        public static string SECFUNC_SIG_PT_2STEP_MAX = "997";

        public static string HASHALG_SHA1 = "1";
        public static string HASHALG_SHA256 = "3";
        public static string HASHALG_SHA384 = "4";
        public static string HASHALG_SHA512 = "5";
        public static string HASHALG_SHA256_SHA256 = "6";
        public static string HASHALG_RIPEMD160 = "999";

        public static string SIGALG_DES = "1";
        public static string SIGALG_RSA = "10";

        public static string SIGMODE_ISO9796_1 = "16";
        public static string SIGMODE_ISO9796_2 = "17";
        public static string SIGMODE_PKCS1 = "18";
        public static string SIGMODE_PSS = "19";
        public static string SIGMODE_RETAIL_MAC = "999";

        /* https://stackoverflow.com/questions/39766324/c-how-to-hash-a-string-into-ripemd160 */

        public static string SignDataRIPEMD160(string Message)
        {
            // Create a ripemd160 object
            RIPEMD160 r160 = RIPEMD160Managed.Create();
            
            // Convert the string to byte
            byte[] myByte = System.Text.Encoding.Default.GetBytes(Message);
            
            // Compute the byte to RIPEMD160 hash
            byte[] encrypted = r160.ComputeHash(myByte);
            
            // Create a new StringBuilder process the hash byte
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < encrypted.Length; i++)
            {
                sb.Append(encrypted[i].ToString("X2"));
            }

            if (DEBUG.Enabled)
                DEBUG.Write("RIPEMD160 hashed message: " + sb.ToString());

            // Convert the StringBuilder to String
            return sb.ToString();
        }

        public static byte[] SignMessage(string hash)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                byte[] publicKey = Encoding.Default.GetBytes(RDH_KEYSTORE.KEY_SIGNING_PRIVATE);
                byte[] Exponent = { 1, 0, 1 };

                RSAParameters RSAKeyInfo = new RSAParameters();

                //Set RSAKeyInfo to the public key values. 
                RSAKeyInfo.Modulus = publicKey;
                RSAKeyInfo.Exponent = Exponent;

                rsa.ImportParameters(RSAKeyInfo);

                var encryptedMessage = rsa.Encrypt(Encoding.Default.GetBytes(hash), false);

                if (DEBUG.Enabled)
                    DEBUG.Write("Encrypted message: " + libfintx.Converter.ByteArrayToString(encryptedMessage));

                return encryptedMessage;
            }
        }
    }
}
