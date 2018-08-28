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

namespace libfintx
{
    public static class Log
    {
        public static bool Enabled { get; set; }

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
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var dir = Path.Combine(documents, Program.Buildname);

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

                if (!File.Exists(Path.Combine(dir, "Log.txt")))
                {
                    using (File.Create(Path.Combine(dir, "Log.txt")))
                    { };
                }

                File.AppendAllText(Path.Combine(dir, "Log.txt"), "[" + DateTime.Now + "]" + " " + Message + Environment.NewLine);
            }
        }
    }
}