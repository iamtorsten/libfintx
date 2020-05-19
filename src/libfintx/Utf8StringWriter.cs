/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System;
using System.IO;
using System.Text;

namespace libfintx
{
    public class Utf8StringWriter : StringWriter
    {
        public Utf8StringWriter(IFormatProvider formatProvider) : base(formatProvider)
        {
        }

        public Utf8StringWriter(StringBuilder sb) : base(sb)
        {
        }

        public Utf8StringWriter(StringBuilder sb, IFormatProvider formatProvider) : base(sb, formatProvider)
        {
        }

        public override Encoding Encoding => new UTF8Encoding(false);
    }
}
