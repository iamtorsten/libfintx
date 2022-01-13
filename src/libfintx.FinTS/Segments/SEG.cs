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
using libfintx.FinTS.Data;
using libfintx.FinTS.Segments;

namespace libfintx.FinTS
{
    public partial class SEG
    {
        public string Delimiter = "+";
        public string Terminator = "'";

        public string toSEG(SEG_DATA sEG_DATA)
        {
            // "HKCAZ:" + client.SEGNUM + ":" + client.HICAZS + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+" + CamtScheme.Camt052 + "+N'"
            string seg = string.Empty;
            seg += sEG_DATA.Header;
            seg += DEG.Separator;
            seg += Convert.ToString(sEG_DATA.Num);
            seg += DEG.Separator;
            seg += Convert.ToString(sEG_DATA.Version);
            seg += Delimiter;
            if (sEG_DATA.RefNum != 0)
            {
                seg += Convert.ToString(sEG_DATA.RefNum);
                seg += Delimiter;
            }
            seg += sEG_DATA.RawData;
            return seg;
        }
    }
}