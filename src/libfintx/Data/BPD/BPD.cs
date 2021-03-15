/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2020 Torsten Klinger
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

using libfintx.Data.Segment;
using libfintx.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace libfintx
{
    public static class BPD
    {
        public static string Value { get; set; }

        public static HIPINS HIPINS { get; set; }

        public static List<HICAZS> HICAZS { get; set; }

        public static List<HIKAZS> HIKAZS { get; set; }

        public static List<Segment> SegmentList { get; set; }

        public static void Reset()
        {
            ReflectionUtil.ResetStaticFields(typeof(BPD));
        }

        public static void ParseBpd(string bpd)
        {
            Value = bpd;
            SegmentList = new List<Segment>();
            HICAZS = new List<HICAZS>();
            HIKAZS = new List<HIKAZS>();

            var lines = bpd.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Where(l => !string.IsNullOrWhiteSpace(l));
            foreach (var line in lines)
            {
                var segment = SegmentParserFactory.ParseSegment(line);
                if (segment is HIPINS)
                    HIPINS = (HIPINS) segment;
                else if (segment is HICAZS)
                    HICAZS.Add((HICAZS) segment);
                else if (segment is HIKAZS)
                    HIKAZS.Add((HIKAZS) segment);
                else
                    SegmentList.Add(segment);
            }
        }
    }
}
