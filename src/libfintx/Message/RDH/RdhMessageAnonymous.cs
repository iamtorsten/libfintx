/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2017 Torsten Klinger
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
    public static class RdhMessageAnonymous
    {
        public static string Create(int Version, string MsgNum, string DialogID, int BLZ, string Segments)
        {
            if (String.IsNullOrEmpty(MsgNum))
                MsgNum = "1";

            MsgNum += "";
            DialogID += "";

            var HEAD_LEN = 29;
            var TRAIL_LEN = 11;

            var Segments_ = string.Empty;

            if (Version == 300)
            {
                Segments_ = "HKISA:" + SEGNUM.SETVal(4) + ":3+2+124+" + RdhProfile.RDHPROFILE + "+" + SEG_Country.Germany + ":" + BLZ + ":999:V:999:999'" +
                    "HKISA:" + SEGNUM.SETVal(5) + ":3+2+124+" + RdhProfile.RDHPROFILE + "+" + SEG_Country.Germany + ":" + BLZ + ":999:S:999:999'";
            }
            else
                throw new Exception("HBCI version not supported");

            Segments = Segments + Segments_;

            var msgLen = HEAD_LEN + TRAIL_LEN + MsgNum.Length * 2 + DialogID.Length + Segments.Length;

            var paddedLen = ("000000000000").Substring(0, 12 - Convert.ToString(msgLen).Length) + Convert.ToString(msgLen);

            var msgHead = string.Empty;

            if (Version == 300)
            {
                msgHead = "HNHBK:" + SEGNUM.SETVal(1) + ":3+" + paddedLen + "+" + ("300") + "+" + DialogID + "+" + MsgNum + "'";
            }
            else
                return string.Empty;

            var msgEnd = "HNHBS:" + SEGNUM.SETVal(6) + ":1+" + MsgNum + "'";

            return msgHead + Segments + msgEnd;
        }
    }
}