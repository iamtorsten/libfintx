/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2017 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 * 	
 * 	libfintx is free software; you can redistribute it and/or
 *	modify it under the terms of the GNU Lesser General Public
 * 	License as published by the Free Software Foundation; either
 * 	version 2.1 of the License, or (at your option) any later version.
 *	
 * 	libfintx is distributed in the hope that it will be useful,
 * 	but WITHOUT ANY WARRANTY; without even the implied warranty of
 * 	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * 	Lesser General Public License for more details.
 *	
 * 	You should have received a copy of the GNU Lesser General Public
 * 	License along with libfintx; if not, write to the Free Software
 * 	Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
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