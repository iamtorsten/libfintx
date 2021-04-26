/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2016 - 2021 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 *  
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 3 of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software Foundation,
 *  Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 * 	
 */

using System;
using System.IO;

namespace libfintx.Globals
{
    public class FinTsGlobals
    {
        private static string _programBaseDir;
        public static string ProgramBaseDir
        {
            get
            {
                if (_programBaseDir == null)
                {
                    var userHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    if (Buildname == null)
                    {
                        throw new InvalidOperationException("Der Wert von FinTsConfig.Buildname muss gesetzt sein.");
                    }

                    var buildname = Buildname.StartsWith(".") ? Buildname : $".{Buildname}";

                    _programBaseDir = Path.Combine(userHome, buildname);
                }

                return _programBaseDir;
            }
            set
            {
                _programBaseDir = value;
            }
        }

        public static string Buildname { get; set; } = "libfintx";

        public static string Version { get; set; } = "1.0.0";

        /// <summary>
        /// Produktregistrierungsnummer. Replace it with you own id if available.
        /// </summary>
        public static string ProductId = "9FA6681DEC0CF3046BFC2F8A6";
    }
}
