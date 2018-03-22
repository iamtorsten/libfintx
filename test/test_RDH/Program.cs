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

        static int Country = 280;
        static int ProfileVersion = 0;

        static void Main(string[] args)
        {
            
            BLZ = 76061482;
            Url = "hbci01.fiducia.de";
            Port = 3000;
            HBCIVersion = 300;
            UserID = "6726706167001413598";

            HBCI.Assembly("libfintx", "1.0");

            HBCI.Tracing(true);

            HBCI.Debugging(true);

            // create rdh-10 key file
            hbci.RDHKEY.Create(FilePath, Pwd, BLZ, UserID, Country, ProfileVersion);

            // hbci key
            hbci.RDHKEY.RDHKEYFILE = FilePath;
            hbci.RDHKEY.RDHKEYFILEPWD = Pwd;

            if (!HBCI.Synchronization_RDH(BLZ, Url, Port, HBCIVersion, UserID, FilePath, Pwd))
            {
                Console.WriteLine(HBCI.Transaction_Output());
            }   

            Console.ReadLine();
        }
    }
}
