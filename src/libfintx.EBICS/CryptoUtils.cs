/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2018 Bjoern Kuensting
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
using System.Security.Cryptography;
using System.Text;

namespace libfintx.EBICS
{
    internal static class CryptoUtils
    {
        internal static string GetNonce()
        {
            var provider = new RNGCryptoServiceProvider();
            var bnonce = new byte[16];
            provider.GetBytes(bnonce);
            return BitConverter.ToString(bnonce).Replace("-", "");
        }

        internal static byte[] GetTransactionKey()
        {
            var provider = new RNGCryptoServiceProvider();
            var key = new byte[16];
            provider.GetBytes(key);
            return key;
        }

        internal static string GetUtcTimeNow()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        internal static string FormatUtcTime(DateTime? dt)
        {
            return dt.HasValue ? dt.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") : null;
        }

        internal static string Print(byte[] arr)
        {
            if (arr == null)
            {
                return "null";
            }
            
            var sb = new StringBuilder("[");
            for (var i = 0; i < arr.Length; i++)
            {
                var b = arr[i];
                sb.Append(b);
                if (i < arr.Length - 1)
                {
                    sb.Append(",");
                }
            }

            sb.Append("]");
            return sb.ToString();
        }
    }
}
