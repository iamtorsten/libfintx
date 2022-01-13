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

// #define WINDOWS

using libfintx.Logger.Log;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Text;

namespace libfintx.FinTS
{
    public class MatrixCode
    {
        public Image<Rgba32> CodeImage { get; set; }
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
                var ms = new MemoryStream(b);
                CodeImage = (Image<Rgba32>) Image.Load(ms);
            }
            catch (Exception ex)
            {
                var errMsg = $"Invalid photoTan image returned. Error: {ex.Message}";
                Log.Write(errMsg);
                throw new Exception(errMsg, ex);
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

        public void Render(object pictureBox)
        {
            if (pictureBox == null)
                return;
        }
    }
}
