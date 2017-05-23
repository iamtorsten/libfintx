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
    public static class HKKAZ
    {
        /// <summary>
        /// Transactions
        /// </summary>
        public static string Init_HKKAZ(int Konto, int BLZ, string IBAN, string BIC, string URL, int HBCIVersion, string UserID, string PIN, string FromDate, string Startpoint)
        {
            Log.Write("Starting job HKKAZ: Request transactions");

            string segments = string.Empty;

            if (String.IsNullOrEmpty(FromDate))
            {
                if (String.IsNullOrEmpty(Startpoint))
                {
                    if (Convert.ToInt16(Segment.HKKAZ) < 7)
                        segments = "HKKAZ:" + SEGNUM.RETVal(3) + ":" + Segment.HKKAZ + "+" + Konto + "::280:" + BLZ + "+N'";
                    else
                        segments = "HKKAZ:" + SEGNUM.RETVal(3) + ":" + Segment.HKKAZ + "+" + IBAN + ":" + BIC + ":" + Konto + "::280:" + BLZ + "+N'";
                }
                else
                {
                    if (Convert.ToInt16(Segment.HKKAZ) < 7)
                        segments = "HKKAZ:" + SEGNUM.RETVal(3) + ":" + Segment.HKKAZ + "+" + Konto + "::280:" + BLZ + "+N++++" + Startpoint + "'";
                    else
                        segments = "HKKAZ:" + SEGNUM.RETVal(3) + ":" + Segment.HKKAZ + "+" + IBAN + ":" + BIC + ":" + Konto + "::280:" + BLZ + "+N++++" + Startpoint + "'";
                }
            }
            else
            {
                if (String.IsNullOrEmpty(Startpoint))
                {
                    if (Convert.ToInt16(Segment.HKKAZ) < 7)
                        segments = "HKKAZ:" + SEGNUM.RETVal(3) + ":" + Segment.HKKAZ + "+" + Konto + "::280:" + BLZ + "+N+" + FromDate + "'";
                    else
                        segments = "HKKAZ:" + SEGNUM.RETVal(3) + ":" + Segment.HKKAZ + "+" + IBAN + ":" + BIC + ":" + Konto + "::280:" + BLZ + "+N+" + FromDate + "'";
                }
                else
                {
                    if (Convert.ToInt16(Segment.HKKAZ) < 7)
                        segments = "HKKAZ:" + SEGNUM.RETVal(3) + ":" + Segment.HKKAZ + "+" + Konto + "::280:" + BLZ + "+N+" + FromDate + "+++" + Startpoint + "'";
                    else
                        segments = "HKKAZ:" + SEGNUM.RETVal(3) + ":" + Segment.HKKAZ + "+" + IBAN + ":" + BIC + ":" + Konto + "::280:" + BLZ + "+N+" + FromDate + "+++" + Startpoint + "'";
                }
            }

            SEG.NUM = SEGNUM.RETInt(3);

            return FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS, SEG.NUM));
        }
    }
}
