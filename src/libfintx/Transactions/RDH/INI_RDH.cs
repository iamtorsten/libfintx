/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2018 Torsten Klinger
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

namespace libfintx
{
    public static class INI_RDH
    {
        /// <summary>
        /// INI RDH
        /// </summary>
        public static bool Init_INI_RDH(int BLZ, string URL, int Port, int HBCIVersion, string UserID, string FilePath, string Password)
        {
            Log.Write("Starting Synchronisation");

            try
            {
                string segments;

                // Get public keys from bank
                if (HBCIVersion == 300)
                {
                    string segments_ = "HKIDN:" + SEGNUM.SETVal(2) + ":2+" + SEG_Country.Germany + ":" + BLZ + "+" + "9999999999" + "+0+0'" +
                        "HKVVB:" + SEGNUM.SETVal(3) + ":3+11+0+1+" + Program.ProductId + "+" + Program.Version + "'";

                    segments = segments_;

                    if (DEBUG.Enabled)
                        DEBUG.Write("INI message: " + segments);

                    if (Helper.Parse_Segment_RDH_Key(RDHMessage.Send(URL, Port, RDHMessageAnonymous.Create(HBCIVersion, "1", "0", BLZ, segments)), BLZ, UserID))
                    {
                        // Sync OK
                        Log.Write("Synchronisation ok");

                        if (RDHKEY.OpenFromFile(FilePath, Password))
                        {
                            segments_ = "HKIDN:" + SEGNUM.SETVal(3) + ":2+" + SEG_Country.Germany + ":" + BLZ + "+" + UserID + "+0+1'" +
                            "HKSAK:" + SEGNUM.SETVal(4) + ":3+2+112+" + RDH_Profile.RDHPROFILE + "+" + SEG_Country.Germany +
                            ":" + BLZ + ":" + UserID + ":" + Keytype.Enc + ":" + RDH_Profile.Version + ":1+5:2:10:@" + RDH_KEYSTORE.KEY_ENCRYPTION_PRIVATE.Length + "@" +
                            RDH_KEYSTORE.KEY_ENCRYPTION_PRIVATE + ":12:@3@" + Converter.FromHexString("01 00 01") + ":13'" +
                            "HKSAK:" + SEGNUM.SETVal(5) + ":3+2+112+" + RDH_Profile.RDHPROFILE + "+" + SEG_Country.Germany +
                            ":" + BLZ + ":" + UserID + ":" + Keytype.Sig + ":" + RDH_Profile.Version + ":1+6:19:10:@" + RDH_KEYSTORE.KEY_SIGNING_PRIVATE.Length + "@" +
                            RDH_KEYSTORE.KEY_SIGNING_PRIVATE + ":12:@3@" + Converter.FromHexString("01 00 01") + ":13'";

                            segments = segments_;

                            if (DEBUG.Enabled)
                                DEBUG.Write("Key exchange message: " + segments);

                            RDHMessage.Send(URL, Port, RDHMessage.Create(HBCIVersion, "1", "0", BLZ, UserID, "0", segments, SEGNUM.SETInt(5)));

                            // INI OK
                            Log.Write("INI ok");

                            return true;
                        }
                        else
                        {
                            Log.Write("INI failed");

                            return false;
                        }
                    }
                    else
                    {
                        UserID = string.Empty;

                        Log.Write("Initialisation failed");

                        throw new Exception(HBCI_Exception.INI());
                    }
                }
                else
                {
                    Log.Write("HBCI version not supported");

                    throw new Exception(HBCI_Exception.HBCIVERSIONNOTSUPPORTED());
                }
            }
            catch (Exception ex)
            {
                UserID = string.Empty;

                Log.Write(ex.ToString());

                if (DEBUG.Enabled)
                    DEBUG.Write("Software error: " + ex.ToString());

                // Exception
                if (!DEBUG.Enabled)
                    throw new Exception(HBCI_Exception.SOFTWARE() + ex.ToString());
                else
                    return false;
            }
        }
    }
}