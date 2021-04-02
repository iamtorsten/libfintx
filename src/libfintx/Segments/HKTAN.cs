/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2016 - 2021 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 *  
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *  
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU Affero General Public License for more details.
 *  
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program. If not, see <http://www.gnu.org/licenses/>.
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
        public static string Init_HKTAN(FinTsClient client, string segments, string segmentId)
        {
            if (String.IsNullOrEmpty(client.HITAB)) // TAN Medium Name not set
            {
                if (client.HITANS == 6)
                    segments = segments + "HKTAN:" + client.SEGNUM + ":" + client.HITANS + "+4+" + segmentId + "'";
                else
                    segments = segments + "HKTAN:" + client.SEGNUM + ":" + client.HITANS + "+4+'";
            }
            else // TAN Medium Name set
            {
                // Version 3, Process 4
                if (client.HITANS == 3)
                    segments = segments + "HKTAN:" + client.SEGNUM + ":" + client.HITANS + "+4++++++++" + client.HITAB + "'";
                // Version 4, Process 4
                if (client.HITANS == 4)
                    segments = segments + "HKTAN:" + client.SEGNUM + ":" + client.HITANS + "+4+++++++++" + client.HITAB + "'";
                // Version 5, Process 4
                if (client.HITANS == 5)
                    segments = segments + "HKTAN:" + client.SEGNUM + ":" + client.HITANS + "+4+++++++++++" + client.HITAB + "'";
                // Version 6, Process 4
                if (client.HITANS == 6)
                {
                    segments = segments + "HKTAN:" + client.SEGNUM + ":" + client.HITANS + "+4+" + segmentId + "+++++++++" + client.HITAB + "'";
                }
            }

            return segments;
        }
    }
}
