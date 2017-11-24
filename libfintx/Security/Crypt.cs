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

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Paddings;

using System.Text;
using System;

namespace libfintx
{
    public static class Crypt
    {

#region RDH-10

        static byte[] sessionKey;

        public static void Encrypt(string Message, out byte[] encSessionKey, out byte[] encMsg)
        {
            if (DEBUG.Enabled)
                DEBUG.Write("Plain message before encryption: " + Message);

            if (DEBUG.Enabled)
                DEBUG.Write("Plain message length: " + Message.Length);

            if (DEBUG.Enabled)
                DEBUG.Write("Public bank encryption key: " + Converter.ByteArrayToString(Encoding.GetEncoding("iso8859-1").GetBytes(RDH_KEYSTORE.KEY_ENCRYPTION_PUBLIC_BANK)));

            encSessionKey = encryptKey(Encoding.GetEncoding("iso8859-1").GetBytes(RDH_KEYSTORE.KEY_ENCRYPTION_PUBLIC_BANK));

            encMsg = encryptMessage(Message);
        }

        public static byte[] InitDES3Key()
        {

            DesEdeKeyGenerator gen = new DesEdeKeyGenerator();
            gen.Init(new KeyGenerationParameters(new SecureRandom(), 127));

            var k = gen.GenerateKey();
            return k;
        }

        static byte[] encryptKey(byte[] Key)
        {
            sessionKey = InitDES3Key();

            if (DEBUG.Enabled)
                DEBUG.Write("Public key length: " + Key.Length);

            var Exponent = new byte[] { 1, 0, 1 };

            var key = Encoding.GetEncoding("iso8859-1").GetBytes(RDH_KEYSTORE.KEY_ENCRYPTION_PUBLIC_BANK);

            BigInteger n = new BigInteger(key);

            int cryptDataSize = n.BitLength;

            byte[] plainText = new byte[cryptDataSize];

            Array.Copy(sessionKey, 0, plainText, plainText.Length - 16, 16);

            BigInteger m = new BigInteger(plainText);

            BigInteger ex = new BigInteger(Exponent);

            BigInteger mo = new BigInteger(+1, key);

            var v = m.ModPow(ex, mo);

            byte[] result = v.ToByteArray();

            if (DEBUG.Enabled)
                DEBUG.Write("Encrypted session key: " + Converter.ByteArrayToString(result));

            if (DEBUG.Enabled)
                DEBUG.Write("Encrypted session key length: " + result.Length);

            // Check for encrypted session key size
            var cryptLength = HBCI_Util.checkForCryptSize(RDH_KEYSTORE.KEY_ENCRYPTION_PUBLIC_BANK.Length, result.Length);

            if (DEBUG.Enabled)
                DEBUG.Write("Crypted session key length is valid: " + cryptLength.ToString());

            if (Trace.Enabled)
                Trace.Write("Session key length: " + result.Length);

            // Throw exception when size is not valid
            if (!cryptLength)
                throw new Exception(HBCI_Exception.CRYPTEDLENGTH());

            return result;
        }

        static byte[] encryptMessage(string msg)
        {
            byte[] plainmsg = Encoding.GetEncoding("iso8859-1").GetBytes(msg);

            byte[] iv = new byte[8];

            DesEdeEngine desedeEngine = new DesEdeEngine();
            BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(desedeEngine));

            KeyParameter keyparam = ParameterUtilities.CreateKeyParameter("DESEDE", sessionKey);
            ParametersWithIV spec = new ParametersWithIV(keyparam, iv);

            byte[] output = new byte[cipher.GetOutputSize(plainmsg.Length)];

            cipher.Init(true, keyparam);

            output = cipher.DoFinal(plainmsg);

            return output;
        }

        public static string DecryptMessageandKey(byte[] cryptKey, byte[] cryptMessage)
        {
            // Decrypt session key
            var k = Encoding.GetEncoding("iso8859-1").GetBytes(RDH_KEYSTORE.KEY_ENCRYPTION_PRIVATE);

            var Exponent = new byte[] { 1, 0, 1 };

            byte[] pKey;

            BigInteger exponent = new BigInteger(+1, Exponent);
            BigInteger modulus = new BigInteger(+1, k);

            BigInteger c = new BigInteger(+1, cryptKey);

            pKey = c.ModPow(exponent, modulus).ToByteArray();

            byte[] realPlainKey = new byte[24];
            Array.Copy(pKey, pKey.Length - 16, realPlainKey, 0, 16);
            Array.Copy(pKey, pKey.Length - 16, realPlainKey, 16, 8);

            byte[] iv = new byte[8];

            // Decrypt bank message
            DesEdeEngine desedeEngine = new DesEdeEngine();
            BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(desedeEngine));

            KeyParameter keyparam = ParameterUtilities.CreateKeyParameter("DESEDE", realPlainKey);
            ParametersWithIV spec = new ParametersWithIV(keyparam, iv);

            cipher.Init(false, keyparam);

            return Encoding.GetEncoding("iso8859-1").GetString(cipher.DoFinal(cryptMessage));
        }

#endregion

    }
}
