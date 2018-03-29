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

using libfintx.Data;

namespace libfintx
{
    public static class TAN
    {
        /// <summary>
        /// TAN
        /// </summary>
        public static string Send_TAN(ConnectionDetails connectionDetails, string TAN)
        {
            Log.Write("Starting TAN process");

            string segments = string.Empty;

            // Version 2, Process 2
            if (Segment.HITANS.Substring(0, 1).Equals("2+2"))
                segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + Segment.HITANS.Substring(0, 1) + "+2++" + Segment.HITAN + "++N'";
            // Version 3, Process 2
            if (Segment.HITANS.Substring(0, 1).Equals("3+2"))
                segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + Segment.HITANS.Substring(0, 1) + "+2++" + Segment.HITAN + "++N'";
            // Version 4, Process 2
            if (Segment.HITANS.Substring(0, 1).Equals("4+2"))
                segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + Segment.HITANS.Substring(0, 1) + "+2++" + Segment.HITAN + "++N'";
            // Version 5, Process 2
            if (Segment.HITANS.Substring(0, 1).Equals("5+2"))
                segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + Segment.HITANS.Substring(0, 1) + "+2++++" + Segment.HITAN + "++N'";
            else
                segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + Segment.HITANS.Substring(0, 1) + "+2++++" + Segment.HITAN + "++N'";

            SEG.NUM = SEGNUM.SETInt(3);

            return FinTSMessage.Send(connectionDetails.Url, FinTSMessage.Create(connectionDetails.HBCIVersion, Segment.HNHBS, Segment.HNHBK, connectionDetails.Blz, connectionDetails.UserId, connectionDetails.Pin, Segment.HISYN, segments, Segment.HIRMS + ":" + TAN, SEG.NUM));
        }
    }
}
