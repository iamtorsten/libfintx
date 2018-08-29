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

using libfintx.Data;
using System;
using System.Collections.Generic;

namespace libfintx
{
    public static class INI
    {
        /// <summary>
        /// INI
        /// </summary>
        public static HBCIDialogResult Init_INI(ConnectionDetails connectionDetails, bool anonymous)
        {
            if (!anonymous)
            {
                /// <summary>
                /// Sync
                /// </summary>
                try
                {
                    // If there is no customer system ID available, perform sync to get one
                    if (connectionDetails.CustomerSystemId == null)
                    {
                        var BankCode = Transaction.HKSYN(connectionDetails);
                        var syncResult = new HBCIDialogResult(Helper.Parse_BankCode(BankCode));
                        if (!syncResult.IsSuccess)
                        {
                            Log.Write("Synchronisation failed.");
                            return syncResult;
                        }
                    }
                    else
                    {
                        Segment.HISYN = connectionDetails.CustomerSystemId;
                    }

                    string segments;

                    /// <summary>
                    /// INI
                    /// </summary>
                    if (connectionDetails.HBCIVersion == 220)
                    {
                        string segments_ = "HKIDN:" + SEGNUM.SETVal(3) + ":2+280:" + connectionDetails.Blz + "+" + connectionDetails.UserId + "+" + Segment.HISYN + "+1'" +
                            "HKVVB:" + SEGNUM.SETVal(4) + ":2+0+0+0+" + Program.Buildname + "+" + Program.Version + "'";

                        segments = segments_;
                    }
                    else if (connectionDetails.HBCIVersion == 300)
                    {
                        string segments_ = "HKIDN:" + SEGNUM.SETVal(3) + ":2+280:" + connectionDetails.Blz + "+" + connectionDetails.UserId + "+" + Segment.HISYN + "+1'" +
                            "HKVVB:" + SEGNUM.SETVal(4) + ":3+0+0+0+" + Program.Buildname + "+" + Program.Version + "'";

                        segments = segments_;
                    }
                    else
                    {
                        //Since connectionDetails is a re-usable object, this shouldn't be cleared.
                        //connectionDetails.UserId = string.Empty;
                        //connectionDetails.Pin = null;

                        Log.Write("HBCI version not supported");

                        throw new Exception("HBCI version not supported");
                    }

                    SEG.NUM = SEGNUM.SETInt(4);

                    var message = FinTSMessage.Create(connectionDetails.HBCIVersion, MSG.SETVal(1), DLG.SETVal(0), connectionDetails.Blz, connectionDetails.UserId, connectionDetails.Pin, Segment.HISYN, segments, Segment.HIRMS, SEG.NUM);
                    var response = FinTSMessage.Send(connectionDetails.Url, message);

                    var bankMessages = Helper.Parse_Segment(connectionDetails.UserId, connectionDetails.Blz, connectionDetails.HBCIVersion, response);
                    var result = new HBCIDialogResult(bankMessages);
                    if (!result.IsSuccess)
                        Log.Write("Initialisation failed: " + result);

                    return result;
                }
                catch (Exception ex)
                {
                    //Since connectionDetails is a re-usable object, this shouldn't be cleared.
                    //connectionDetails.UserId = string.Empty;
                    //connectionDetails.Pin = null;

                    Log.Write(ex.ToString());

                    throw new Exception("Software error", ex);
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

                    if (connectionDetails.HBCIVersion == 300)
                    {
                        string segments_ = "HKIDN:" + SEGNUM.SETVal(2) + ":2+280:" + connectionDetails.Blz + "+" + "9999999999" + "+0+0'" +
                                    "HKVVB:" + SEGNUM.SETVal(3) + ":3+0+0+1+" + Program.Buildname + "+" + Program.Version + "'";

                        segments = segments_;
                    }
                    else
                    {
                        //Since connectionDetails is a re-usable object, this shouldn't be cleared.
                        //connectionDetails.UserId = string.Empty;
                        //connectionDetails.Pin = null;

                        Log.Write("HBCI version not supported");

                        throw new Exception("HBCI version not supported");
                    }

                    SEG.NUM = SEGNUM.SETInt(4);

                    string message = FinTSMessageAnonymous.Create(connectionDetails.HBCIVersion, MSG.SETVal(1), DLG.SETVal(0), connectionDetails.Blz, connectionDetails.UserId, connectionDetails.Pin, SYS.SETVal(0), segments, null, SEG.NUM);
                    string response = FinTSMessage.Send(connectionDetails.Url, message);

                    var messages = Helper.Parse_Segment(connectionDetails.UserId, connectionDetails.Blz, connectionDetails.HBCIVersion, response);
                    var result = new HBCIDialogResult(messages);
                    if (!result.IsSuccess)
                    {
                        Log.Write("Synchronisation anonymous failed. " + result);
                        return result;
                    }

                    // Sync OK
                    Log.Write("Synchronisation anonymous ok");

                    /// <summary>
                    /// INI
                    /// </summary>
                    if (connectionDetails.HBCIVersion == 300)
                    {
                        string segments__ = "HKIDN:" + SEGNUM.SETVal(3) + ":2+280:" + connectionDetails.Blz + "+" + connectionDetails.UserId + "+" + Segment.HISYN + "+1'" +
                            "HKVVB:" + SEGNUM.SETVal(4) + ":3+0+0+0+" + Program.Buildname + "+" + Program.Version + "'" +
                            "HKSYN:" + SEGNUM.SETVal(5) + ":3+0'";

                        segments = segments__;
                    }
                    else
                    {
                        //Since connectionDetails is a re-usable object, this shouldn't be cleared.
                        //connectionDetails.UserId = string.Empty;
                        //connectionDetails.Pin = null;

                        Log.Write("HBCI version not supported");

                        throw new Exception("HBCI version not supported");
                    }

                    SEG.NUM = SEGNUM.SETInt(5);

                    message = FinTSMessage.Create(connectionDetails.HBCIVersion, MSG.SETVal(1), DLG.SETVal(0), connectionDetails.Blz, connectionDetails.UserId, connectionDetails.Pin, Segment.HISYN, segments, Segment.HIRMS, SEG.NUM);
                    response = FinTSMessage.Send(connectionDetails.Url, message);

                    messages = Helper.Parse_Segment(connectionDetails.UserId, connectionDetails.Blz, connectionDetails.HBCIVersion, response);
                    result = new HBCIDialogResult(messages);
                    if (!result.IsSuccess)
                        Log.Write("Initialisation failed.");

                    return result;
                }
                catch (Exception ex)
                {
                    //Since connectionDetails is a re-usable object, this shouldn't be cleared.
                    //connectionDetails.UserId = string.Empty;
                    //connectionDetails.Pin = null;

                    Log.Write(ex.ToString());

                    DEBUG.Write("Software error: " + ex.ToString());

                    throw new Exception("Software error: " + ex.ToString());
                }
            }
        }
    }
}
