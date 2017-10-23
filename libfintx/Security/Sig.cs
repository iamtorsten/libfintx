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

/* https://stackoverflow.com/questions/8437288/signing-and-verifying-signatures-with-rsa-c-sharp */

using System;
using System.Security.Cryptography;
using System.Text;

namespace libfintx
{
    public class Sig
    {
        public static string SignData(string message, RSA privateKey)
        {
            // The array to store the signed message in bytes
            byte[] signedBytes;

            using (var rsa = new RSACryptoServiceProvider())
            {
                // Write the message to a byte array using UTF8 as the encoding.
                var encoder = new ASCIIEncoding();

                byte[] originalData = encoder.GetBytes(message);

                try
                {
                    // Sign the data, using SHA1 as the hashing algorithm 
                    signedBytes = rsa.SignData(originalData, CryptoConfig.MapNameToOID("SHA1"));
                }
                catch (CryptographicException e)
                {
                    Log.Write(e.Message);

                    return null;
                }
                finally
                {
                    // Set the keycontainer to be cleared when rsa is garbage collected.
                    rsa.PersistKeyInCsp = false;
                }
            }
            
            // Convert the a base64 string before returning
            return Convert.ToBase64String(signedBytes);
        }
    }
}
