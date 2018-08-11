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
    public static class TAN
    {
        /// <summary>
        /// TAN
        /// </summary>
        public static string Send_TAN(ConnectionDetails connectionDetails, string TAN)
        {
            Log.Write("Starting TAN process");

            string segments = string.Empty;

            var HITANS = !String.IsNullOrEmpty(Segment.HITANS.Substring(0, 1)) ? Int32.Parse(Segment.HITANS.Substring(0, 1)) : 0;

            if (String.IsNullOrEmpty(Segment.HITAB)) // TAN Medium Name not set
            {
                // Version 2
                if (HITANS == 2)
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + Segment.HITANS.Substring(0, 1) + "+2++" + Segment.HITAN + "++N'";
                // Version 3
                else if (HITANS == 3)
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + Segment.HITANS.Substring(0, 1) + "+2++" + Segment.HITAN + "++N'";
                // Version 4
                else if (HITANS == 4)
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + Segment.HITANS.Substring(0, 1) + "+2++" + Segment.HITAN + "++N'";
                // Version 5
                else if (HITANS == 5)
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + Segment.HITANS.Substring(0, 1) + "+2++++" + Segment.HITAN + "++N'";
                else // default
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + Segment.HITANS.Substring(0, 1) + "+2++++" + Segment.HITAN + "++N'";
            }
            else
            {
                // Version 2
                if (HITANS == 2)
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + Segment.HITANS.Substring(0, 1) + "+2++" + Segment.HITAN + "++N++++" + Segment.HITAB + "'";
                // Version 3
                else if (HITANS == 3)
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + Segment.HITANS.Substring(0, 1) + "+2++" + Segment.HITAN + "++N++++" + Segment.HITAB + "'";
                // Version 4
                else if (HITANS == 4)
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + Segment.HITANS.Substring(0, 1) + "+2++" + Segment.HITAN + "++N++++" + Segment.HITAB + "'";
                // Version 5
                else if (HITANS == 5)
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + Segment.HITANS.Substring(0, 1) + "+2++++" + Segment.HITAN + "++N++++" + Segment.HITAB + "'";
                else // default
                    segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + Segment.HITANS.Substring(0, 1) + "+2++" + Segment.HITAN + "++N++++" + Segment.HITAB + "'";
            }

            SEG.NUM = SEGNUM.SETInt(3);

            string message = FinTSMessage.Create(connectionDetails.HBCIVersion, Segment.HNHBS, Segment.HNHBK, connectionDetails.Blz, connectionDetails.UserId, connectionDetails.Pin, Segment.HISYN, segments, Segment.HIRMS + ":" + TAN, SEG.NUM);
            return FinTSMessage.Send(connectionDetails.Url, message);
        }
    }
}
