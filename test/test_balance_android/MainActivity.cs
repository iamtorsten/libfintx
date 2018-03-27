using Android.App;
using Android.Widget;
using Android.OS;
using System;

namespace test_balance_android
{
    [Activity(Label = "test_balance_android", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
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

        int count = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };

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
    }
}

