using System;
using static libfintx.HKCDE;

namespace libfintx
{
    public class HKCDN
    {
        /// <summary>
        /// Submit bankers order
        /// </summary>
        public static string Init_HKCDN(FinTsClient client, string OrderId, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime FirstTimeExecutionDay, TimeUnit timeUnit, string Rota, int ExecutionDay, DateTime? LastExecutionDay)
        {
            Log.Write("Starting job HKCDN: Modify bankers order");

            client.SEGNUM = SEGNUM.SETInt(3);

            var connectionDetails = client.ConnectionDetails;
            string segments = "HKCDN:" + client.SEGNUM + ":1+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.03+@@";

            var sepaMessage = pain00100103.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, new DateTime(1999, 1, 1)).Replace("'", "");
            segments = segments.Replace("@@", "@" + sepaMessage.Length + "@") + sepaMessage;

            segments += "++" + OrderId + "+" + FirstTimeExecutionDay.ToString("yyyyMMdd") + ":" + (char) timeUnit + ":" + Rota + ":" + ExecutionDay;
            if (LastExecutionDay != null)
                segments += ":" + LastExecutionDay.Value.ToString("yyyyMMdd");

            segments += "'";

            if (Helper.IsTANRequired("HKCDN"))
            {
                client.SEGNUM = SEGNUM.SETInt(4);
                segments = HKTAN.Init_HKTAN(client, segments);
            }

            string message = FinTSMessage.Create(connectionDetails.HbciVersion, client.HNHBS, client.HNHBK, connectionDetails.BlzPrimary, connectionDetails.UserId, connectionDetails.Pin, client.SystemId, segments, client.HIRMS, client.SEGNUM);
            var TAN = FinTSMessage.Send(connectionDetails.Url, message);

            client.HITAN = Helper.Parse_String(Helper.Parse_String(TAN, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(client, TAN);

            return TAN;
        }
    }
}
