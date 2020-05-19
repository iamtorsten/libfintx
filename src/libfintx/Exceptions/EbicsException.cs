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
    [Serializable]
    public class EbicsException : Exception
    {
        public EbicsException()
        {
        }

        public EbicsException(string message) : base(message)
        {
        }

        public EbicsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EbicsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}