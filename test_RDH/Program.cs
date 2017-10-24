using System;

using HBCI = libfintx.Main;
using hbci = libfintx;

namespace test_RDH
{
    class Program
    {
        static string FilePath = @"D:\rdh.rdh";
        static string Pwd = "rdh";

        static int BLZ = 0;
        static string Url = string.Empty;
        static int Port = 0;
        static int HBCIVersion = 0;
        static string UserID = string.Empty;

        static void Main(string[] args)
        {
            
            BLZ = 76061482;
            Url = "hbci01.fiducia.de";
            Port = 3000;
            HBCIVersion = 300;
            UserID = "6726706155000628100";

            HBCI.Assembly("libfintx", "1.0");

            HBCI.Tracing(true);

            HBCI.Debugging(true);

            hbci.RDHKEY.Create(FilePath, Pwd);

            if (HBCI.Synchronization_RDH(BLZ, Url, Port, HBCIVersion, UserID, FilePath, Pwd))
            {
                Console.WriteLine("Synchronisation ok");
            }
            else
                Console.WriteLine(HBCI.Transaction_Output());

            Console.ReadLine();
        }
    }
}
