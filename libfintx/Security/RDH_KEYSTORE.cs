/*	
 * 	
 *  This file is part of hbci4dotnet.
 *  
 *  Copyright (c) 2017 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 * 	
 * 	hbci4dotnet is free software; you can redistribute it and/or
 *	modify it under the terms of the GNU Lesser General Public
 * 	License as published by the Free Software Foundation; either
 * 	version 2.1 of the License, or (at your option) any later version.
 *	
 * 	hbci4dotnet is distributed in the hope that it will be useful,
 * 	but WITHOUT ANY WARRANTY; without even the implied warranty of
 * 	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * 	Lesser General Public License for more details.
 *	
 * 	You should have received a copy of the GNU Lesser General Public
 * 	License along with hbci4dotnet; if not, write to the Free Software
 * 	Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 * 	
 */

namespace libfintx
{
    public static class RDH_KEYSTORE
    {
        public static string KEY_ENCRYPTION_PUBLIC_BANK { get; set; }
        public static string KEY_SIGNING_PUBLIC_BANK { get; set; }

        public static string KEY_ENCRYPTION_PRIVATE { get; set; }
        public static string KEY_SIGNING_PRIVATE { get; set; }
        public static string KEY_SIGNING_PRIVATE_PRIVATE { get; set; }
    }
}
