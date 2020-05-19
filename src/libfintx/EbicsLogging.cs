/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using Microsoft.Extensions.Logging;

namespace libfintx
{
    public static class EbicsLogging
    {
        public static ILoggerFactory LoggerFactory { get; set; } = new LoggerFactory();
        public static bool MethodLoggingEnabled { get; set; }

        internal static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
    }
}