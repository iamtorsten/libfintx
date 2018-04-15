using System;

using HBCI = libfintx.Main;

using libfintx.Data;
using System.Windows.Forms;
using libfintx;
using System.Threading.Tasks;

namespace test_pushTAN
{
    class Program
    {
        static bool anonymous = false;

        static string receiver = string.Empty;
        static string receiverIBAN = string.Empty;
        static string receiverBIC = string.Empty;
        static decimal amount = 0;
        static string usage = string.Empty;
        public static ConnectionDetails connectionDetails;

        public static PictureBox pictureBox { get; set; }

        [STAThread]
        static void Main(string[] args)
        {
            connectionDetails = new ConnectionDetails()
            {
                AccountHolder = "Torsten Klinger",
                Blz = 76050101,
                BIC = "SSKNDE77XXX",
                IBAN = "xxx",
                Url = "https://banking-by1.s-fints-pt-by.de/fints30",
                HBCIVersion = 300,
                UserId = "xxx",
                Pin = "xxx"
            };

            receiver = "Klinger";
            receiverIBAN = "xxx";
            receiverBIC = "GENODEF1HSB";
            amount = 1.0m;
            usage = "TEST";

            HBCI.Assembly("libfintx", "1");

            HBCI.Tracing(true);

            if (HBCI.Synchronization(connectionDetails, anonymous))
            {
                Task oTAN = new Task(() => openTANWindow());
                oTAN.Start();

                Segment.HIRMS = "921"; // -> pushTAN

                var tanmediumname = libfintx.Main.RequestTANMediumName(connectionDetails);
                Segment.HITAB = tanmediumname;

                System.Threading.Thread.Sleep(5000);

                Console.WriteLine(EncodingHelper.ConvertToUTF8(HBCI.Transfer(connectionDetails, receiver, receiverIBAN, receiverBIC, amount, usage, Segment.HIRMS, pictureBox, anonymous)));

                Console.WriteLine(Segment.HITANS);
            }

            var timer = new System.Threading.Timer(
                e => Output(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(10));

            Console.ReadLine();
        }

        static void Output()
        {
            Console.WriteLine(HBCI.Transaction_Output());
        }

        static bool openTANWindow()
        {
            Application.Run(new TAN());
            return true;
        }
    }
}