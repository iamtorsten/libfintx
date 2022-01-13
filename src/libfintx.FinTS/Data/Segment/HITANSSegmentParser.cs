/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2021 - 2022 Abid Hussain
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
    internal class HITANSSegmentParser : ISegmentParser
    {
        public Segment ParseSegment(Segment segment)
        {
            var result = new HITANS(segment);

            // HITANS:175:7:4+1+1+1+N:N:0:922:2:pushTAN-dec:Decoupled::pushTAN 2.0:::Aufforderung:2048:J:2:N:0:0:N:N:00:2:N:2:180:1:1:J:J'
            var matches = Regex.Matches(segment.Payload, @"(?<1>9\d{2}):(?<2>1|2):(?<3>.+?):(?<4>.*?):(?<5>.*?):(?<6>.+?):");
            if (matches.Count == 0)
                throw new ArgumentException($"Could not parse segment '{segment.Name}':{Environment.NewLine}{segment.Payload}");

            foreach (Match match in matches)
            {
                var tanProcess = new HITANS_TanProcess();

                var tanCode = match.Groups["1"].Value;
                if (tanCode != null)
                    tanProcess.TanCode = Convert.ToInt32(tanCode);

                var processName = match.Groups["6"].Value;
                if (processName != null)
                    tanProcess.Name = processName;

                result.TanProcesses.Add(tanProcess);
            }

            return result;
        }
    }
}
