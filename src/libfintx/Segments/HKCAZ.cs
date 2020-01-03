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

using libfintx.Camt;
using libfintx.Data;
using System;

namespace libfintx
{
    public static class HKCAZ
    {
        /// <summary>
        /// Transactions in camt053 format
        /// </summary>
        public static string Init_HKCAZ(ConnectionDetails connectionDetails, string FromDate, string ToDate, string Startpoint, CamtVersion camtVers)
        {
            string segments = string.Empty;

            SEG.NUM = SEGNUM.SETInt(3);

            switch (camtVers)
            {
                case CamtVersion.Camt052:
                    Log.Write("Starting job HKCAZ: Request transactions in camt052 format");

                    if (String.IsNullOrEmpty(FromDate))
                    {
                        if (String.IsNullOrEmpty(Startpoint))
                        {
                            segments = "HKCAZ:" + SEG.NUM + ":" + Segment.HKCAZ + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+" + CamtScheme.Camt052 + "+N'";
                        }
                        else
                        {
                            segments = "HKCAZ:" + SEG.NUM + ":" + Segment.HKCAZ + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+" + CamtScheme.Camt052 + "+N++++" + Startpoint + "'";
                        }
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(Startpoint))
                        {
                            segments = "HKCAZ:" + SEG.NUM + ":" + Segment.HKCAZ + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+" + CamtScheme.Camt052 + "+N+" + FromDate + "+" + ToDate + "'";
                        }
                        else
                        {
                            segments = "HKCAZ:" + SEG.NUM + ":" + Segment.HKCAZ + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+" + CamtScheme.Camt052 + "+N+" + FromDate + "+" + ToDate + "++" + Startpoint + "'";
                        }
                    }

                    SEG.NUM = SEGNUM.SETInt(3);
                    break;

                case CamtVersion.Camt053:
                    Log.Write("Starting job HKCAZ: Request transactions in camt053 format");

                    if (String.IsNullOrEmpty(FromDate))
                    {
                        if (String.IsNullOrEmpty(Startpoint))
                        {
                            segments = "HKCAZ:" + SEG.NUM + ":" + Segment.HKCAZ + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+" + CamtScheme.Camt053 + "+N'";
                        }
                        else
                        {
                            segments = "HKCAZ:" + SEG.NUM + ":" + Segment.HKCAZ + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+" + CamtScheme.Camt053 + "+N++++" + Startpoint + "'";
                        }
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(Startpoint))
                        {
                            segments = "HKCAZ:" + SEG.NUM + ":" + Segment.HKCAZ + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+" + CamtScheme.Camt053 + "+N+" + FromDate + "+" + ToDate + "'";
                        }
                        else
                        {
                            segments = "HKCAZ:" + SEG.NUM + ":" + Segment.HKCAZ + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+" + CamtScheme.Camt053 + "+N+" + FromDate + "+" + ToDate + "++" + Startpoint + "'";
                        }
                    }

                    break;

                default: // -> Will never happen, only for compiler
                    return string.Empty;
            }

            if (Helper.IsTANRequired("HKCAZ"))
            {
                SEG.NUM = SEGNUM.SETInt(4);
                segments = HKTAN.Init_HKTAN(segments);
            }

            string message = FinTSMessage.Create(connectionDetails.HbciVersion, Segment.HNHBS, Segment.HNHBK, connectionDetails.BlzPrimary, connectionDetails.UserId, connectionDetails.Pin, Segment.HISYN, segments, Segment.HIRMS, SEG.NUM);
            string response = FinTSMessage.Send(connectionDetails.Url, message);

            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(response, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(response);

            return response;
        }
    }
}
