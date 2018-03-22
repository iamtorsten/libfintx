using System;

namespace test_balance
{
    class Program
    {
        static string Account;
        static int BLZ;
        static string BIC;
        static string IBAN;
        static string URL;
        static int HBCIVersion;
        static string UserID;
        static string PIN;
        static bool Anonymous;

        static void Main(string[] args)
        {
            Account = "xxx";
            BLZ = 76061482;
            BIC = "GENODEF1HSB";
            IBAN = "xxx";
            URL = "https://hbci11.fiducia.de/cgi-bin/hbciservlet";
            HBCIVersion = 300;
            UserID = "xxx";
            PIN = "xxx";
            Anonymous = false;

            #region Sync

            /* Sync */

            libfintx.Main.Assembly("libfintx", "0.1");

            libfintx.Main.Tracing(true);

            if (libfintx.Main.Synchronization(BLZ, URL, HBCIVersion, UserID, PIN, Anonymous))
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

            var balance = libfintx.Main.Balance(Account, BLZ, IBAN, BIC, URL, HBCIVersion, UserID, PIN, false);

            Console.WriteLine("[ Balance ]");
            Console.WriteLine();
            Console.WriteLine(balance);
            Console.WriteLine();

            #endregion

            Console.ReadLine();
        }
    }
}
