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

using libfintx.Data;
using System;

namespace libfintx
{
    public static class HKCDE
    {
        /// <summary>
        /// Submit bankers order
        /// </summary>
        public static string Init_HKCDE(ConnectionDetails connectionDetails, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime FirstTimeExecutionDay, TimeUnit timeUnit, string Rota, int ExecutionDay)
        {
            Log.Write("Starting job HKCDE: Submit bankers order");

            string segments = "HKCDE:" + SEGNUM.SETVal(3) + ":1+" + connectionDetails.IBAN + ":" + connectionDetails.BIC + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.03+@@";

            var sepaMessage = pain00100103.Create(connectionDetails.AccountHolder, connectionDetails.IBAN, connectionDetails.BIC, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, new DateTime(1999, 1, 1)).Replace("'", "");
            segments = segments.Replace("@@", "@" + sepaMessage.Length + "@") + sepaMessage;

            segments += "+" + FirstTimeExecutionDay.ToString("yyyyMMdd") + ":" + (char)timeUnit + ":" + Rota + ":" + ExecutionDay + "'";

            segments = HKTAN.Init_HKTAN(segments);

            SEG.NUM = SEGNUM.SETInt(4);

            string message = FinTSMessage.Create(connectionDetails.HBCIVersion, Segment.HNHBS, Segment.HNHBK, connectionDetails.Blz, connectionDetails.UserId, connectionDetails.Pin, Segment.HISYN, segments, Segment.HIRMS, SEG.NUM);
            var TAN = FinTSMessage.Send(connectionDetails.Url, message);

            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(TAN, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(TAN);

            return TAN;
        }

        public enum TimeUnit
        {
            Monthly = 'M',
            Weekly = 'W'
        }
    }
}
