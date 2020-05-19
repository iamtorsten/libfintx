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
    public class DeserializationException : EbicsException
    {
        private string _xml;

        public string Xml => _xml;

        public DeserializationException()
        {
        }

        public DeserializationException(string message) : base(message)
        {
        }

        public DeserializationException(string message, string xmlPayload) : base(message)
        {
            _xml = xmlPayload;
        }

        public DeserializationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public DeserializationException(string message, Exception innerException, string xmlPayload) : base(message,
            innerException)
        {
            _xml = xmlPayload;
        }

        protected DeserializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}