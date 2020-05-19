/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System;
using System.Security.Cryptography;
using System.Text;

namespace libfintx
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
