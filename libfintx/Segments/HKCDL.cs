﻿using libfintx.Data;
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
        public static string Init_HKCDL(ConnectionDetails connectionDetails, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, string OrderId, DateTime FirstTimeExecutionDay, TimeUnit timeUnit, string Rota, int ExecutionDay)
        {
            Log.Write("Starting job HKCDL: Delete bankers order");

            string segments = "HKCDL:" + SEGNUM.SETVal(3) + ":1+" + connectionDetails.IBAN + ":" + connectionDetails.BIC + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.03+@@";

            var sepaMessage = pain00100103.Create(connectionDetails.AccountHolder, connectionDetails.IBAN, connectionDetails.BIC, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, new DateTime(1999, 1, 1));
            sepaMessage = sepaMessage.Replace("'", "") + "++" + OrderId + "+" + FirstTimeExecutionDay.ToString("yyyyMMdd") + ":" + (char)timeUnit + ":" + Rota + ":" + ExecutionDay + "'";

            segments = segments.Replace("@@", "@" + (sepaMessage.Length - 1) + "@") + sepaMessage + "++";

            segments = HKTAN.Init_HKTAN(segments);

            SEG.NUM = SEGNUM.SETInt(4);

            string message = FinTSMessage.Create(connectionDetails.HBCIVersion, Segment.HNHBS, Segment.HNHBK, connectionDetails.Blz, connectionDetails.UserId, connectionDetails.Pin, Segment.HISYN, segments, Segment.HIRMS, SEG.NUM);
            var TAN = FinTSMessage.Send(connectionDetails.Url, message);

            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(TAN, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(TAN);

            return TAN;
        }
    }
}
