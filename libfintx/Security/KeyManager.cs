/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2017 Torsten Klinger
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
    public class KeyManager
    {
        public static byte[] Import_Public_Bank_Key()
        {
            byte[] Exponent = { 1, 0, 1 };

            byte[] PublicKey = Encoding.Default.GetBytes(RDH_KEYSTORE.KEY_ENCRYPTION_PUBLIC_BANK);

            //Create a new instance of the RSACryptoServiceProvider class.
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

            //Create a new instance of the RSAParameters structure. 
            RSAParameters RSAKeyInfo = new RSAParameters();

            //Set RSAKeyInfo to the public key values. 
            RSAKeyInfo.Modulus = PublicKey;
            RSAKeyInfo.Exponent = Exponent;

            //Import key parameters into RSA. 
            RSA.ImportParameters(RSAKeyInfo);

            return RSA.ExportCspBlob(false);
        }
    }
}
