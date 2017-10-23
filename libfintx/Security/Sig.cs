/*	
 * 	
 *  This file is part of hbci4dotnet.
 *  
 *  Copyright (c) 2017 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 * 	
 * 	hbci4dotnet is free software; you can redistribute it and/or
 *	modify it under the terms of the GNU Lesser General Public
 * 	License as published by the Free Software Foundation; either
 * 	version 2.1 of the License, or (at your option) any later version.
 *	
 * 	hbci4dotnet is distributed in the hope that it will be useful,
 * 	but WITHOUT ANY WARRANTY; without even the implied warranty of
 * 	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * 	Lesser General Public License for more details.
 *	
 * 	You should have received a copy of the GNU Lesser General Public
 * 	License along with hbci4dotnet; if not, write to the Free Software
 * 	Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 * 	
 */

using System.Security.Cryptography;
using System.Text;

namespace libfintx
{
    public class Sig
    {
        public static byte[] SignData(string message)
        {
            /* PRIVATE KEY */

            byte[] str = Encoding.Default.GetBytes(message);

            // Compute the hash
            SHA1Managed sha1hash = new SHA1Managed();
            byte[] hashdata = sha1hash.ComputeHash(str);

            // Sign the hash data with private key
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportCspBlob(KeyManager.Import_Private_SIG_Key());
            
            // Signature hold the sign data of plaintext , signed by private key
            byte[] signature = rsa.SignData(str, "SHA1");

            return signature;
        }

        public static bool VerifyData(byte[] signature, string plaintext)
        {
            /* PUBLIC KEY */

            byte[] str = Encoding.Default.GetBytes(plaintext);

            // Compute the hash again
            SHA1Managed sha1hash = new SHA1Managed();
            byte[] hashdata = sha1hash.ComputeHash(str);

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportCspBlob(KeyManager.Import_Private_SIG_Key());

            if (rsa.VerifyHash(hashdata, "SHA1", signature))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
