/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2020 Torsten Klinger
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

namespace libfintx
{
    public static class Tan4
    {
        /// <summary>
        /// TAN process 4
        /// </summary>
        public static string Send_TAN4(FinTsClient client, string TAN, string MediumName)
        {
            Log.Write("Starting job TAN process 4");

            var connectionDetails = client.ConnectionDetails;
            string segments = string.Empty;

            var HITANS = !String.IsNullOrEmpty(client.HITANS.Substring(0, 1)) ? int.Parse(client.HITANS.Substring(0, 1)) : 0;

            // Version 3
            if (HITANS == 3)
                segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + client.HITANS.Substring(0, 1) + "+4+++++++" + MediumName + "'";
            // Version 4
            if (HITANS == 4)
                segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + client.HITANS.Substring(0, 1) + "+4++++++++" + MediumName + "'";
            // Version 5
            if (HITANS == 5)
                segments = "HKTAN:" + SEGNUM.SETVal(3) + ":" + client.HITANS.Substring(0, 1) + "+4++++++++++" + MediumName + "'";

            SEG.NUM = SEGNUM.SETInt(3);

            return FinTSMessage.Send(connectionDetails.Url, FinTSMessage.Create(connectionDetails.HbciVersion, client.HNHBS, client.HNHBK, connectionDetails.BlzPrimary, connectionDetails.UserId, connectionDetails.Pin, client.SystemId, segments, client.HIRMS + ":" + TAN, SEG.NUM));
        }
    }
}
