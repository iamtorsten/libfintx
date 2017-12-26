﻿/*	
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
    public static class HKSAL
    {
        /// <summary>
        /// Balance
        /// </summary>
        public static string Init_HKSAL(string Konto, int BLZ, string IBAN, string BIC, string URL, int HBCIVersion, string UserID, string PIN)
        {
            Log.Write("Starting job HKSAL: Request balance");

            string segments = string.Empty;

            if (Convert.ToInt16(Segment.HISALS) >= 7)
                segments = "HKSAL:" + SEGNUM.SETVal(3) + ":" + Segment.HISALS + "+" + IBAN + ":" + BIC + "+N'";
            else
            {
                segments = "HKSAL:" + SEGNUM.SETVal(3) + ":" + Segment.HISALS + "+" + Konto + "::280:" + BLZ + "+N'";
            }

            SEG.NUM = SEGNUM.SETInt(3);

            return FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS, SEG.NUM));
        }
    }
}
