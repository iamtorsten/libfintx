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
    public static class TAN4
    {
        /// <summary>
        /// TAN process 4
        /// </summary>
        public static string Send_TAN4(string TAN, string URL, int HBCIVersion, int BLZ, string UserID, string PIN, string MediumName)
        {
            Log.Write("Starting job TAN process 4");

            string segments = string.Empty;

            // Version 3, Process 4
            if (Segment.HITANS.Substring(0, 1).Equals("3+4"))
                segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + Segment.HITANS.Substring(0, 1) + "+4+++++++" + MediumName + "'";
            // Version 4, Process 4
            if (Segment.HITANS.Substring(0, 1).Equals("4+4"))
                segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + Segment.HITANS.Substring(0, 1) + "+4++++++++" + MediumName + "'";
            // Version 5, Process 4
            if (Segment.HITANS.Substring(0, 1).Equals("5+4"))
                segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + Segment.HITANS.Substring(0, 1) + "+4++++++++++" + MediumName + "'";

            SEG.NUM = SEGNUM.SETInt(3);

            return FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS + ":" + TAN, SEG.NUM));
        }
    }
}
