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
    internal class DEBUG
    {
        public static bool Enabled { get; set; }

        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="message"></param>
        public static void Write(string message)
        {
            if (Enabled)
            {
                Console.WriteLine("[" + DateTime.Now + "]" + " " + message + Environment.NewLine);
            }
        }
    }
}
