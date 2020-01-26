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

        public static List<HITANS> HITANS { get; set; }

        public static HICAZS HICAZS { get; set; }

        public static void Reset()
        {
            ReflectionUtil.ResetStaticFields(typeof(BPD));
        }

        public static void ParseBpd(string bpd)
        {
            Value = bpd;

            var lines = bpd.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            var hipins = lines.FirstOrDefault(l => l.StartsWith("HIPINS"));
            HIPINS = HIPINS.Parse_HIPINS(hipins ?? string.Empty);

            HITANS = new List<HITANS>();
            var list = lines.Where(l => l.StartsWith("HITANS"));
            foreach (var hitans in list)
            {
                var item = libfintx.HITANS.Parse_HITANS(hitans);
                HITANS.Add(item);
            }

            var hicazs = lines.FirstOrDefault(l => l.StartsWith("HICAZS"));
            HICAZS = HICAZS.Parse_HICAZS(hicazs);
        }
    }
}