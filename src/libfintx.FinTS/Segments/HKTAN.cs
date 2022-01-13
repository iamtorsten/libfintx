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
using libfintx.FinTS.Segments;

namespace libfintx.FinTS
{
    public static class HKTAN
    {
        /// <summary>
        /// Set tan process
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        public static string Init_HKTAN(FinTsClient client, string segments, string segmentId)
        {
            SEG sEG = new SEG();

            if (String.IsNullOrEmpty(client.HITAB)) // TAN Medium Name not set
            {
                // Erweiterung decoupled
                // Torsten: Gemäß meiner Auffassung sendet HTAN#7 das Segment deckungsgleich HKTAN#6
                if (client.HITANS >= 6)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(segmentId);
                    sb.Append(sEG.Terminator);
                    segments = segments + sEG.toSEG(new SEG_DATA
                    {
                        Header = "HKTAN",
                        Num = client.SEGNUM,
                        Version = client.HITANS,
                        RefNum = 4,
                        RawData = sb.ToString()
                    });
                }
                //segments = segments + "HKTAN:" + client.SEGNUM + ":" + client.HITANS + "+4+" + segmentId + "'";
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(sEG.Terminator);
                    segments = segments + sEG.toSEG(new SEG_DATA
                    {
                        Header = "HKTAN",
                        Num = client.SEGNUM,
                        Version = client.HITANS,
                        RefNum = 4,
                        RawData = sb.ToString()
                    });
                }
                //segments = segments + "HKTAN:" + client.SEGNUM + ":" + client.HITANS + "+4+'";
            }
            else // TAN Medium Name set
            {
                // Version 3, Process 4
                if (client.HITANS == 3)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAB);
                    sb.Append(sEG.Terminator);
                    segments = segments + sEG.toSEG(new SEG_DATA
                    {
                        Header = "HKTAN",
                        Num = client.SEGNUM,
                        Version = client.HITANS,
                        RefNum = 4,
                        RawData = sb.ToString()
                    });
                }
                //segments = segments + "HKTAN:" + client.SEGNUM + ":" + client.HITANS + "+4++++++++" + client.HITAB + "'";
                // Version 4, Process 4
                if (client.HITANS == 4)
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
                    sb.Append(client.HITAB);
                    sb.Append(sEG.Terminator);
                    segments = segments + sEG.toSEG(new SEG_DATA
                    {
                        Header = "HKTAN",
                        Num = client.SEGNUM,
                        Version = client.HITANS,
                        RefNum = 4,
                        RawData = sb.ToString()
                    });
                }
                    
                //segments = segments + "HKTAN:" + client.SEGNUM + ":" + client.HITANS + "+4+++++++++" + client.HITAB + "'";
                // Version 5, Process 4
                if (client.HITANS == 5)
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
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAB);
                    sb.Append(sEG.Terminator);
                    segments = segments + sEG.toSEG(new SEG_DATA
                    {
                        Header = "HKTAN",
                        Num = client.SEGNUM,
                        Version = client.HITANS,
                        RefNum = 4,
                        RawData = sb.ToString()
                    });
                }  
                //segments = segments + "HKTAN:" + client.SEGNUM + ":" + client.HITANS + "+4+++++++++++" + client.HITAB + "'";
                // Version 6, Process 4
                if (client.HITANS == 6)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(segmentId);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAB);
                    sb.Append(sEG.Terminator);
                    segments = segments + sEG.toSEG(new SEG_DATA
                    {
                        Header = "HKTAN",
                        Num = client.SEGNUM,
                        Version = client.HITANS,
                        RefNum = 4,
                        RawData = sb.ToString()
                    });
                    // segments = segments + "HKTAN:" + client.SEGNUM + ":" + client.HITANS + "+4+" + segmentId + "+++++++++" + client.HITAB + "'";
                }
                // Version 7, Process 4
                // Erweiterung decoupled
                // Torsten: Gemäß meiner Auffassung sendet HTAN#7 das Segment deckungsgleich HKTAN#6
                if (client.HITANS == 7)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(segmentId);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(sEG.Delimiter);
                    sb.Append(client.HITAB);
                    sb.Append(sEG.Terminator);
                    segments = segments + sEG.toSEG(new SEG_DATA
                    {
                        Header = "HKTAN",
                        Num = client.SEGNUM,
                        Version = client.HITANS,
                        RefNum = 4,
                        RawData = sb.ToString()
                    });
                    //segments = segments + "HKTAN:" + client.SEGNUM + ":" + client.HITANS + "+4+" + segmentId + "+++++++++" + client.HITAB + "'";
                }
            }

            return segments;
        }
    }
}
