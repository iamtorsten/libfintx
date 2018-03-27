using System;

using UIKit;

namespace test_balance_iOS
{
    public partial class ViewController : UIViewController
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

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public ViewController()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

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
            Console.WriteLine(balance.Balance);
            Console.WriteLine();

            #endregion
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}