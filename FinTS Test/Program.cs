using System;

namespace FinTS_Test
{
    class Program
    {
        static int BLZ = 7606172;
        static string URL = "https://hbci11.fiducia.de/cgi-bin/hbciservlet";
        static int HBCIVersion = 300;
        static string UserID = "xxxxxxxxx";
        static string PIN = "xxxxxx";
        static bool Anonymous = false;

        static void Main(string[] args)
        {
            libfintx.Main.Assembly("libfintx", "0.1");

            libfintx.Main.Tracing(true);

            if (libfintx.Main.Synchronization(BLZ, URL, HBCIVersion, UserID, PIN, Anonymous))
                Console.WriteLine("Ok");
            else
                Console.WriteLine(libfintx.Main.Transaction_Output());

            Console.ReadLine();
        }
    }
}
