using System;
using System.Collections.Generic;
using libfintx;
using libfintx.Data;

namespace libfintx.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter your banking credentials!");
            ConnectionDetails details = new ConnectionDetails();
            details.Account = Console.ReadLine();
            details.HBCIVersion = 300;
            HBCIOutput(libfintx.Main.Synchronization(details).Messages);
        }

        /// <summary>
        /// HBCI-Nachricht ausgeben
        /// </summary>
        /// <param name="hbcimsg"></param>
        private static void HBCIOutput(IEnumerable<HBCIBankMessage> hbcimsg)
        {
            foreach (var msg in hbcimsg)
            {
                Console.WriteLine("Code: " + msg.Code + " | " + "Typ: " + msg.Type + " | " + "Nachricht: " + msg.Message);
            }
        }
    }
}
