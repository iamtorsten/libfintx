using libfintx.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libfintx
{
    public static class HKCDB
    {
        /// <summary>
        /// Get bankers orders
        /// </summary>
        public static string Init_HKCDB(ConnectionDetails connectionDetails)
        {
            Log.Write("Starting job HKCDB: Get bankers order");

            string segments = "HKCDB:" + SEGNUM.SETVal(3) + ":1+" + connectionDetails.IBAN + ":" + connectionDetails.BIC + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.03'";

            SEG.NUM = SEGNUM.SETInt(3);

            string message = FinTSMessage.Create(connectionDetails.HBCIVersion, Segment.HNHBS, Segment.HNHBK, connectionDetails.Blz, connectionDetails.UserId, connectionDetails.Pin, Segment.HISYN, segments, Segment.HIRMS, SEG.NUM);
            return FinTSMessage.Send(connectionDetails.Url, message);
        }
    }
}
