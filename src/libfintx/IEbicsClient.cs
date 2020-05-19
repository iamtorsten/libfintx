/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using libfintx.Config;
using libfintx.Parameters;
using libfintx.Responses;

namespace libfintx
{
    public interface IEbicsClient
    {
        EbicsConfig Config { get; set; }
        HpbResponse HPB(HpbParams p);
        PtkResponse PTK(PtkParams p);
        StaResponse STA(StaParams p);
        CctResponse CCT(CctParams p);
        IniResponse INI(IniParams p);
        HiaResponse HIA(HiaParams p);
        SprResponse SPR(SprParams p);
        CddResponse CDD(CddParams p);
    }
}