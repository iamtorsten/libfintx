/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2016 - 2021 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 *  
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *  
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU Affero General Public License for more details.
 *  
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program. If not, see <http://www.gnu.org/licenses/>.
 * 	
 */

namespace libfintx.FinTS
{
    public class Sig
    {
        public const string SECFUNC_HBCI_SIG_RDH = "1";
        public const string SECFUNC_HBCI_SIG_DDV = "2";

        public const string SECFUNC_FINTS_SIG_DIG = "1";
        public const string SECFUNC_FINTS_SIG_SIG = "2";

        public const string SECFUNC_SIG_PT_1STEP = "999";
        public const string SECFUNC_SIG_PT_2STEP_MIN = "900";
        public const string SECFUNC_SIG_PT_2STEP_MAX = "997";

        public const string HASHALG_SHA1 = "1";
        public const string HASHALG_SHA256 = "3";
        public const string HASHALG_SHA384 = "4";
        public const string HASHALG_SHA512 = "5";
        public const string HASHALG_SHA256_SHA256 = "6";
        public const string HASHALG_RIPEMD160 = "999";

        public const string SIGALG_DES = "1";
        public const string SIGALG_RSA = "10";

        public const string SIGMODE_ISO9796_1 = "16";
        public const string SIGMODE_ISO9796_2 = "17";
        public const string SIGMODE_PKCS1 = "18";
        public const string SIGMODE_PSS = "19";
        public const string SIGMODE_RETAIL_MAC = "999";
    }
}
