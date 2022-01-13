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
using System.Text;
using System.Threading.Tasks;
using libfintx.FinTS.Message;
using libfintx.FinTS.Segments;
using libfintx.Logger.Log;

namespace libfintx.FinTS
{
    public static class Tan4
    {
        /// <summary>
        /// TAN process 4
        /// </summary>
        public static async Task<String> Send_TAN4(FinTsClient client, string TAN, string MediumName)
        {
            Log.Write("Starting job TAN process 4");

            string segments = string.Empty;

            SEG sEG = new SEG();

            // Version 3
            if (client.HITANS == 3)
            {
                //segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HITANS + "+4+++++++" + MediumName + "'";
                StringBuilder sb = new StringBuilder();
                sb.Append(sEG.Delimiter);
                sb.Append(sEG.Delimiter);
                sb.Append(sEG.Delimiter);
                sb.Append(sEG.Delimiter);
                sb.Append(sEG.Delimiter);
                sb.Append(sEG.Delimiter);
                sb.Append(MediumName);
                sb.Append(sEG.Terminator);
                segments = sEG.toSEG(new SEG_DATA
                {
                    Header = "HKTAN",
                    Num = Convert.ToInt16(SEG_NUM.Seg3),
                    Version = client.HITANS,
                    RefNum = 4,
                    RawData = sb.ToString()
                });
            }
                
            // Version 4
            else if (client.HITANS == 4)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(sEG.Delimiter);
                sb.Append(sEG.Delimiter );
                sb.Append(sEG.Delimiter );
                sb.Append(sEG.Delimiter );
                sb.Append(sEG.Delimiter );
                sb.Append(sEG.Delimiter );
                sb.Append(sEG.Delimiter );
                sb.Append(MediumName );
                sb.Append(sEG.Terminator);
                segments = sEG.toSEG(new SEG_DATA
                {
                    Header = "HKTAN",
                    Num = Convert.ToInt16(SEG_NUM.Seg3),
                    Version = client.HITANS,
                    RefNum = 4,
                    RawData = sb.ToString()
                });
            }
                
            //segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HITANS + "+4++++++++" + MediumName + "'";
            // Version 5
            else if (client.HITANS == 5)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(sEG.Delimiter);
                sb.Append(sEG.Delimiter);
                sb.Append(sEG.Delimiter);
                sb.Append(sEG.Delimiter);
                sb.Append(sEG.Delimiter);
                sb.Append(sEG.Delimiter);
                sb.Append(sEG.Delimiter);
                sb.Append(sEG.Delimiter);
                sb.Append(sEG.Delimiter);
                sb.Append(MediumName);
                sb.Append(sEG.Terminator);
                segments = sEG.toSEG(new SEG_DATA
                {
                    Header = "HKTAN",
                    Num = Convert.ToInt16(SEG_NUM.Seg3),
                    Version = client.HITANS,
                    RefNum = 4,
                    RawData = sb.ToString()
                });
            }
            //segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HITANS + "+4++++++++++" + MediumName + "'";

            client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg3);

            return await FinTSMessage.Send(client, FinTSMessage.Create(client, client.HNHBS, client.HNHBK,
                segments, client.HIRMS + ":" + TAN));
        }
    }
}
