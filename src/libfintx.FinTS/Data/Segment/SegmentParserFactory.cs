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

namespace libfintx.FinTS.Data.Segment
{
    internal class SegmentParserFactory
    {
        public static Segment ParseSegment(string segmentCode)
        {
            var segment = new Segment(segmentCode);

            var genericParser = new GenericSegmentParser();
            genericParser.ParseSegment(segment);

            Type parserType = Type.GetType($"libfintx.FinTS.Data.Segment.{segment.Name}SegmentParser");
            if (parserType == null)
                return segment;

            return ((ISegmentParser) Activator.CreateInstance(parserType)).ParseSegment(segment);
        }
    }
}
