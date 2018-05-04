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
using System.Drawing;
using System.IO;
using System.Text;

namespace libfintx
{
    public class MatrixCode
    {
        public Image CodeImage { get; set; }
        public string ImageMimeType { get; set; }

        /// <summary>
        /// photoTAN matrix code
        /// </summary>
        /// <param name="photoTanString"></param>
        public MatrixCode(string photoTanString)
        {
            try
            {
                var data = Encoding.GetEncoding("ISO-8859-1").GetBytes(photoTanString);
                int offset = 0;

                //Read mimetype            
                byte[] b = new byte[2];
                Array.Copy(data, offset, b, 0, 2);

                int mimeTypeLen = Int32.Parse(Decode(b));
                b = new byte[mimeTypeLen];
                offset += 2;

                Array.Copy(data, offset, b, 0, mimeTypeLen);
                ImageMimeType = Encoding.Default.GetString(b);
                offset += mimeTypeLen;
            
                //Read image data            
                offset += 2;
                int len = data.Length - offset;
                b = new byte[len];
                Array.Copy(data, offset, b, 0, len);
                MemoryStream ms = new MemoryStream(b);
                CodeImage = Image.FromStream(ms);                
            }
            catch (Exception ex)
            {
                var errMsg = $"Invalid photoTan image returned. Error: {ex.Message}";
                Log.Write(errMsg);
                throw new Exception(errMsg);
            }
            
        }

        /// <summary>
        /// Internal decode picture format
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private string Decode(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; ++i)
            {
                sb.Append(Convert.ToString(bytes[i], 10));
            }
            return sb.ToString();
        }
    }
}