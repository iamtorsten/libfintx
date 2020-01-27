/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2020 Torsten Klinger
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
        public static string Init_INI(FinTsClient client)
        {
            var connectionDetails = client.ConnectionDetails;
            if (!client.Anonymous)
            {
                /// <summary>
                /// Sync
                /// </summary>
                try
                {
                    string segments;

                    SEG.NUM = SEGNUM.SETInt(5);

                    /// <summary>
                    /// INI
                    /// </summary>
                    if (connectionDetails.HbciVersion == 220)
                    {
                        string segments_ =
                            "HKIDN:" + SEGNUM.SETVal(3) + ":2+280:" + connectionDetails.BlzPrimary + "+" + connectionDetails.UserId + "+" + client.SystemId + "+1'" +
                            "HKVVB:" + SEGNUM.SETVal(4) + ":2+0+0+0+" + Program.ProductId + "+" + Program.Version + "'";

                        segments = segments_;
                    }
                    else if (connectionDetails.HbciVersion == 300)
                    {
                        string segments_ =
                            "HKIDN:" + SEGNUM.SETVal(3) + ":2+280:" + connectionDetails.BlzPrimary + "+" + connectionDetails.UserId + "+" + client.SystemId + "+1'" +
                            "HKVVB:" + SEGNUM.SETVal(4) + ":3+0+0+0+" + Program.ProductId + "+" + Program.Version + "'";

                        if (client.HITANS != null && client.HITANS.Substring(0, 3).Equals("6+4"))
                            segments_ = HKTAN.Init_HKTAN(client, segments_);

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

                    var message = FinTSMessage.Create(connectionDetails.HbciVersion, "1", "0", connectionDetails.BlzPrimary, connectionDetails.UserId, connectionDetails.Pin, client.SystemId, segments, client.HIRMS, SEG.NUM);
                    var response = FinTSMessage.Send(connectionDetails.Url, message);

                    Helper.Parse_Segment(client, response);

                    client.HITAN = Helper.Parse_String(Helper.Parse_String(response, "HITAN:", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

                    return response;
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

                    if (connectionDetails.HbciVersion == 300)
                    {
                        string segments_ =
                            "HKIDN:" + SEGNUM.SETVal(2) + ":2+280:" + connectionDetails.BlzPrimary + "+" + "9999999999" + "+0+0'" +
                            "HKVVB:" + SEGNUM.SETVal(3) + ":3+0+0+1+" + Program.ProductId + "+" + Program.Version + "'";

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

                    string message = FinTsMessageAnonymous.Create(connectionDetails.HbciVersion, "1", "0", connectionDetails.Blz, connectionDetails.UserId, connectionDetails.Pin, "0", segments, null, SEG.NUM);
                    string response = FinTSMessage.Send(connectionDetails.Url, message);

                    var messages = Helper.Parse_Segment(client, response);
                    var result = new HBCIDialogResult(messages, response);
                    if (!result.IsSuccess)
                    {
                        Log.Write("Synchronisation anonymous failed. " + result);
                        return response;
                    }

                    // Sync OK
                    Log.Write("Synchronisation anonymous ok");

                    /// <summary>
                    /// INI
                    /// </summary>
                    if (connectionDetails.HbciVersion == 300)
                    {
                        string segments__ =
                            "HKIDN:" + SEGNUM.SETVal(3) + ":2+280:" + connectionDetails.BlzPrimary + "+" + connectionDetails.UserId + "+" + client.SystemId + "+1'" +
                            "HKVVB:" + SEGNUM.SETVal(4) + ":3+0+0+0+" + Program.ProductId + "+" + Program.Version + "'" +
                            "HKSYN:" + SEGNUM.SETVal(5) + ":3+0'";

                        segments = segments__;
                    }
                    else
                    {
                        Log.Write("HBCI version not supported");

                        throw new Exception("HBCI version not supported");
                    }

                    SEG.NUM = SEGNUM.SETInt(5);

                    message = FinTSMessage.Create(connectionDetails.HbciVersion, "1", "0", connectionDetails.BlzPrimary, connectionDetails.UserId, connectionDetails.Pin, client.SystemId, segments, client.HIRMS, SEG.NUM);
                    response = FinTSMessage.Send(connectionDetails.Url, message);

                    Helper.Parse_Segment(client, response);

                    client.HITAN = Helper.Parse_String(Helper.Parse_String(response, "HITAN:", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

                    return response;
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
