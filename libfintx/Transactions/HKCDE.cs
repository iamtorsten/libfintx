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

namespace libfintx
{
    public static class HKCDE
    {
        /// <summary>
        /// Submit bankers order
        /// </summary>
        public static string Init_HKCDE(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC, string Receiver,
            string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, string FirstTimeExecutionDay, string TimeUnit, string Rota,
            string ExecutionDay, string URL, int HBCIVersion, string UserID, string PIN)
        {
            Log.Write("Starting job HKCDE: Submit bankers order");

            string segments = "HKCDE:" + SEGNUM.RETVal(3) + ":1+" + AccountholderIBAN + ":" + AccountholderBIC + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.002.03+@@";

            var message = pain00100203.Create(Accountholder, AccountholderIBAN, AccountholderBIC, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, "1999-01-01");

            message = message.Replace("'", "") + "+" + FirstTimeExecutionDay + ":" + TimeUnit + ":" + Rota + ":" + ExecutionDay + "'";

            segments = segments.Replace("@@", "@" + (message.Length - 1) + "@") + message;

            segments = segments + "HKTAN:" + SEGNUM.RETVal(4) + ":" + Segment.HITANS + "'";

            SEG.NUM = 4;

            var TAN = FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS, SEG.NUM));

            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(TAN, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(TAN);

            return TAN;
        }
    }
}
