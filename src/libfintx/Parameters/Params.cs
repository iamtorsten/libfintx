/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using Newtonsoft.Json;

namespace libfintx.Parameters
{
    public abstract class Params
    {
        

        public string SecurityMedium { get; set; } = "0000";

        static Params()
        {

        }

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
