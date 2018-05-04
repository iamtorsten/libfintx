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

namespace libfintx
{
    public static class FinTSMessageAnonymous
    {
        /// <summary>
        /// Create anonymous FinTS message
        /// </summary>
        /// <param name="Version"></param>
        /// <param name="MsgNum"></param>
        /// <param name="DialogID"></param>
        /// <param name="BLZ"></param>
        /// <param name="UserID"></param>
        /// <param name="PIN"></param>
        /// <param name="SystemID"></param>
        /// <param name="Segments"></param>
        /// <param name="TAN"></param>
        /// <param name="SegmentNum"></param>
        /// <returns></returns>
        public static string Create(int Version, string MsgNum, string DialogID, int BLZ, string UserID, string PIN, 
            string SystemID, string Segments, string TAN, int SegmentNum)
        {
            if (String.IsNullOrEmpty(MsgNum))
                MsgNum = "1";

            MsgNum += "";
            DialogID += "";

            var HEAD_LEN = 29;
            var TRAIL_LEN = 11;

            var msgLen = HEAD_LEN + TRAIL_LEN + MsgNum.Length * 2 + DialogID.Length;

            var paddedLen = ("000000000000").Substring(0, 12 - Convert.ToString(msgLen).Length) + Convert.ToString(msgLen);

            var msgHead = string.Empty;

            msgHead = "HNHBK:1:3+" + paddedLen + "+" + ("300") + "+" + DialogID + "+" + MsgNum + "'";

            var msgEnd = "HNHBS:" + Convert.ToString(SegmentNum + 1) + ":1+" + MsgNum + "'";

            return msgHead + Segments + msgEnd;
        }
    }
}
