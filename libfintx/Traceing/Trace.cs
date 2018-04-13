/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2017 Torsten Klinger
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

namespace libfintx
{
    class Trace
    {
        public static bool Enabled { get; set; }

        /// <summary>
        /// Trace
        /// </summary>
        /// <param name="Message"></param>
        public static void Write(string Message)
        {
            if (Enabled)
            {
                // Directory
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var dir = Path.Combine(documents, Program.Buildname);

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

                if (!File.Exists(Path.Combine(dir, "Trace.txt")))
                {
                    using (File.Create(Path.Combine(dir, "Trace.txt")))
                    { };
                }

                File.AppendAllText(Path.Combine(dir, "Trace.txt"), "[" + DateTime.Now + "]" + " " + Message + Environment.NewLine);
            }
        }
    }
}