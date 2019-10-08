/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2018 Torsten Klinger
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

using System;
using System.Text;

namespace libfintx
{
    public static class Converter
    {
        /// <summary>
        /// Convert string from hex
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static string FromHexString(string hexString)
        {
            string hexValues = hexString;

            string[] hexValuesSplit = hexValues.Split(' ');

            string charValue = string.Empty;

            foreach (String hex in hexValuesSplit)
            {
                // Convert the number expressed in base-16 to an integer.
                int value = Convert.ToInt32(hex, 16);
                
                // Get the character corresponding to the integral value.
                string stringValue = Char.ConvertFromUtf32(value);
                charValue = charValue + (char)value;
            }

            return charValue;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", " ");
        }

        public static string ConvertEncoding(string str, Encoding input, Encoding result)
        {
            byte[] bytes = input.GetBytes(str);
            return result.GetString(bytes);
        }
    }
}
