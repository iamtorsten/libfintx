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

/*
 *
 *	Based on Olaf Willuhn's Java implementation of flicker code rendering,
 *	available at https://github.com/willuhn/hbci4java/blob/master/src/org/kapott/hbci/manager/FlickerRenderer.java
 *
 */


using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using System;
using System.Collections.Generic;
using System.Threading;
using CoreRectangle = SixLabors.ImageSharp.Rectangle;

namespace libfintx.FinTS
{
    public class FlickerCodeRenderer
    {
        /// <summary>
        /// Default clock frequency in Hz
        /// </summary>
        public const int FREQUENCY_DEFAULT = 10;

        /// <summary>
        /// Minimal clock frequency
        /// </summary>
        public const int FREQUENCY_MIN = 2;

        /// <summary>
        /// Maximal clock frequency
        /// </summary>
        public const int FREQUENCY_MAX = 40;

        private readonly IList<byte[]> _bitArray;

        public FlickerCodeRenderer(string code)
        {
            // Sync-ID
            code = "0FFF" + code;

            // Bitfield with BCD-Codierung
            IDictionary<string, byte[]> bcdmap = new Dictionary<string, byte[]>();
            bcdmap["0"] = new byte[] { 0, 0, 0, 0, 0 };
            bcdmap["1"] = new byte[] { 0, 1, 0, 0, 0 };
            bcdmap["2"] = new byte[] { 0, 0, 1, 0, 0 };
            bcdmap["3"] = new byte[] { 0, 1, 1, 0, 0 };
            bcdmap["4"] = new byte[] { 0, 0, 0, 1, 0 };
            bcdmap["5"] = new byte[] { 0, 1, 0, 1, 0 };
            bcdmap["6"] = new byte[] { 0, 0, 1, 1, 0 };
            bcdmap["7"] = new byte[] { 0, 1, 1, 1, 0 };
            bcdmap["8"] = new byte[] { 0, 0, 0, 0, 1 };
            bcdmap["9"] = new byte[] { 0, 1, 0, 0, 1 };
            bcdmap["A"] = new byte[] { 0, 0, 1, 0, 1 };
            bcdmap["B"] = new byte[] { 0, 1, 1, 0, 1 };
            bcdmap["C"] = new byte[] { 0, 0, 0, 1, 1 };
            bcdmap["D"] = new byte[] { 0, 1, 0, 1, 1 };
            bcdmap["E"] = new byte[] { 0, 0, 1, 1, 1 };
            bcdmap["F"] = new byte[] { 0, 1, 1, 1, 1 };

            // Swap left and right char of each byte
            this._bitArray = new List<byte[]>();
            for (int i = 0; i < code.Length; i += 2)
            {
                _bitArray.Add(bcdmap[Convert.ToString(code[i + 1])]);
                _bitArray.Add(bcdmap[Convert.ToString(code[i])]);
            }
        }

        public Image<Rgba32> RenderAsGif(int width = 320, int height = 120, int margin = 7)
        {
            using (var image = new Image<Rgba32>(width, height))
            {
                int barwidth = width / 5;
                for (int arrayIndex = 0; arrayIndex < _bitArray.Count; arrayIndex++)
                {
                    for (byte cl = 1; cl >= 0; cl--)
                    {
                        // Weird flex move that sets the 5th bit first to 1 then to 0
                        var bits = this._bitArray[arrayIndex];
                        bits[0] = cl;

                        // Build Frame
                        using (var frame = new Image<Rgba32>(width, height))
                        {
                            for (int byteIndex = 0; byteIndex < 5; byteIndex++)
                            {
                                var color = _bitArray[arrayIndex][byteIndex] == 1 ? Color.White :Color.Black;
                                var bounds = new CoreRectangle(byteIndex * barwidth + margin, margin, barwidth - 2 * margin, height - 2 * margin);
                                frame.Mutate(x => x.Fill(color, bounds));
                            }
                            image.Frames.AddFrame(frame.Frames[0]);
                        }
                    }
                }
                return image;
            }
        }
    }
}
