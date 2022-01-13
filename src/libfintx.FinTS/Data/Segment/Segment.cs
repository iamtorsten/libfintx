/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2021 - 2002 Abid Hussain
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
    public class Segment
    {
        public string Value { get; set; }

        public string Name { get; set; }

        public int Number { get; set; }

        public int Version { get; set; }

        public int? Ref { get; set; }

        public string Payload { get; set; }

        private List<string> _dataElements;
        /// <summary>
        /// Datenelemente der Payload.
        /// </summary>
        public List<string> DataElements
        {
            get
            {
                if (_dataElements == null)
                    _dataElements = new List<string>();

                return _dataElements;
            }
            set
            {
                _dataElements = value;
            }
        }

        public Segment(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Segment segment &&
                   Name == segment.Name &&
                   Number == segment.Number &&
                   Version == segment.Version;
        }

        public override int GetHashCode()
        {
            var hashCode = 1663882333;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Number.GetHashCode();
            hashCode = hashCode * -1521134295 + Version.GetHashCode();
            return hashCode;
        }
    }
}
