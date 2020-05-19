/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System;
using System.Runtime.Serialization;

namespace libfintx.Exceptions
{
    public class RecoverySyncException: EbicsException
    {
        public RecoverySyncException()
        {
        }

        public RecoverySyncException(string message) : base(message)
        {
        }

        public RecoverySyncException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RecoverySyncException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}