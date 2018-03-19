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
using System.Text;
using System.Windows.Forms;

namespace libfintx
{
    public static class MatrixCode
    {
        public static void setCode(byte[] data, PictureBox pictureBox)
        {
            int offset = 0;

            // mimetype
            {
                byte[] b = new byte[2];
                System.Array.Copy(data, offset, b, 0, 2);

                int len = Int32.Parse(Decode(b));
                b = new byte[len];
                offset += 2;

                System.Array.Copy(data, offset, b, 0, len);
                mimetype = Encoding.Default.GetString(b);
                offset += len;
            }

            // image data
            {
                offset += 2;
                int len = data.Length - offset;
                byte[] b = new byte[len];

                System.Array.Copy(data, offset, b, 0, len);

                // TODO: create bitmap and load it into pitureBox image
            }
        }

        private static String Decode(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; ++i)
            {
                sb.Append(Convert.ToString(bytes[i], 10));
            }
            return sb.ToString();
        }

        public static String getMimetype()
        {
            return mimetype;
        }

        private static String mimetype = null;
    }
}