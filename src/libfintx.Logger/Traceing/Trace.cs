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
using System.Text.RegularExpressions;
using libfintx.Globals;

namespace libfintx.Logger.Trace
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
        /// Mask credentials (User-ID, PIN) before writing to trace file.
        /// </summary>
        public static bool MaskCredentials { get; set; }

        /// <summary>
        /// Maximum size of trace file in MB after file will be cleared. Non-positive value means that file will never be cleared.
        /// </summary>
        public static int MaxFileSize { get; set; }

        /// <summary>
        /// Used to mask credentials.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="userId"></param>
        /// <param name="pin"></param>
        public static void Write(string message, string userId, string pin)
        {
            message = Regex.Replace(message, $@"\b{Regex.Escape(userId)}\b", "XXXXXX");
            message = Regex.Replace(message, $@"\b{Regex.Escape(pin)}\b", "XXXXXX");

            Write(message);
        }

        /// <summary>
        /// Trace
        /// </summary>
        /// <param name="message"></param>
        public static void Write(string message)
        {
            if (Enabled)
            {
                // Directory
                var dir = FinTsGlobals.ProgramBaseDir;

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
                    var sizeMB = new FileInfo(file).Length / (double) 1000000;
                    if (sizeMB > MaxFileSize)
                        File.WriteAllText(file, string.Empty);
                }

                if (Formatted)
                {
                    var formatted = string.Empty;
                    var matches = Regex.Matches(message, "[A-Z]+?[^']*'+");
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
