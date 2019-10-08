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
            Console.WriteLine("BLZ:");
            details.Blz = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("KontoID:");
            details.Account = Console.ReadLine();
            Console.WriteLine("IBAN:");
            details.IBAN = Console.ReadLine();
            Console.WriteLine("Institute FinTS Url:");
            details.Url = Console.ReadLine();
            Console.WriteLine("Account:");
            details.UserId = Console.ReadLine();
            Console.WriteLine("PIN:");
            details.Pin = Console.ReadLine();
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
