using System;

using HBCI = libfintx.FinTsConfig;
using hbci = libfintx;
using client = libfintx.FinTsClient;
using libfintx;

namespace libfintx.Test_RDH
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
            UserID = "";

            HBCI.Tracing(true);

            HBCI.Debugging(true);

            // create rdh-10 key file
            hbci.RdhKey.Create(FilePath, Pwd, BLZ, UserID, Country, ProfileVersion);

            // hbci key
            hbci.RdhKey.RDHKEYFILE = FilePath;
            hbci.RdhKey.RDHKEYFILEPWD = Pwd;

            FinTsClient client = new FinTsClient(null);

            client.Synchronization_RDH(BLZ, Url, Port, HBCIVersion, UserID, FilePath, Pwd);

            Console.ReadLine();
        }
    }
}
