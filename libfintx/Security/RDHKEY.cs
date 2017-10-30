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

using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace libfintx
{
    public static class RDHKEY
    {
        static string ENC_PUBLIC = string.Empty;
        static string ENC_PRIVATE = string.Empty;

        static string SIG_PUBLIC = string.Empty;
        static string SIG_PRIVATE = string.Empty;

        public static bool Create (string FilePath, string Password)
        {
            Log.Write("Creating RDH-10 key file");
            Log.Write(FilePath);

            // Encryption keys
            using (var rsa = new RSACryptoServiceProvider(1984))
            {
                try
                {
                    var enc_public = rsa.ExportParameters(false);
                    var enc_private = rsa.ExportParameters(true);

                    ENC_PUBLIC = toString(enc_public);
                    ENC_PRIVATE = toString(enc_private);
                }
                catch (Exception ex)
                {
                    Log.Write(ex.ToString());

                    return false;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;

                }
            }

            // Signing keys
            using (var rsa = new RSACryptoServiceProvider(1984))
            {
                try
                {
                    var sig_public = rsa.ExportParameters(false);
                    var sig_private = rsa.ExportParameters(true);

                    SIG_PUBLIC = toString(sig_public);
                    SIG_PRIVATE = toString(sig_private);
                }
                catch (Exception ex)
                {
                    Log.Write(ex.ToString());

                    return false;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;

                }
            }

            var encpublic = Crypt.Encrypt_PBEWithMD5AndDES(ENC_PUBLIC, Password);
            var encprivate = Crypt.Encrypt_PBEWithMD5AndDES(ENC_PRIVATE, Password);
            var sigpublic = Crypt.Encrypt_PBEWithMD5AndDES(SIG_PUBLIC, Password);
            var sigprivate = Crypt.Encrypt_PBEWithMD5AndDES(SIG_PRIVATE, Password);

            try
            {
                if (!File.Exists(FilePath))
                {
                    using (File.Create(FilePath))
                    { }

                    File.AppendAllText(FilePath, encpublic);
                    File.AppendAllText(FilePath, Environment.NewLine);
                    File.AppendAllText(FilePath, encprivate);
                    File.AppendAllText(FilePath, Environment.NewLine);
                    File.AppendAllText(FilePath, sigpublic);
                    File.AppendAllText(FilePath, Environment.NewLine);
                    File.AppendAllText(FilePath, sigprivate);
                    File.AppendAllText(FilePath, Environment.NewLine);

                    Log.Write("Creating RDH-10 key file done");

                    return true;
                }
                else
                {
                    Log.Write("RDH-10 key file already exists");

                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString());

                return false;
            }
        }

        public static bool OpenFromFile (string FilePath, string Password)
        {
            Log.Write("Open RDH-10 key file");
            Log.Write(FilePath);

            try
            {
                string[] lines = File.ReadAllLines(FilePath);

                int i = 1;

                foreach (var line in lines)
                {
                    switch (i)
                    {
                        case 1:
                            RDH_KEYSTORE.KEY_ENCRYPTION_PRIVATE = Helper.DecodeFrom64EncodingDefault(
                                Helper.Parse_String(Crypt.Decrypt_PBEWithMD5AndDES(line, Password), "<Modulus>", "</Modulus>"));
                            break;
                        case 3:
                            RDH_KEYSTORE.KEY_SIGNING_PRIVATE = Helper.DecodeFrom64EncodingDefault(
                                Helper.Parse_String(Crypt.Decrypt_PBEWithMD5AndDES(line, Password), "<Modulus>", "</Modulus>"));
                            break;
                        case 4:
                            RDH_KEYSTORE.KEY_SIGNING_PRIVATE_XML = Crypt.Decrypt_PBEWithMD5AndDES(line, Password);
                            break;
                    }

                    i++;
                }

                Log.Write("Reading RDH-10 key file done");

                return true;
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString());

                return false;
            }
        }

        private static string toString(RSAParameters key)
        {
            var writer = new StringWriter();

            var serializer = new XmlSerializer(typeof(RSAParameters));

            serializer.Serialize(writer, key);

            return writer.ToString();
        }
    }
}