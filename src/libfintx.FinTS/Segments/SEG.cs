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

namespace libfintx.FinTS
{
    public class SEG
    {
        public string Delimiter = "+";
        public string Terminator = "'";
        public string Finisher = ":";

        public string toSEG(string header, int num, int version, int refNum, string rawData)
        {
            // "HKCAZ:" + client.SEGNUM + ":" + client.HICAZS + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+" + CamtScheme.Camt052 + "+N'"
            string seg = string.Empty;
            seg += header;
            seg += Finisher;
            seg += Convert.ToString(num);
            seg += Finisher;
            seg += Convert.ToString(version);
            seg += Delimiter;
            if (refNum != 0)
            {
                seg += Convert.ToString(refNum);
                seg += Delimiter;
            }
            seg += rawData;
            return seg;
        }
    }
}