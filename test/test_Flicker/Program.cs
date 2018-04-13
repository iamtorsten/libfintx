using System;
using System.Threading.Tasks;
using System.Windows.Forms;

using HBCI = libfintx.Main;

using libfintx;
using libfintx.Data;
using System.Drawing;

namespace test_Flicker
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

            //Transfer with chipTan based on WindowsForms rendering
            /*
            if (HBCI.Synchronization(connectionDetails, anonymous))
            {
                Task oFlicker = new Task(() => openFlickerWindow());
                oFlicker.Start();

                Task oTAN = new Task(() => openTANWindow());
                oTAN.Start();

                Segment.HIRMS = "911"; // -> chip-TAN

                System.Threading.Thread.Sleep(5000);

                Console.WriteLine(EncodingHelper.ConvertToUTF8(HBCI.Transfer(connectionDetails, receiver, receiverIBAN, receiverBIC, amount, usage, Segment.HIRMS, pictureBox, anonymous)));
            }

            var timer = new System.Threading.Timer(
                e => Output(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(10));

            Console.ReadLine();
            */

            //Transfer with chipTan based on GIF-rendering            
            if (HBCI.Synchronization(connectionDetails, anonymous))
            {   
                Segment.HIRMS = "911"; // -> chip-TAN               

                Image flickerImage = null;
                Console.WriteLine(EncodingHelper.ConvertToUTF8(HBCI.Transfer(connectionDetails, receiver, receiverIBAN, receiverBIC, amount, usage, Segment.HIRMS, anonymous, out flickerImage, 220, 160)));

                Form frm = new Form();
                frm.Size = new Size(flickerImage.Width + 5, flickerImage.Height + 5);
                PictureBox pb = new PictureBox();
                pb.Dock = DockStyle.Fill;
                frm.Controls.Add(pb);
                pb.Image = flickerImage;                
                Application.Run(frm);
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