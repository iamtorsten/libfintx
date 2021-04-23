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
using libfintx.Version;

namespace libfintx
{
    public static class HKSYN
    {
        public static async Task<String> Init_HKSYN(FinTsClient client)
        {
            Log.Write("Starting Synchronisation");

            string segments;
            var connectionDetails = client.ConnectionDetails;

            if (connectionDetails.HbciVersion == Convert.ToInt16(HBCI.v220))
            {
                string segments_ =
                    "HKIDN:" + SEG_NUM.Seg3 + ":2+" + SEG_Country.Germany + ":" + connectionDetails.BlzPrimary + "+" + connectionDetails.UserId + "+0+1'" +
                    "HKVVB:" + SEG_NUM.Seg4 + ":2+0+0+0+" + FinTsConfig.ProductId + "+" + FinTsConfig.Version + "'" +
                    "HKSYN:" + SEG_NUM.Seg5 + ":2+0'";

                segments = segments_;
            }
            else if (connectionDetails.HbciVersion == Convert.ToInt16(HBCI.v300))
            {
                string segments_ =
                    "HKIDN:" + SEG_NUM.Seg3 + ":2+" + SEG_Country.Germany + ":" + connectionDetails.BlzPrimary + "+" + connectionDetails.UserId + "+0+1'" +
                    "HKVVB:" + SEG_NUM.Seg4 + ":3+0+0+0+" + FinTsConfig.ProductId + "+" + FinTsConfig.Version + "'" +
                    "HKSYN:" + SEG_NUM.Seg5 + ":3+0'";

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

            client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg5);

            string message = FinTSMessage.CreateSync(client, segments);
            string response = await FinTSMessage.Send(client, message);

            Helper.Parse_Segments(client, response);

            return response;
        }
    }
}
