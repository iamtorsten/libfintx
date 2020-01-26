/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2020 Torsten Klinger
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
    public static class RdhKeyStore
    {
        public static string KEY_ENCRYPTION_PUBLIC_BANK { get; set; }
        public static string KEY_SIGNING_PUBLIC_BANK { get; set; }

        public static string KEY_ENCRYPTION_PRIVATE { get; set; }
        public static string KEY_SIGNING_PRIVATE { get; set; }

        public static string KEY_ENCRYPTION_PRIVATE_XML { get; set; }
        public static string KEY_SIGNING_PRIVATE_XML { get; set; }

        public static string Blz { get; set; }

        public static string UserId { get; set; }

        public static string Country { get; set; }

        public static string ProfileVersion { get; set; }
    }
}