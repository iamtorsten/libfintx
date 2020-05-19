/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System;
using Microsoft.Extensions.Logging;
using libfintx.Responses;

namespace libfintx.Commands
{
    internal abstract class GenericCommand<T> : Command where T : Response
    {
        private static readonly ILogger s_logger = EbicsLogging.CreateLogger<GenericCommand<T>>();

        protected T _response;

        internal T Response
        {
            get
            {
                if (_response == null)
                {
                    _response = Activator.CreateInstance<T>();
                }

                return _response;
            }
            set => _response = value;
        }

        internal override DeserializeResponse Deserialize(string payload)
        {
            using (new MethodLogger(s_logger))
            {
                var dr = base.Deserialize(payload);
                UpdateResponse(Response, dr);
                return dr;
            }
        }
    }
}