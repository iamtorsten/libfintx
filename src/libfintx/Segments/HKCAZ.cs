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

using libfintx.Camt;

namespace libfintx
{
    public static class HKCAZ
    {
        /// <summary>
        /// Transactions in camt053 format
        /// </summary>
        public static string Init_HKCAZ(FinTsClient client, string FromDate, string ToDate, string Startpoint, CamtVersion camtVers)
        {
            string segments = string.Empty;
            var connectionDetails = client.ConnectionDetails;

            client.SEGNUM = SEGNUM.SETInt(3);

            switch (camtVers)
            {
                case CamtVersion.Camt052:
                    Log.Write("Starting job HKCAZ: Request transactions in camt052 format");

                    if (string.IsNullOrEmpty(FromDate))
                    {
                        if (string.IsNullOrEmpty(Startpoint))
                        {
                            segments = "HKCAZ:" + client.SEGNUM + ":" + client.HKCAZ + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+" + CamtScheme.Camt052 + "+N'";
                        }
                        else
                        {
                            segments = "HKCAZ:" + client.SEGNUM + ":" + client.HKCAZ + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+" + CamtScheme.Camt052 + "+N++++" + Startpoint + "'";
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Startpoint))
                        {
                            segments = "HKCAZ:" + client.SEGNUM + ":" + client.HKCAZ + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+" + CamtScheme.Camt052 + "+N+" + FromDate + "+" + ToDate + "'";
                        }
                        else
                        {
                            segments = "HKCAZ:" + client.SEGNUM + ":" + client.HKCAZ + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+" + CamtScheme.Camt052 + "+N+" + FromDate + "+" + ToDate + "++" + Startpoint + "'";
                        }
                    }

                    client.SEGNUM = SEGNUM.SETInt(3);
                    break;

                case CamtVersion.Camt053:
                    Log.Write("Starting job HKCAZ: Request transactions in camt053 format");

                    if (string.IsNullOrEmpty(FromDate))
                    {
                        if (string.IsNullOrEmpty(Startpoint))
                        {
                            segments = "HKCAZ:" + client.SEGNUM + ":" + client.HKCAZ + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+" + CamtScheme.Camt053 + "+N'";
                        }
                        else
                        {
                            segments = "HKCAZ:" + client.SEGNUM + ":" + client.HKCAZ + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+" + CamtScheme.Camt053 + "+N++++" + Startpoint + "'";
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Startpoint))
                        {
                            segments = "HKCAZ:" + client.SEGNUM + ":" + client.HKCAZ + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+" + CamtScheme.Camt053 + "+N+" + FromDate + "+" + ToDate + "'";
                        }
                        else
                        {
                            segments = "HKCAZ:" + client.SEGNUM + ":" + client.HKCAZ + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+" + CamtScheme.Camt053 + "+N+" + FromDate + "+" + ToDate + "++" + Startpoint + "'";
                        }
                    }

                    break;

                default: // -> Will never happen, only for compiler
                    return string.Empty;
            }

            if (Helper.IsTANRequired("HKCAZ"))
            {
                client.SEGNUM = SEGNUM.SETInt(4);
                segments = HKTAN.Init_HKTAN(client, segments);
            }

            string message = FinTSMessage.Create(connectionDetails.HbciVersion, client.HNHBS, client.HNHBK, connectionDetails.BlzPrimary, connectionDetails.UserId, connectionDetails.Pin, client.SystemId, segments, client.HIRMS, client.SEGNUM);
            string response = FinTSMessage.Send(connectionDetails.Url, message);

            client.HITAN = Helper.Parse_String(Helper.Parse_String(response, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
