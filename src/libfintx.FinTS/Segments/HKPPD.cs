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
using System.Threading.Tasks;
using libfintx.FinTS.Data;
using libfintx.FinTS.Message;
using libfintx.Logger.Log;

namespace libfintx.FinTS
{
    public static class HKPPD
    {
        /// <summary>
        /// Load prepaid
        /// </summary>
        public static async Task<String> Init_HKPPD(FinTsClient client, int MobileServiceProvider, string PhoneNumber,
            int Amount)
        {
            Log.Write("Starting job HKPPD: Load prepaid");

            client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg3);

            var connectionDetails = client.ConnectionDetails;
            SEG sEG = new SEG();
            string segments = sEG.toSEG("HKPPD",
                client.SEGNUM,
                2,
                0,
                connectionDetails.Iban +
                DEG.Separator +
                connectionDetails.Bic +
                sEG.Delimiter +
                MobileServiceProvider +
                sEG.Delimiter +
                PhoneNumber +
                sEG.Delimiter +
                Amount +
                ",:EUR" +
                sEG.Terminator);
            //string segments = "HKPPD:" + client.SEGNUM + ":2+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+" + MobileServiceProvider + "+" + PhoneNumber + "+" + Amount + ",:EUR'";

            if (Helper.IsTANRequired("HKPPD"))
            {
                client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg4);
                segments = HKTAN.Init_HKTAN(client, segments, "HKPPD");
            }

            string message = FinTSMessage.Create(client, client.HNHBS, client.HNHBK, segments, client.HIRMS);
            var response = await FinTSMessage.Send(client, message);

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
