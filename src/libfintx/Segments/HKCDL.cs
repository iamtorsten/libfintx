using libfintx.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static libfintx.HKCDE;

namespace libfintx
{
    public class HKCDL
    {
        /// <summary>
        /// Delete banker's order
        /// </summary>
        public static string Init_HKCDL(ConnectionDetails connectionDetails, string OrderId, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime FirstTimeExecutionDay, TimeUnit timeUnit, string Rota, int ExecutionDay, DateTime? LastExecutionDay)
        {
            Log.Write("Starting job HKCDL: Delete bankers order");

            SEG.NUM = SEGNUM.SETInt(3);

            string segments = "HKCDL:" + SEG.NUM + ":1+" + connectionDetails.IBAN + ":" + connectionDetails.BIC + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.03+@@";

            var sepaMessage = pain00100103.Create(connectionDetails.AccountHolder, connectionDetails.IBAN, connectionDetails.BIC, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, new DateTime(1999, 1, 1)).Replace("'", "");
            segments = segments.Replace("@@", "@" + sepaMessage.Length + "@") + sepaMessage;

            segments += "++" + OrderId + "+" + FirstTimeExecutionDay.ToString("yyyyMMdd") + ":" + (char)timeUnit + ":" + Rota + ":" + ExecutionDay;
            if (LastExecutionDay != null)
                segments += ":" + LastExecutionDay.Value.ToString("yyyyMMdd");

            segments += "'";

            if (Helper.IsTANRequired("HKCDL"))
            {
                SEG.NUM = SEGNUM.SETInt(4);
                segments = HKTAN.Init_HKTAN(segments);
            }

            string message = FinTSMessage.Create(connectionDetails.HBCIVersion, Segment.HNHBS, Segment.HNHBK, connectionDetails.BlzPrimary, connectionDetails.UserId, connectionDetails.Pin, Segment.HISYN, segments, Segment.HIRMS, SEG.NUM);
            var TAN = FinTSMessage.Send(connectionDetails.Url, message);

            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(TAN, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(TAN);

            return TAN;
        }
    }
}
