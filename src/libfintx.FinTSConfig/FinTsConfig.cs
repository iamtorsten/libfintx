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

using libfintx.Logger.Debug;
using libfintx.Logger.Log;
using libfintx.Logger.Trace;

namespace libfintx.FinTSConfig
{
    public static class Config
    {
        /// <summary>	
        /// Enable / Disable Tracing	
        /// </summary>	
        public static void Tracing(bool Enabled, bool Formatted = false, bool maskCredentials = true, int maxFileSizeMB = 1)
        {
            Trace.Enabled = Enabled;
            Trace.Formatted = Formatted;
            Trace.MaskCredentials = maskCredentials;
            Trace.MaxFileSize = maxFileSizeMB;
        }

        /// <summary>	
        /// Enable / Disable Debugging	
        /// </summary>	
        public static void Debugging(bool Enabled)
        {
            DEBUG.Enabled = Enabled;
        }

        /// <summary>	
        /// Enable / Disable Logging	
        /// </summary>	
        public static void Logging(bool Enabled, int maxFileSizeMB = 10)
        {
            Log.Enabled = Enabled;
            Log.MaxFileSize = maxFileSizeMB;
        }
    }
}
