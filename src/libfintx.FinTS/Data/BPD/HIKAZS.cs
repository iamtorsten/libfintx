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


using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using libfintx.FinTS.Data.Segment;

namespace libfintx.FinTS
{
    public class HIKAZS : SegmentBase
    {
        public int Zeitraum { get; set; }

        public HIKAZS(Segment segment) : base(segment)
        {
        }
    }
}
