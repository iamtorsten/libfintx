/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2016 - 2022 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 *  
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 3 of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software Foundation,
 *  Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 * 	
 */

using System;
using System.Text;
using System.Threading.Tasks;
using libfintx.FinTS.Data;
using libfintx.FinTS.Message;
using libfintx.FinTS.Version;
using libfintx.Globals;
using libfintx.Logger.Debug;
using libfintx.Logger.Log;

namespace libfintx.FinTS
{
    public static class INI
    {
        /// <summary>
        /// INI
        /// </summary>
        public static async Task<String> Init_INI(FinTsClient client, string hkTanSegmentId = null)
        {
            SEG sEG = new SEG();
            StringBuilder sb = new StringBuilder();

            var connectionDetails = client.ConnectionDetails;
            if (!client.Anonymous)
            {
                /// <summary>
                /// Sync
                /// </summary>
                try
                {
                    string segments;

                    /// <summary>
                    /// INI
                    /// </summary>
                    if (connectionDetails.HbciVersion == Convert.ToInt16(HBCI.v220))
                    {
                        sb = new StringBuilder();
                        sb.Append("HKIDN");
                        sb.Append(DEG.Separator);
                        sb.Append(SEG_NUM.Seg3);
                        sb.Append(DEG.Separator);
                        sb.Append("2");
                        sb.Append(sEG.Delimiter);
                        sb.Append(SEG_COUNTRY.Germany);
                        sb.Append(DEG.Separator);
                        sb.Append(connectionDetails.BlzPrimary);
                        sb.Append(sEG.Delimiter);
                        sb.Append(connectionDetails.UserIdEscaped);
                        sb.Append(sEG.Delimiter);
                        sb.Append(client.SystemId);
                        sb.Append(sEG.Delimiter);
                        sb.Append("1");
                        sb.Append(sEG.Terminator);

                        sb.Append("HKVVB");
                        sb.Append(DEG.Separator);
                        sb.Append(SEG_NUM.Seg4);
                        sb.Append(DEG.Separator);
                        sb.Append("2");
                        sb.Append(sEG.Delimiter);
                        sb.Append("0");
                        sb.Append(sEG.Delimiter);
                        sb.Append("0");
                        sb.Append(sEG.Delimiter);
                        sb.Append("0");
                        sb.Append(sEG.Delimiter);
                        sb.Append(FinTsGlobals.ProductId);
                        sb.Append(sEG.Delimiter);
                        sb.Append(FinTsGlobals.Version);
                        sb.Append(sEG.Terminator);

                        string segments_ = sb.ToString();

                        // string segments_ =
                        //    "HKIDN:" + SEG_NUM.Seg3 + ":2+" + SEG_COUNTRY.Germany + ":" + connectionDetails.BlzPrimary + "+" + connectionDetails.UserId + "+" + client.SystemId + "+1'" +
                        //    "HKVVB:" + SEG_NUM.Seg4 + ":2+0+0+0+" + FinTsGlobals.ProductId + "+" + FinTsGlobals.Version + "'";

                        segments = segments_;
                    }
                    else if (connectionDetails.HbciVersion == Convert.ToInt16(HBCI.v300))
                    {
                        sb = new StringBuilder();
                        sb.Append("HKIDN");
                        sb.Append(DEG.Separator);
                        sb.Append(SEG_NUM.Seg3);
                        sb.Append(DEG.Separator);
                        sb.Append("2");
                        sb.Append(sEG.Delimiter);
                        sb.Append(SEG_COUNTRY.Germany);
                        sb.Append(DEG.Separator);
                        sb.Append(connectionDetails.BlzPrimary);
                        sb.Append(sEG.Delimiter);
                        sb.Append(connectionDetails.UserIdEscaped);
                        sb.Append(sEG.Delimiter);
                        sb.Append(client.SystemId);
                        sb.Append(sEG.Delimiter);
                        sb.Append("1");
                        sb.Append(sEG.Terminator);

                        sb.Append("HKVVB");
                        sb.Append(DEG.Separator);
                        sb.Append(SEG_NUM.Seg4);
                        sb.Append(DEG.Separator);
                        sb.Append("3");
                        sb.Append(sEG.Delimiter);
                        sb.Append("0");
                        sb.Append(sEG.Delimiter);
                        sb.Append("0");
                        sb.Append(sEG.Delimiter);
                        sb.Append("0");
                        sb.Append(sEG.Delimiter);
                        sb.Append(FinTsGlobals.ProductId);
                        sb.Append(sEG.Delimiter);
                        sb.Append(FinTsGlobals.Version);
                        sb.Append(sEG.Terminator);

                        string segments_ = sb.ToString();

                        // string segments_ =
                        //    "HKIDN:" + SEG_NUM.Seg3 + ":2+" + SEG_COUNTRY.Germany + ":" + connectionDetails.BlzPrimary + "+" + connectionDetails.UserId + "+" + client.SystemId + "+1'" +
                        //    "HKVVB:" + SEG_NUM.Seg4 + ":3+0+0+0+" + FinTsGlobals.ProductId + "+" + FinTsGlobals.Version + "'";

                        if (client.HITANS >= 6)
                        {
                            client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg5);
                            segments_ = HKTAN.Init_HKTAN(client, segments_, hkTanSegmentId);
                        }
                        else
                        {
                            client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg4);
                        }

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

                    var message = FinTSMessage.Create(client, 1, "0", segments, client.HIRMS);
                    var response = await FinTSMessage.Send(client, message);

                    Helper.Parse_Segments(client, response);

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

                    if (connectionDetails.HbciVersion == Convert.ToInt16(HBCI.v300))
                    {
                        sb = new StringBuilder();
                        sb.Append("HKIDN");
                        sb.Append(DEG.Separator);
                        sb.Append(SEG_NUM.Seg3);
                        sb.Append(DEG.Separator);
                        sb.Append("2");
                        sb.Append(sEG.Delimiter);
                        sb.Append(SEG_COUNTRY.Germany);
                        sb.Append(DEG.Separator);
                        sb.Append(connectionDetails.BlzPrimary);
                        sb.Append(sEG.Delimiter);
                        sb.Append("9999999999");
                        sb.Append(sEG.Delimiter);
                        sb.Append("0");
                        sb.Append(sEG.Delimiter);
                        sb.Append("0");
                        sb.Append(sEG.Terminator);

                        sb.Append("HKVVB");
                        sb.Append(DEG.Separator);
                        sb.Append(SEG_NUM.Seg4);
                        sb.Append(DEG.Separator);
                        sb.Append("3");
                        sb.Append(sEG.Delimiter);
                        sb.Append("0");
                        sb.Append(sEG.Delimiter);
                        sb.Append("0");
                        sb.Append(sEG.Delimiter);
                        sb.Append("1");
                        sb.Append(sEG.Delimiter);
                        sb.Append(FinTsGlobals.ProductId);
                        sb.Append(sEG.Delimiter);
                        sb.Append(FinTsGlobals.Version);
                        sb.Append(sEG.Terminator);

                        string segments_ = sb.ToString();

                        // string segments_ =
                        //    "HKIDN:" + SEG_NUM.Seg2 + ":2+" + SEG_COUNTRY.Germany + ":" + connectionDetails.BlzPrimary + "+" + "9999999999" + "+0+0'" +
                        //    "HKVVB:" + SEG_NUM.Seg3 + ":3+0+0+1+" + FinTsGlobals.ProductId + "+" + FinTsGlobals.Version + "'";

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

                    client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg4);

                    string message = FinTsMessageAnonymous.Create(connectionDetails.HbciVersion, "1", "0", connectionDetails.Blz, connectionDetails.UserIdEscaped, connectionDetails.Pin, "0", segments, null, client.SEGNUM);
                    string response = await FinTSMessage.Send(client, message);

                    var messages = Helper.Parse_Segments(client, response);
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
                    if (connectionDetails.HbciVersion == Convert.ToInt16(HBCI.v300))
                    {
                        sb = new StringBuilder();
                        sb.Append("HKIDN");
                        sb.Append(DEG.Separator);
                        sb.Append(SEG_NUM.Seg3);
                        sb.Append(DEG.Separator);
                        sb.Append("2");
                        sb.Append(sEG.Delimiter);
                        sb.Append(SEG_COUNTRY.Germany);
                        sb.Append(DEG.Separator);
                        sb.Append(connectionDetails.BlzPrimary);
                        sb.Append(sEG.Delimiter);
                        sb.Append(connectionDetails.UserIdEscaped);
                        sb.Append(sEG.Delimiter);
                        sb.Append(client.SystemId);
                        sb.Append(sEG.Delimiter);
                        sb.Append("1");
                        sb.Append(sEG.Terminator);

                        sb.Append("HKVVB");
                        sb.Append(DEG.Separator);
                        sb.Append(SEG_NUM.Seg4);
                        sb.Append(DEG.Separator);
                        sb.Append("3");
                        sb.Append(sEG.Delimiter);
                        sb.Append("0");
                        sb.Append(sEG.Delimiter);
                        sb.Append("0");
                        sb.Append(sEG.Delimiter);
                        sb.Append("0");
                        sb.Append(sEG.Delimiter);
                        sb.Append(FinTsGlobals.ProductId);
                        sb.Append(sEG.Delimiter);
                        sb.Append(FinTsGlobals.Version);
                        sb.Append(sEG.Terminator);

                        sb.Append("HKSYN");
                        sb.Append(DEG.Separator);
                        sb.Append(SEG_NUM.Seg4);
                        sb.Append(DEG.Separator);
                        sb.Append("3");
                        sb.Append(sEG.Delimiter);
                        sb.Append("0");
                        sb.Append(sEG.Terminator);

                        string segments__ = sb.ToString();

                        // string segments__ =
                        //    "HKIDN:" + SEG_NUM.Seg3 + ":2+" + SEG_COUNTRY.Germany + ":" + connectionDetails.BlzPrimary + "+" + connectionDetails.UserId + "+" + client.SystemId + "+1'" +
                        //    "HKVVB:" + SEG_NUM.Seg4 + ":3+0+0+0+" + FinTsGlobals.ProductId + "+" + FinTsGlobals.Version + "'" +
                        //    "HKSYN:" + SEG_NUM.Seg5 + ":3+0'";

                        segments = segments__;
                    }
                    else
                    {
                        Log.Write("HBCI version not supported");

                        throw new Exception("HBCI version not supported");
                    }

                    client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg5);

                    message = FinTSMessage.Create(client, 1, "0", segments, client.HIRMS);
                    response = await FinTSMessage.Send(client, message);

                    Helper.Parse_Segments(client, response);

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
