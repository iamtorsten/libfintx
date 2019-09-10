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

            if (Helper.IsTANRequired(connectionDetails.BPD, "HKCDB"))
                segments = HKTAN.Init_HKTAN(segments);

            SEG.NUM = SEGNUM.SETInt(3);

            string message = FinTSMessage.Create(connectionDetails.HBCIVersion, Segment.HNHBS, Segment.HNHBK, connectionDetails.BlzPrimary, connectionDetails.UserId, connectionDetails.Pin, Segment.HISYN, segments, Segment.HIRMS, SEG.NUM);
            string response = FinTSMessage.Send(connectionDetails.Url, message);

            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(response, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(response);

            return response;
        }
    }
}
