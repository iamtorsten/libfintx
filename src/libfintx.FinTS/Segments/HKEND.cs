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
using System.Text;
using System.Threading.Tasks;
using libfintx.FinTS.Data;
using libfintx.FinTS.Message;
using libfintx.Logger.Log;

namespace libfintx.FinTS
{
    public static class HKEND
    {
        /// <summary>
        /// End of dialog
        /// </summary>
        /// <param name="connectionDetails"></param>
        /// <param name="dialogID"></param>
        public static async Task<String> Init_HKEND(FinTsClient client, string dialogID)
        {
            Log.Write("Starting job HKEND: End of dialog");

            var connectionDetails = client.ConnectionDetails;
            string segments = string.Empty;

            //segments = "HKEND:" + SEG_NUM.Seg3 + ":1+" + dialogID + "'";

            SEG sEG = new SEG();
            StringBuilder sb = new StringBuilder();
            sb.Append("HKEND");
            sb.Append(DEG.Separator);
            sb.Append(SEG_NUM.Seg3);
            sb.Append(DEG.Separator);
            sb.Append("1");
            sb.Append(sEG.Delimiter);
            sb.Append(dialogID);
            sb.Append(sEG.Terminator);
            segments = sb.ToString();

            client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg3);

            string message = FinTSMessage.Create(client, client.HNHBS, client.HNHBK, segments, client.HIRMS);
            return await FinTSMessage.Send(client, message);
        }
    }
}
