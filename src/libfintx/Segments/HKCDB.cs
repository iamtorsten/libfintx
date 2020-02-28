using System;
using System.Threading.Tasks;

namespace libfintx
{
    public static class HKCDB
    {
        /// <summary>
        /// Get bankers orders
        /// </summary>
        public static async Task<String> Init_HKCDB(FinTsClient client)
        {
            Log.Write("Starting job HKCDB: Get bankers order");

            client.SEGNUM = SEGNUM.SETInt(3);

            var connectionDetails = client.ConnectionDetails;
            string segments = "HKCDB:" + client.SEGNUM + ":1+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.03'";

            if (Helper.IsTANRequired("HKCDB"))
            {
                client.SEGNUM = SEGNUM.SETInt(4);
                segments = HKTAN.Init_HKTAN(client, segments);
            }

            string message = FinTSMessage.Create(connectionDetails.HbciVersion, client.HNHBS, client.HNHBK, connectionDetails.BlzPrimary, connectionDetails.UserId, connectionDetails.Pin, client.SystemId, segments, client.HIRMS, client.SEGNUM);
            string response = await FinTSMessage.Send(connectionDetails.Url, message);

            client.HITAN = Helper.Parse_String(Helper.Parse_String(response, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
