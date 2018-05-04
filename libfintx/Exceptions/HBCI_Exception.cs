/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2018 Torsten Klinger
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
    /// Exception class
    /// </summary>
    class HBCI_Exception
    {
        public static string CRYPTEDLENGTH() { return "Session key length is not equal to the length of Public bank encryption key length."; }

        public static string SOFTWARE() { return "Software error: "; }

        public static string INI() { return "Initialisation failed"; }

        public static string HBCIVERSIONNOTSUPPORTED() { return "HBCI version not supported"; }
    }
}
