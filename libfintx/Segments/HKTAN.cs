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

using System;

namespace libfintx
{
    public static class HKTAN
    {
        /// <summary>
        /// Set tan process
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        public static string Init_HKTAN(string segments)
        {
            if (String.IsNullOrEmpty(Segment.HITAB)) // TAN Medium Name not set
                segments = segments + "HKTAN:" + SEGNUM.SETVal(4) + ":" + Segment.HITANS + "'";
            else // TAN Medium Name set
            {
                // Version 3, Process 4
                if (Segment.HITANS.Substring(0, 3).Equals("3+4"))
                    segments = segments + "HKTAN:" + SEGNUM.SETVal(4) + ":" + Segment.HITANS + "++++++++" + Segment.HITAB + "'";
                // Version 4, Process 4
                if (Segment.HITANS.Substring(0, 3).Equals("4+4"))
                    segments = segments + "HKTAN:" + SEGNUM.SETVal(4) + ":" + Segment.HITANS + "+++++++++" + Segment.HITAB + "'";
                // Version 5, Process 4
                if (Segment.HITANS.Substring(0, 3).Equals("5+4"))
                    segments = segments + "HKTAN:" + SEGNUM.SETVal(4) + ":" + Segment.HITANS + "+++++++++++" + Segment.HITAB + "'";
            }

            return segments;
        }
    }
}
