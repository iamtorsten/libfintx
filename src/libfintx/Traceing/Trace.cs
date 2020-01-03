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
using System.IO;
using System.Text.RegularExpressions;

namespace libfintx
{
    public static class Trace
    {
        /// <summary>
        /// Enable tracecing
        /// 
        /// Warning:
        /// This enables the library to write plain message incl. PIN, UserID and TAN
        /// human readable into a textfile!
        /// </summary>
        public static bool Enabled { get; set; }

        public static bool Formatted { get; set; }

        /// <summary>
        /// Maximum size of trace file in MB after file will be cleared. Non-positive value means that file will never be cleared.
        /// </summary>
        public static int MaxFileSize { get; set; }

        /// <summary>
        /// Trace
        /// </summary>
        /// <param name="message"></param>
        public static void Write(string message)
        {
            if (Enabled)
            {
                // Directory
                var dir = Helper.GetProgramBaseDir();

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                // Tracefile
                dir = Path.Combine(dir, "TRACE");

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                string file = Path.Combine(dir, "Trace.txt");
                if (!File.Exists(file))
                {
                    using (File.Create(Path.Combine(dir, "Trace.txt")))
                    { };
                }

                if (MaxFileSize > 0)
                {
                    var sizeMB = (double)new FileInfo(file).Length / (double)1000000;
                    if (sizeMB > MaxFileSize)
                        File.WriteAllText(file, string.Empty);
                }

                if (Formatted)
                {
                    var formatted = string.Empty;
                    var matches = Regex.Matches(message, "[A-Z]{5}[^']*'+");
                    foreach (Match match in matches)
                    {
                        formatted += match.Value + Environment.NewLine;
                    }

                    File.AppendAllText(file, "[" + DateTime.Now + "]" + Environment.NewLine + formatted + Environment.NewLine);
                }
                else
                {
                    File.AppendAllText(file, "[" + DateTime.Now + "]" + " " + message + Environment.NewLine);
                }
            }
        }
    }
}