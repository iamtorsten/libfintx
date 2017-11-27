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

        public static string RDHKEYFILE { get; set; }
        public static string RDHKEYFILEPWD { get; set; }

        public static bool Create (string FilePath, string Password, int BLZ, string UserID, int Country, int ProfileVersion)
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

                    // Load keys into store
                    RDH_KEYSTORE.KEY_ENCRYPTION_PRIVATE = ENC_PUBLIC;
                    RDH_KEYSTORE.KEY_ENCRYPTION_PRIVATE_XML = ENC_PRIVATE;
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

                    // Load keys into store
                    RDH_KEYSTORE.KEY_SIGNING_PRIVATE = SIG_PUBLIC;
                    RDH_KEYSTORE.KEY_SIGNING_PRIVATE_XML = SIG_PRIVATE;
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

            var encpublic = RDHXPassport.Encrypt_PBEWithMD5AndDES(ENC_PUBLIC, Password);
            var encprivate = RDHXPassport.Encrypt_PBEWithMD5AndDES(ENC_PRIVATE, Password);
            var sigpublic = RDHXPassport.Encrypt_PBEWithMD5AndDES(SIG_PUBLIC, Password);
            var sigprivate = RDHXPassport.Encrypt_PBEWithMD5AndDES(SIG_PRIVATE, Password);
            var blz = RDHXPassport.Encrypt_PBEWithMD5AndDES(Convert.ToString(BLZ), Password);
            var userid = RDHXPassport.Encrypt_PBEWithMD5AndDES(UserID, Password);
            var country = RDHXPassport.Encrypt_PBEWithMD5AndDES(Convert.ToString(Country), Password);
            var profileversion = RDHXPassport.Encrypt_PBEWithMD5AndDES(Convert.ToString(ProfileVersion), Password);

            try
            {
                if (!File.Exists(FilePath))
                {
                    using (File.Create(FilePath))
                    { }

                    // Create hbci key

                    File.AppendAllText(FilePath, encpublic);
                    File.AppendAllText(FilePath, Environment.NewLine);
                    File.AppendAllText(FilePath, encprivate);
                    File.AppendAllText(FilePath, Environment.NewLine);
                    File.AppendAllText(FilePath, sigpublic);
                    File.AppendAllText(FilePath, Environment.NewLine);
                    File.AppendAllText(FilePath, sigprivate);
                    File.AppendAllText(FilePath, Environment.NewLine);
                    File.AppendAllText(FilePath, blz);
                    File.AppendAllText(FilePath, Environment.NewLine);
                    File.AppendAllText(FilePath, userid);
                    File.AppendAllText(FilePath, Environment.NewLine);
                    File.AppendAllText(FilePath, country);
                    File.AppendAllText(FilePath, Environment.NewLine);
                    File.AppendAllText(FilePath, profileversion);
                    File.AppendAllText(FilePath, Environment.NewLine);
                    File.AppendAllText(FilePath, Environment.NewLine); // --> Public bank enc key
                    File.AppendAllText(FilePath, Environment.NewLine);
                    File.AppendAllText(FilePath, Environment.NewLine); // --> Public bank sig key

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
                                Helper.Parse_String(RDHXPassport.Decrypt_RSA_PBEWithMD5AndDES(line, Password), "<Modulus>", "</Modulus>"));
                            break;
                        case 2:
                            RDH_KEYSTORE.KEY_ENCRYPTION_PRIVATE_XML = RDHXPassport.Decrypt_RSA_PBEWithMD5AndDES(line, Password);
                            break;
                        case 3:
                            RDH_KEYSTORE.KEY_SIGNING_PRIVATE = Helper.DecodeFrom64EncodingDefault(
                                Helper.Parse_String(RDHXPassport.Decrypt_RSA_PBEWithMD5AndDES(line, Password), "<Modulus>", "</Modulus>"));
                            break;
                        case 4:
                            RDH_KEYSTORE.KEY_SIGNING_PRIVATE_XML = RDHXPassport.Decrypt_RSA_PBEWithMD5AndDES(line, Password);
                            break;
                        case 5:
                            RDH_KEYSTORE.BLZ = RDHXPassport.Decrypt_PBEWithMD5AndDES(line, Password);
                            break;
                        case 6:
                            RDH_KEYSTORE.UserID = RDHXPassport.Decrypt_PBEWithMD5AndDES(line, Password);
                            break;
                        case 7:
                            RDH_KEYSTORE.Country = RDHXPassport.Decrypt_PBEWithMD5AndDES(line, Password);
                            break;
                        case 8:
                            RDH_KEYSTORE.ProfileVersion = RDHXPassport.Decrypt_PBEWithMD5AndDES(line, Password);
                            break;
                        case 9:
                            RDH_KEYSTORE.KEY_ENCRYPTION_PUBLIC_BANK = RDHXPassport.Decrypt_PBEWithMD5AndDES(line, Password);
                            break;
                        case 10:
                            RDH_KEYSTORE.KEY_SIGNING_PUBLIC_BANK = RDHXPassport.Decrypt_PBEWithMD5AndDES(line, Password);
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

        public static void Update(string FilePath, string Password)
        {
            Log.Write("Open RDH-10 key file for update");
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
                            if (String.IsNullOrEmpty(RDH_KEYSTORE.KEY_ENCRYPTION_PRIVATE))
                                RDH_KEYSTORE.KEY_ENCRYPTION_PRIVATE = Helper.DecodeFrom64EncodingDefault(
                                    Helper.Parse_String(RDHXPassport.Decrypt_RSA_PBEWithMD5AndDES(line, Password), "<Modulus>", "</Modulus>"));
                            break;
                        case 2:
                            if (String.IsNullOrEmpty(RDH_KEYSTORE.KEY_ENCRYPTION_PRIVATE_XML))
                                RDH_KEYSTORE.KEY_ENCRYPTION_PRIVATE_XML = RDHXPassport.Decrypt_RSA_PBEWithMD5AndDES(line, Password);
                            break;
                        case 3:
                            if (String.IsNullOrEmpty(RDH_KEYSTORE.KEY_SIGNING_PRIVATE))
                                RDH_KEYSTORE.KEY_SIGNING_PRIVATE = Helper.DecodeFrom64EncodingDefault(
                                    Helper.Parse_String(RDHXPassport.Decrypt_RSA_PBEWithMD5AndDES(line, Password), "<Modulus>", "</Modulus>"));
                            break;
                        case 4:
                            if (String.IsNullOrEmpty(RDH_KEYSTORE.KEY_SIGNING_PRIVATE_XML))
                                RDH_KEYSTORE.KEY_SIGNING_PRIVATE_XML = RDHXPassport.Decrypt_RSA_PBEWithMD5AndDES(line, Password);
                            break;
                        case 5:
                            if (String.IsNullOrEmpty(RDH_KEYSTORE.BLZ))
                                RDH_KEYSTORE.BLZ = RDHXPassport.Decrypt_PBEWithMD5AndDES(line, Password);
                            break;
                        case 6:
                            if (String.IsNullOrEmpty(RDH_KEYSTORE.UserID))
                                RDH_KEYSTORE.UserID = RDHXPassport.Decrypt_PBEWithMD5AndDES(line, Password);
                            break;
                        case 7:
                            if (String.IsNullOrEmpty(RDH_KEYSTORE.Country))
                                RDH_KEYSTORE.Country = RDHXPassport.Decrypt_PBEWithMD5AndDES(line, Password);
                            break;
                        case 8:
                            if (String.IsNullOrEmpty(RDH_KEYSTORE.ProfileVersion))
                                RDH_KEYSTORE.ProfileVersion = RDHXPassport.Decrypt_PBEWithMD5AndDES(line, Password);
                            break;
                        case 9:
                            if (String.IsNullOrEmpty(RDH_KEYSTORE.KEY_ENCRYPTION_PUBLIC_BANK))
                                RDH_KEYSTORE.KEY_ENCRYPTION_PUBLIC_BANK = RDHXPassport.Decrypt_PBEWithMD5AndDES(line, Password);
                            break;
                        case 10:
                            if (String.IsNullOrEmpty(RDH_KEYSTORE.KEY_SIGNING_PUBLIC_BANK))
                                RDH_KEYSTORE.KEY_SIGNING_PUBLIC_BANK = RDHXPassport.Decrypt_PBEWithMD5AndDES(line, Password);
                            break;
                    }

                    i++;
                }

                var encpublic = RDHXPassport.Encrypt_RSA_PBEWithMD5AndDES(RDH_KEYSTORE.KEY_ENCRYPTION_PRIVATE, Password);
                var encprivate = RDHXPassport.Encrypt_RSA_PBEWithMD5AndDES(RDH_KEYSTORE.KEY_ENCRYPTION_PRIVATE_XML, Password);
                var sigpublic = RDHXPassport.Encrypt_RSA_PBEWithMD5AndDES(RDH_KEYSTORE.KEY_SIGNING_PRIVATE, Password);
                var sigprivate = RDHXPassport.Encrypt_RSA_PBEWithMD5AndDES(RDH_KEYSTORE.KEY_SIGNING_PRIVATE_XML, Password);
                var blz = RDHXPassport.Encrypt_PBEWithMD5AndDES(RDH_KEYSTORE.BLZ, Password);
                var userid = RDHXPassport.Encrypt_PBEWithMD5AndDES(RDH_KEYSTORE.UserID, Password);
                var country = RDHXPassport.Encrypt_PBEWithMD5AndDES(RDH_KEYSTORE.Country, Password);
                var profileversion = RDHXPassport.Encrypt_PBEWithMD5AndDES(RDH_KEYSTORE.ProfileVersion, Password);
                var encpubbank = RDHXPassport.Encrypt_PBEWithMD5AndDES(RDH_KEYSTORE.KEY_ENCRYPTION_PUBLIC_BANK, Password);
                var sigpubbank = RDHXPassport.Encrypt_PBEWithMD5AndDES(RDH_KEYSTORE.KEY_SIGNING_PUBLIC_BANK, Password);

                // Clear content
                FileStream fileStream = File.Open(FilePath, FileMode.Open);
                fileStream.SetLength(0);
                fileStream.Close();

                File.AppendAllText(FilePath, encpublic);
                File.AppendAllText(FilePath, Environment.NewLine);
                File.AppendAllText(FilePath, encprivate);
                File.AppendAllText(FilePath, Environment.NewLine);
                File.AppendAllText(FilePath, sigpublic);
                File.AppendAllText(FilePath, Environment.NewLine);
                File.AppendAllText(FilePath, sigprivate);
                File.AppendAllText(FilePath, Environment.NewLine);
                File.AppendAllText(FilePath, blz);
                File.AppendAllText(FilePath, Environment.NewLine);
                File.AppendAllText(FilePath, userid);
                File.AppendAllText(FilePath, Environment.NewLine);
                File.AppendAllText(FilePath, country);
                File.AppendAllText(FilePath, Environment.NewLine);
                File.AppendAllText(FilePath, profileversion);
                File.AppendAllText(FilePath, Environment.NewLine);
                File.AppendAllText(FilePath, encpubbank);
                File.AppendAllText(FilePath, Environment.NewLine);
                File.AppendAllText(FilePath, sigpubbank);

                Log.Write("Updating RDH-10 key file done");
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString());
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