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
    public static class HKCCS
    {
        /// <summary>
        /// Transfer
        /// </summary>
        public static string Init_HKCCS(ConnectionDetails connectionDetails, string ReceiverName, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage)
        {
            Log.Write("Starting job HKCCS: Transfer money");

            string segments = string.Empty;

            string sepaMessage = string.Empty;

            SEG.NUM = SEGNUM.SETInt(3);

            if (Segment.HISPAS == 1)
            {
                segments = "HKCCS:" + SEG.NUM + ":1+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.03+@@";
                sepaMessage = pain00100103.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, ReceiverName, ReceiverIBAN, ReceiverBIC, Amount, Usage, new DateTime(1999, 1, 1));
            }
            else if (Segment.HISPAS == 2)
            {
                segments = "HKCCS:" + SEG.NUM + ":1+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.002.03+@@";
                sepaMessage = pain00100203.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, ReceiverName, ReceiverIBAN, ReceiverBIC, Amount, Usage, new DateTime(1999, 1, 1));
            }  
            else if (Segment.HISPAS == 3)
            {
                segments = "HKCCS:" + SEG.NUM + ":1+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.003.03+@@";
                sepaMessage = pain00100303.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, ReceiverName, ReceiverIBAN, ReceiverBIC, Amount, Usage, new DateTime(1999, 1, 1));
            }

            segments = segments.Replace("@@", "@" + (sepaMessage.Length - 1) + "@") + sepaMessage;

            if (Helper.IsTANRequired("HKCCS"))
            {
                SEG.NUM = SEGNUM.SETInt(4);
                segments = HKTAN.Init_HKTAN(segments);
            }

            var message = FinTSMessage.Create(connectionDetails.HbciVersion, Segment.HNHBS, Segment.HNHBK, connectionDetails.BlzPrimary, connectionDetails.UserId, connectionDetails.Pin, Segment.HISYN, segments, Segment.HIRMS, SEG.NUM);
            var response = FinTSMessage.Send(connectionDetails.Url, message);

            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(response, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(response);

            return response;
        }
    }
}
