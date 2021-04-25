/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2020 Abid Hussain
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

using libfintx.FinTS.Data.Segment;
using libfintx.FinTS.Util;
using libfintx.Logger.Log;
using System;
using System.Collections.Generic;
using System.Linq;

namespace libfintx.FinTS
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
                try
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
                catch (Exception ex)
                {
                    Log.Write($"Couldn't parse segment: {ex.Message}{Environment.NewLine}{line}");
                }
            }
        }
    }
}
