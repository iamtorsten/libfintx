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
using System.Threading.Tasks;

namespace libfintx
{
    public static class HKCDE
    {
        /// <summary>
        /// Submit bankers order
        /// </summary>
        public static async Task<String> Init_HKCDE(FinTsClient client, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime FirstTimeExecutionDay, TimeUnit timeUnit, string Rota, int ExecutionDay, DateTime? LastExecutionDay)
        {
            Log.Write("Starting job HKCDE: Submit bankers order");

            client.SEGNUM = SEGNUM.SETInt(3);

            var connectionDetails = client.ConnectionDetails;
            string segments = "HKCDE:" + client.SEGNUM + ":1+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.03+@@";

            var sepaMessage = pain00100103.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, new DateTime(1999, 1, 1)).Replace("'", "");
            segments = segments.Replace("@@", "@" + sepaMessage.Length + "@") + sepaMessage;

            segments += "+" + FirstTimeExecutionDay.ToString("yyyyMMdd") + ":" + (char) timeUnit + ":" + Rota + ":" + ExecutionDay;
            if (LastExecutionDay != null)
                segments += ":" + LastExecutionDay.Value.ToString("yyyyMMdd");

            segments += "'";

            if (Helper.IsTANRequired("HKCDE"))
            {
                client.SEGNUM = SEGNUM.SETInt(4);
                segments = HKTAN.Init_HKTAN(client, segments);
            }

            string message = FinTSMessage.Create(client, client.HNHBS, client.HNHBK, segments, client.HIRMS);
            var response = await FinTSMessage.Send(client, message);

            Helper.Parse_Message(client, response);

            return response;
        }

        public enum TimeUnit
        {
            Monthly = 'M',
            Weekly = 'W'
        }
    }
}
