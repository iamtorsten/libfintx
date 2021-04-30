/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2021 Abid Hussain
 *  
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 3 of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software Foundation,
 *  Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 * 	
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace libfintx.FinTS.Data.Segment
{
    internal class HIPINSSegmentParser : ISegmentParser
    {
        public Segment ParseSegment(Segment segment)
        {
            HIPINS result = new HIPINS(segment);

            // HIPINS:170:1:4+1+1+0+5:5:6:USERID:CUSTID:HKAUB:J:
            // HIPINS:78:1:4+1+1+0+5:20:6:VR-NetKey oder Alias::HKTAN:N:
            var match = Regex.Match(segment.Payload, @"^(\d*)\+(\d*)\+(\d*)\+(\d*):(\d*):(\d*):(?<belegungbenutzerkennung>.*?):(?<belegungkundenid>.*?):(?<gvlist>.*)$");

            if (match.Success)
            {
                var gvList = match.Groups["gvlist"].Value;
                foreach (Match gvMatch in Regex.Matches(gvList, @"(?<gv>[A-Z]+):(?<tanrequired>J|N)"))
                {
                    var gv = gvMatch.Groups["gv"].Value;
                    var tanRequired = gvMatch.Groups["tanrequired"].Value;

                    result.GvPinTan[gv] = (tanRequired == "J");
                }
            }

            return result;
        }
    }
}
