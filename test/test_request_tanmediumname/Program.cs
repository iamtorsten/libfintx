using libfintx.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_request_tanmediumname
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionDetails = new ConnectionDetails()
            {
                Blz = 76050101,
                Url = "https://banking-by1.s-fints-pt-by.de/fints30",
                HBCIVersion = 300,
                UserId = "xxx",
                Pin = "xxx"
            };

            bool Anonymous = false;

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

            #region tanmediumname

            /* TANMediumname */

            var tanmediumname = libfintx.Main.RequestTANMediumName(connectionDetails);

            Console.WriteLine("[ TAN Medium Name ]");
            Console.WriteLine();
            Console.WriteLine(tanmediumname);
            Console.WriteLine();

            #endregion

            Console.ReadLine();
        }
    }
}
