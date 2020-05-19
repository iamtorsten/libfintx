/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System;

namespace libfintx.Parameters
{
    public class PtkParams : Params
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}