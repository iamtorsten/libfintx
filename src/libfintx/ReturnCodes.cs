/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

namespace libfintx
{
    public static class ReturnCodes
    {
        public const int TechnicalCodeOK = 0;
        public const int BusinessCodeOK = 0;
        public const int TechnicalCodePostProcessDone = 11000;
        public const int TechnicalCodePostProcessSkipped = 11001;
        public const int TechnicalCodeRecoverySync = 61101;
    }
}
