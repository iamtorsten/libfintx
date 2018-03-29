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

using System;

namespace libfintx
{
    public static class INI
    {
        /// <summary>
        /// INI
        /// </summary>
        public static bool Init_INI(int BLZ, string URL, int HBCIVersion, string UserID, string PIN, bool Anonymous)
        {
            if (!Anonymous)
            {
                /// <summary>
                /// Sync
                /// </summary>
                try
                {
                    Log.Write("Starting Synchronisation");

                    string segments;

                    if (HBCIVersion == 220)
                    {
                        string segments_ = "HKIDN:" + SEGNUM.SETVal(3) + ":2+280:" + BLZ + "+" + UserID + "+0+1'" +
                            "HKVVB:" + SEGNUM.SETVal(4) + ":2+0+0+0+" + Program.Buildname + "+" + Program.Version + "'" +
                            "HKSYN:" + SEGNUM.SETVal(5) + ":2+0'";

                        segments = segments_;
                    }
                    else if (HBCIVersion == 300)
                    {
                        string segments_ = "HKIDN:" + SEGNUM.SETVal(3) + ":2+280:" + BLZ + "+" + UserID + "+0+1'" +
                            "HKVVB:" + SEGNUM.SETVal(4) + ":3+0+0+0+" + Program.Buildname + "+" + Program.Version + "'" +
                            "HKSYN:" + SEGNUM.SETVal(5) + ":3+0'";

                        segments = segments_;
                    }
                    else
                    {
                        UserID = string.Empty;
                        PIN = null;

                        Log.Write("HBCI version not supported");

                        throw new Exception("HBCI version not supported");
                    }

                    SEG.NUM = SEGNUM.SETInt(5);

                    if (Helper.Parse_Segment(UserID, BLZ, HBCIVersion,
                        FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, MSG.SETVal(1), DLG.SETVal(0), BLZ, UserID,
                        PIN, SYS.SETVal(0), segments, null, SEG.NUM))))
                    {
                        // Sync OK
                        Log.Write("Synchronisation ok");

                        /// <summary>
                        /// INI
                        /// </summary>
                        if (HBCIVersion == 220)
                        {
                            string segments_ = "HKIDN:" + SEGNUM.SETVal(3) + ":2+280:" + BLZ + "+" + UserID + "+" + Segment.HISYN + "+1'" +
                                "HKVVB:" + SEGNUM.SETVal(4) + ":2+0+0+0+" + Program.Buildname + "+" + Program.Version + "'";

                            segments = segments_;
                        }
                        else if (HBCIVersion == 300)
                        {
                            string segments_ = "HKIDN:" + SEGNUM.SETVal(3) + ":2+280:" + BLZ + "+" + UserID + "+" + Segment.HISYN + "+1'" +
                                "HKVVB:" + SEGNUM.SETVal(4) + ":3+0+0+0+" + Program.Buildname + "+" + Program.Version + "'";

                            segments = segments_;
                        }
                        else
                        {
                            UserID = string.Empty;
                            PIN = null;

                            Log.Write("HBCI version not supported");

                            throw new Exception("HBCI version not supported");
                        }

                        SEG.NUM = SEGNUM.SETInt(4);

                        if (Helper.Parse_Segment(UserID, BLZ, HBCIVersion,
                            FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, MSG.SETVal(1), DLG.SETVal(0), BLZ, UserID, PIN, Segment.HISYN,
                            segments, Segment.HIRMS, SEG.NUM))))
                            return true;
                        else
                        {
                            UserID = string.Empty;
                            PIN = null;

                            Log.Write("Initialisation failed");

                            throw new Exception("Initialisation failed");
                        }
                    }
                    else
                    {
                        UserID = string.Empty;
                        PIN = null;

                        Log.Write("Sync failed");

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    UserID = string.Empty;
                    PIN = null;

                    Log.Write(ex.ToString());

                    throw new Exception("Software error");
                }
            }
            else
            {
                /// <summary>
                /// Sync
                /// </summary>
                try
                {
                    Log.Write("Starting Synchronisation anonymous");

                    string segments;

                    if (HBCIVersion == 300)
                    {
                        string segments_ = "HKIDN:" + SEGNUM.SETVal(2) + ":2+280:" + BLZ + "+" + "9999999999" + "+0+0'" +
                                    "HKVVB:" + SEGNUM.SETVal(3) + ":3+0+0+1+" + Program.Buildname + "+" + Program.Version + "'";

                        segments = segments_;
                    }
                    else
                    {
                        UserID = string.Empty;
                        PIN = null;

                        Log.Write("HBCI version not supported");

                        throw new Exception("HBCI version not supported");
                    }

                    SEG.NUM = SEGNUM.SETInt(4);

                    if (Helper.Parse_Segment(UserID, BLZ, HBCIVersion,
                        FinTSMessage.Send(URL, FinTSMessageAnonymous.Create(HBCIVersion, MSG.SETVal(1), DLG.SETVal(0), BLZ,
                        UserID, PIN, SYS.SETVal(0), segments, null, SEG.NUM))))
                    {
                        // Sync OK
                        Log.Write("Synchronisation anonymous ok");

                        /// <summary>
                        /// INI
                        /// </summary>
                        if (HBCIVersion == 300)
                        {
                            string segments__ = "HKIDN:" + SEGNUM.SETVal(3) + ":2+280:" + BLZ + "+" + UserID + "+" + Segment.HISYN + "+1'" +
                                "HKVVB:" + SEGNUM.SETVal(4) + ":3+0+0+0+" + Program.Buildname + "+" + Program.Version + "'" +
                                "HKSYN:" + SEGNUM.SETVal(5) + ":3+0'";

                            segments = segments__;
                        }
                        else
                        {
                            UserID = string.Empty;
                            PIN = null;

                            Log.Write("HBCI version not supported");

                            throw new Exception("HBCI version not supported");
                        }

                        SEG.NUM = SEGNUM.SETInt(5);

                        if (Helper.Parse_Segment(UserID, BLZ, HBCIVersion,
                            FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, MSG.SETVal(1), DLG.SETVal(0), BLZ, UserID, PIN,
                            Segment.HISYN, segments, Segment.HIRMS, SEG.NUM))))
                        {
                            return true;
                        }
                        else
                        {
                            UserID = string.Empty;
                            PIN = null;

                            Log.Write("Initialisation failed");

                            throw new Exception("Initialisation failed");
                        }
                    }
                    else
                    {
                        UserID = string.Empty;
                        PIN = null;

                        Log.Write("Sync failed");

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    UserID = string.Empty;
                    PIN = null;

                    Log.Write(ex.ToString());

                    DEBUG.Write("Software error: " + ex.ToString());

                    throw new Exception("Software error: " + ex.ToString());
                }
            }
        }
    }
}
