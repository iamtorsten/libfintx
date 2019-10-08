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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace libfintx
{
    public static class camt052File
    {
        /// <summary>
        /// Save xml string as camt052 file
        /// </summary>
        public static string Save(string Account, string UMS)
        {
            string documents = "", dir = "";

            dir = Path.Combine(documents, Program.Buildname);
            dir = Path.Combine(dir, "camt052");

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string filename = Path.Combine(dir, Helper.MakeFilenameValid(Account + "_" + DateTime.Now + "-" + Guid.NewGuid() + ".camt052"));

            // camt052
            if (!File.Exists(filename))
            {
                using (File.Create(filename))
                { };

                File.AppendAllText(filename, UMS);
            }
            else
                File.AppendAllText(filename, UMS);

            return filename;
        }
    }
}
