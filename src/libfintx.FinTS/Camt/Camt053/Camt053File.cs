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

using System;
using System.IO;
using System.Text;
using libfintx.Globals;

namespace libfintx.FinTS.Camt.Camt053
{
    public static class Camt053File
    {
        /// <summary>
        /// Save xml string as camt053 file
        /// </summary>
        public static string Save(string Account, string UMS, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            string dir = FinTsGlobals.ProgramBaseDir;

            dir = Path.Combine(dir, "camt053");

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string filename = Path.Combine(dir, Helper.MakeFilenameValid(Account + "_" + DateTime.Now + "-" + Guid.NewGuid() + ".camt053"));

            // camt053
            if (!File.Exists(filename))
            {
                using (File.Create(filename))
                { };
            }

            File.AppendAllText(filename, UMS, encoding);

            return filename;
        }
    }
}
