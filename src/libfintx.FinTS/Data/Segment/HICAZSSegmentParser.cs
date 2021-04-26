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
    internal class HICAZSSegmentParser : ISegmentParser
    {
        public Segment ParseSegment(Segment segment)
        {
            var result = new HICAZS(segment);

            // HICAZS:92:1:4+1+1+1+450:N:N:urn?:iso?:std?:iso?:20022?:tech?:xsd?:camt.052.001.02
            var match = Regex.Match(segment.Payload, @"^(?<maxanzauftr>\d*)\+(?<minanzsig>\d*)\+(?<sicherheitskl>\d*)\+(?<zeitraum>\d*):(?<maxeintr>[J|N]{0,1}):(?<allekt>[J|N]{0,1}):(?<formate>.*)");
            if (!match.Success)
                throw new ArgumentException($"Could not parse segment{Environment.NewLine}{segment.Payload}");

            var zeitraum = match.Groups["zeitraum"].Value;
            if (zeitraum != null)
                result.Zeitraum = Convert.ToInt32(zeitraum);

            result.Format = match.Groups["formate"].Value;

            return result;
        }
    }
}
