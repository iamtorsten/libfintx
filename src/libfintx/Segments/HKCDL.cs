using System;
using System.Threading.Tasks;
using static libfintx.HKCDE;

namespace libfintx
{
    public class HKCDL
    {
        /// <summary>
        /// Delete banker's order
        /// </summary>
        public static async Task<String> Init_HKCDL(FinTsClient client, string OrderId, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime FirstTimeExecutionDay, TimeUnit timeUnit, string Rota, int ExecutionDay, DateTime? LastExecutionDay)
        {
            Log.Write("Starting job HKCDL: Delete bankers order");

            client.SEGNUM = SEGNUM.SETInt(3);
            var connectionDetails = client.ConnectionDetails;
            string segments = "HKCDL:" + client.SEGNUM + ":1+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.03+@@";

            var sepaMessage = pain00100103.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, new DateTime(1999, 1, 1)).Replace("'", "");
            segments = segments.Replace("@@", "@" + sepaMessage.Length + "@") + sepaMessage;

            segments += "++" + OrderId + "+" + FirstTimeExecutionDay.ToString("yyyyMMdd") + ":" + (char) timeUnit + ":" + Rota + ":" + ExecutionDay;
            if (LastExecutionDay != null)
                segments += ":" + LastExecutionDay.Value.ToString("yyyyMMdd");

            segments += "'";

            if (Helper.IsTANRequired("HKCDL"))
            {
                client.SEGNUM = SEGNUM.SETInt(4);
                segments = HKTAN.Init_HKTAN(client, segments);
            }

            string message = FinTSMessage.Create(client, client.HNHBS, client.HNHBK, segments, client.HIRMS);
            var TAN = await FinTSMessage.Send(client, message);

            client.HITAN = Helper.Parse_String(Helper.Parse_String(TAN, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(client, TAN);

            return TAN;
        }
    }
}
