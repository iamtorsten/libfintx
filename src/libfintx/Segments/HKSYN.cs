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
    public static class HKSYN
    {
        public static string Init_HKSYN(FinTsClient client)
        {
            Log.Write("Starting Synchronisation");

            string segments;
            var connectionDetails = client.ConnectionDetails;

            if (connectionDetails.HbciVersion == 220)
            {
                string segments_ =
                    "HKIDN:" + SEGNUM.SETVal(3) + ":2+280:" + connectionDetails.BlzPrimary + "+" + connectionDetails.UserId + "+0+1'" +
                    "HKVVB:" + SEGNUM.SETVal(4) + ":2+0+0+0+" + FinTsConfig.ProductId + "+" + FinTsConfig.Version + "'" +
                    "HKSYN:" + SEGNUM.SETVal(5) + ":2+0'";

                segments = segments_;
            }
            else if (connectionDetails.HbciVersion == 300)
            {
                string segments_ =
                    "HKIDN:" + SEGNUM.SETVal(3) + ":2+280:" + connectionDetails.BlzPrimary + "+" + connectionDetails.UserId + "+0+1'" +
                    "HKVVB:" + SEGNUM.SETVal(4) + ":3+0+0+0+" + FinTsConfig.ProductId + "+" + FinTsConfig.Version + "'" +
                    "HKSYN:" + SEGNUM.SETVal(5) + ":3+0'";

                segments = segments_;
            }
            else
            {
                //Since connectionDetails is a re-usable object, this shouldn't be cleared.
                //connectionDetails.UserId = string.Empty;
                //connectionDetails.Pin = null;

                Log.Write("HBCI version not supported");

                throw new Exception("HBCI version not supported");
            }

            SEG.NUM = SEGNUM.SETInt(5);

            string message = FinTSMessage.Create(connectionDetails.HbciVersion, "1", "0", connectionDetails.BlzPrimary, connectionDetails.UserId, connectionDetails.Pin, "0", segments, null, SEG.NUM);
            string response = FinTSMessage.Send(connectionDetails.Url, message);

            Helper.Parse_Segment(client, response);

            return response;
        }
    }
}
