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

namespace libfintx
{
    public static class HKKAZ
    {
        /// <summary>
        /// Transactions
        /// </summary>
        public static string Init_HKKAZ(ConnectionDetails connectionDetails, string FromDate, string ToDate, string Startpoint)
        {
            Log.Write("Starting job HKKAZ: Request transactions");

            string segments = string.Empty;

            if (String.IsNullOrEmpty(FromDate))
            {
                if (String.IsNullOrEmpty(Startpoint))
                {
                    if (Convert.ToInt16(Segment.HKKAZ) < 7)
                        segments = "HKKAZ:" + SEGNUM.SETVal(3) + ":" + Segment.HKKAZ + "+" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+N'";
                    else
                        segments = "HKKAZ:" + SEGNUM.SETVal(3) + ":" + Segment.HKKAZ + "+" + connectionDetails.IBAN + ":" + connectionDetails.BIC + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+N'";
                }
                else
                {
                    if (Convert.ToInt16(Segment.HKKAZ) < 7)
                        segments = "HKKAZ:" + SEGNUM.SETVal(3) + ":" + Segment.HKKAZ + "+" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+N++++" + Startpoint + "'";
                    else
                        segments = "HKKAZ:" + SEGNUM.SETVal(3) + ":" + Segment.HKKAZ + "+" + connectionDetails.IBAN + ":" + connectionDetails.BIC + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+N++++" + Startpoint + "'";
                }
            }
            else
            {
                if (String.IsNullOrEmpty(Startpoint))
                {
                    if (Convert.ToInt16(Segment.HKKAZ) < 7)
                        segments = "HKKAZ:" + SEGNUM.SETVal(3) + ":" + Segment.HKKAZ + "+" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+N+" + FromDate + "+" + ToDate + "'";
                    else
                        segments = "HKKAZ:" + SEGNUM.SETVal(3) + ":" + Segment.HKKAZ + "+" + connectionDetails.IBAN + ":" + connectionDetails.BIC + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+N+" + FromDate + "+" + ToDate + "'";
                }
                else
                {
                    if (Convert.ToInt16(Segment.HKKAZ) < 7)
                        segments = "HKKAZ:" + SEGNUM.SETVal(3) + ":" + Segment.HKKAZ + "+" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+N+" + FromDate + "+" + ToDate + "++" + Startpoint + "'";
                    else
                        segments = "HKKAZ:" + SEGNUM.SETVal(3) + ":" + Segment.HKKAZ + "+" + connectionDetails.IBAN + ":" + connectionDetails.BIC + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+N+" + FromDate + "+" + ToDate + "++" + Startpoint + "'";
                }
            }

            SEG.NUM = SEGNUM.SETInt(3);

            string message = FinTSMessage.Create(connectionDetails.HBCIVersion, Segment.HNHBS, Segment.HNHBK, connectionDetails.Blz, connectionDetails.UserId, connectionDetails.Pin, Segment.HISYN, segments, Segment.HIRMS, SEG.NUM);
            return FinTSMessage.Send(connectionDetails.Url, message);
        }
    }
}
