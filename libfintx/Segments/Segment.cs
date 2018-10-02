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

using System.Reflection;
using System.Linq;
using System;

namespace libfintx
{
    public static class Segment
    {
        /// <summary>
        /// TAN
        /// </summary>
        public static string HIRMS { get; set; }
        public static string HIRMSf { get; set; }

        /// <summary>
        /// TAN
        /// </summary>
        public static string HITANS { get; set; }

        /// <summary>
        /// TAN
        /// </summary>
        public static string HITAN { get; set; }

        /// <summary>
        /// DialogID
        /// </summary>
        public static string HNHBK { get; set; }

        /// <summary>
        /// SystemID
        /// </summary>
        public static string HISYN { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public static string HNHBS { get; set; }

        /// <summary>
        /// Segment
        /// </summary>
        public static string HISALS { get; set; }
        public static string HISALSf { get; set; }

        /// <summary>
        /// Transactions
        /// </summary>
        public static string HKKAZ { get; set; }

        /// <summary>
        /// Transactions camt053
        /// </summary>
        public static string HKCAZ { get { return "1"; } }

        /// <summary>
        /// TAN Medium Name
        /// </summary>
        public static string HITAB { get; set; }

        /// <summary>
        /// PAIN version
        /// </summary>
        public static int HISPAS { get; set; }

        public static void Reset()
        {
            try
            {
                var propList = typeof(Segment)
                    .GetProperties(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.PropertyType == typeof(string));

                foreach (var prop in propList)
                {
                    if (prop.CanWrite)
                        prop.SetValue(null, null, null);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Couldn't reset Segment.", ex);
            }
        }
    }
}
