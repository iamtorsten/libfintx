using libfintx.Data;
using System;

namespace test_balance
{
    class Program
    {
        static bool Anonymous;

        static void Main(string[] args)
        {
            var connectionDetails = new ConnectionDetails()
            {
                Account = "xxx",
                Blz = 76061482,
                BIC = "GENODEF1HSB",
                IBAN = "xxx",
                Url = "https://hbci11.fiducia.de/cgi-bin/hbciservlet",
                HBCIVersion = 300,
                UserId = "xxx",
                Pin = "xxx"
            };            
            Anonymous = false;

            #region Sync

            /* Sync */

            libfintx.Main.Assembly("libfintx", "0.1");

            libfintx.Main.Tracing(true);

            if (libfintx.Main.Synchronization(connectionDetails, Anonymous))
            {
                Console.WriteLine("[ Sync ]");
                Console.WriteLine();
                Console.WriteLine("Sync ok");
                Console.WriteLine();
            }

            else
            {
                Console.WriteLine("[ Sync ]");
                Console.WriteLine();
                Console.WriteLine(libfintx.Main.Transaction_Output());
                Console.WriteLine();
            }

            #endregion

            #region balance

            /* Balance */

            var balance = libfintx.Main.Balance(connectionDetails, false);

            Console.WriteLine("[ Balance ]");
            Console.WriteLine();
            Console.WriteLine(balance.Balance);
            Console.WriteLine();

            #endregion

            Console.ReadLine();
        }
    }
}
