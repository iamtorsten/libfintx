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

using libfintx.Data;
using System;

namespace libfintx
{
    public static class HKCAZ
    {
        /// <summary>
        /// Transactions in camt053 format
        /// </summary>
        public static string Init_HKCAZ(ConnectionDetails connectionDetails, string FromDate, string ToDate, string Startpoint)
        {
            Log.Write("Starting job HKCAZ: Request transactions in camt053 format");

            string segments = string.Empty;

            if (String.IsNullOrEmpty(FromDate))
            {
                if (String.IsNullOrEmpty(Startpoint))
                {
                    segments = "HKCAZ:" + SEGNUM.SETVal(3) + ":" + Segment.HKCAZ + "+" + connectionDetails.IBAN + ":" + connectionDetails.BIC + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:camt.052.001.02+N'";
                }
                else
                {
                    segments = "HKCAZ:" + SEGNUM.SETVal(3) + ":" + Segment.HKCAZ + "+" + connectionDetails.IBAN + ":" + connectionDetails.BIC + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:camt.052.001.02+N++++" + Startpoint + "'";
                }
            }
            else
            {
                if (String.IsNullOrEmpty(Startpoint))
                {
                    segments = "HKCAZ:" + SEGNUM.SETVal(3) + ":" + Segment.HKCAZ + "+" + connectionDetails.IBAN + ":" + connectionDetails.BIC + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:camt.052.001.02+N+" + FromDate + "+" + ToDate + "'";
                }
                else
                {
                    segments = "HKCAZ:" + SEGNUM.SETVal(3) + ":" + Segment.HKCAZ + "+" + connectionDetails.IBAN + ":" + connectionDetails.BIC + ":" + connectionDetails.Account + "::280:" + connectionDetails.Blz + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:camt.052.001.02+N+" + FromDate + "+" + ToDate + "++" + Startpoint + "'";
                }
            }

            SEG.NUM = SEGNUM.SETInt(3);

            return FinTSMessage.Send(connectionDetails.Url, FinTSMessage.Create(connectionDetails.HBCIVersion, Segment.HNHBS, Segment.HNHBK, connectionDetails.Blz, connectionDetails.UserId, connectionDetails.Pin, Segment.HISYN, segments, Segment.HIRMS, SEG.NUM));
        }
    }
}
