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

using System;
using System.Threading.Tasks;
using libfintx.FinTS.Message;
using libfintx.Logger.Log;

namespace libfintx.FinTS
{
    public static class HKSPA
    {
        /// <summary>
        /// Request SEPA account connection
        /// </summary>
        /// <param name="connectionDetails"></param>
        /// <returns></returns>
        public static async Task<String> Init_HKSPA(FinTsClient client)
        {
            Log.Write("Starting job HKSPA: Request SEPA account connection");

            var connectionDetails = client.ConnectionDetails;
            string segments = string.Empty;

            segments = "HKEND:" + SEG_NUM.Seg3 + "1'";

            client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg3);

            return await FinTSMessage.Send(client, FinTSMessage.Create(client, client.HNHBS, client.HNHBK, segments, client.HIRMS));
        }
    }
}
