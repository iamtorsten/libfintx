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

namespace libfintx
{
    /// <summary>
    /// Encryption algs
    /// </summary>
    public static class Enc
    {
        public const string SECFUNC_ENC_3DES = "4";
        public const string SECFUNC_ENC_PLAIN = "998";

        public const string ENCALG_2K3DES = "13";

        public const string ENCMODE_CBC = "2";
        public const string ENCMODE_PKCS1 = "18";

        public const string ENC_KEYTYPE_RSA = "6";
    }
}
