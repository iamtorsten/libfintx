using System;
using System.Threading.Tasks;
using System.Windows.Forms;

using HBCI = libfintx.Main;

using libfintx;

namespace test_Flicker
{
    class Program
    {
        static bool Anonymous = false;

        static string IBAN = string.Empty;
        static string BIC = string.Empty;

        static string AccountHolder = string.Empty;
        static string Receiver = string.Empty;
        static string ReceiverIBAN = string.Empty;
        static string ReceiverBIC = string.Empty;
        static string Amount = string.Empty;
        static string Usage = string.Empty;

        public static string URL { get; set; }
        public static int HBCIVersion { get; set; }
        public static int BLZ { get; set; }
        public static string UserID { get; set; }
        public static string PIN { get; set; }

        public static PictureBox pictureBox { get; set; }

        [STAThread]
        static void Main(string[] args)
        {
            URL = "https://hbci11.fiducia.de/cgi-bin/hbciservlet";
            HBCIVersion = 300;
            BLZ = 76061482;
            UserID = "xxxxxx";
            PIN = "xxxxxx";

            IBAN = "xxxxxx";
            BIC = "xxxxxx";
            AccountHolder = "xxxxxx";
            Receiver = "xxxxxx";
            ReceiverIBAN = "xxxxxx";
            ReceiverBIC = "xxx";
            Amount = "1,0";
            Usage = "TEST";

            HBCI.Assembly("libfintx", "1");

            HBCI.Tracing(true);

            if (HBCI.Synchronization(BLZ, URL, HBCIVersion, UserID, PIN, Anonymous))
            {
                Task oFlicker = new Task(() => openFlickerWindow());
                oFlicker.Start();

                Task oTAN = new Task(() => openTANWindow());
                oTAN.Start();

                Segment.HIRMS = "972";

                System.Threading.Thread.Sleep(5000);

                Console.WriteLine(EncodingHelper.ConvertToUTF8(HBCI.Transfer(BLZ, AccountHolder, IBAN, BIC, Receiver, ReceiverIBAN, ReceiverBIC,
                    Amount, Usage, URL, HBCIVersion, UserID, PIN, "972", pictureBox, Anonymous)));
            }

            var timer = new System.Threading.Timer(
                e => Output(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(10));

            Console.ReadLine();
        }

        static bool openFlickerWindow()
        {
            Application.Run(new Flicker());
            return true;
        }

        static bool openTANWindow()
        {
            Application.Run(new TAN());
            return true;
        }

        static void Output()
        {
            Console.WriteLine(HBCI.Transaction_Output());
        }
    }
}
