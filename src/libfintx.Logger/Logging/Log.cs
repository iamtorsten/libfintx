/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2016 - 2022 Torsten Klinger
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
using libfintx.Globals;

namespace libfintx.Logger.Log
{
    public static class Log
    {
        public static bool Enabled { get; set; }

        /// <summary>
        /// Maximum size of trace file in MB after file will be cleared. Non-positive value means that file will never be cleared.
        /// </summary>
        public static int MaxFileSize { get; set; }

        public static void Write(object obj)
        {
            Write(obj.ToString());
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="Message"></param>
        public static void Write(string Message)
        {
            if (Enabled)
            {
                // Directory
                var dir = FinTsGlobals.ProgramBaseDir;

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                // Logfile
                dir = Path.Combine(dir, "LOG");

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                string file = Path.Combine(dir, "Log.txt");
                if (!File.Exists(file))
                {
                    using (File.Create(file))
                    { };
                }

                if (MaxFileSize > 0)
                {
                    var sizeMB = new FileInfo(file).Length / (double) 1000000;
                    if (sizeMB > MaxFileSize)
                        File.WriteAllText(file, string.Empty);
                }

                File.AppendAllText(file, "[" + DateTime.Now + "]" + " " + Message + Environment.NewLine);
            }
        }
    }
}
