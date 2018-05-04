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

namespace libfintx
{
    public class EncodingHelper
    {
        /// <summary>
        /// Convert string to UTF-8
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static string ConvertToUTF8(string Message)
        {
            return Message.Replace("?b", "üb")
                .Replace("m?@", "ma@")
                .Replace("f?h", "füh")
                .Replace("r?", "rü")
                .Replace("f?", "fä")
                .Replace("t?", "tä")
                .Replace("m?", "mü")
                .Replace("?l", "ül")
                .Replace("h?f", "häf");
        }
    }
}
