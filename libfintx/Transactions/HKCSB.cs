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

namespace libfintx
{
    public static class HKCSB
    {
        /// <summary>
        /// Get bankers orders
        /// </summary>
        public static string Init_HKCSB(int BLZ, string IBAN, string BIC, string URL, int HBCIVersion, string UserID, string PIN)
        {
            Log.Write("Starting job HKCSB: Get bankers order");

            string segments = "HKCSB:" + SEGNUM.SETVal(3) + ":1+" + IBAN + ":" + BIC + "+sepade?:xsd?:pain.001.001.03.xsd'";

            SEG.NUM = SEGNUM.SETInt(3);

            return FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS, SEG.NUM));
        }
    }
}
