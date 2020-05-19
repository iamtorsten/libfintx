/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace libfintx
{
    internal class MethodLogger : IDisposable
    {
        private readonly ILogger _log;
        private readonly string _msgExit;

        private const string _withClazz = "{0}.{1} {2}";
        private const string _withoutClazz = "{0} {1}";

        public MethodLogger(ILogger logger, bool useClazzName = true)
        {
            if (!EbicsLogging.MethodLoggingEnabled) return;
            _log = logger;
            var frame = new StackFrame(1);
            var mth = frame.GetMethod();
            var mname = mth.Name;
            var clazz = mth.DeclaringType.Name;
            var msgEntry = useClazzName
                ? string.Format(_withClazz, clazz, mname, "entry")
                : string.Format(_withoutClazz, mname, "entry");
            _msgExit = useClazzName
                ? string.Format(_withClazz, clazz, mname, "exit")
                : string.Format(_withoutClazz, mname, "exit");
            _log.LogDebug(msgEntry);
        }

        public void Dispose()
        {
            if (!EbicsLogging.MethodLoggingEnabled) return;
            _log.LogDebug(_msgExit);
        }
    }
}