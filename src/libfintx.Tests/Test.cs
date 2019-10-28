/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2019 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 * 	
 * 	libfintx is free software; you can redistribute it and/or
 *	modify it under the terms of the GNU Lesser General Public
 * 	License as published by the Free Software Foundation; either
 * 	version 2.1 of the License, or (at your option) any later version.
 *	
 * 	libfintx is distributed in the hope that it will be useful,
 * 	but WITHOUT ANY WARRANTY; without even the implied warranty of
 * 	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * 	Lesser General Public License for more details.
 *	
 * 	You should have received a copy of the GNU Lesser General Public
 * 	License along with libfintx; if not, write to the Free Software
 * 	Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 * 	
 */

//#define WINDOWS

using libfintx.Data;
using System;
using System.Linq;
using System.IO;
using Xunit;
using Xunit.Abstractions;

using HBCI = libfintx.Main;

#if (DEBUG && WINDOWS)
using hbci = libfintx;

using System.Windows.Forms;
#endif

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace libfintx
{
    public class Test
    {
        bool Anonymous;

        private readonly ITestOutputHelper output;

        public Test(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Test_Balance()
        {
            var connectionDetails = new ConnectionDetails()
            {
                Account = "xxx",
                Blz = 76061482,
                BIC = "GENODEF1HSB",
                IBAN = "xxx",
                Url = "https://hbci11.fiducia.de/cgi-bin/hbciservlet",
                HBCIVersion = 300,
                UserId = "xxx",
                Pin = "xxx"
            };
            Anonymous = false;

            #region Sync

            /* Sync */

            libfintx.Main.Assembly("libfintx", "0.1");
            libfintx.Main.Tracing(true);

            #endregion

            #region balance

            /* Balance */

            var balance = HBCI.Balance(connectionDetails, new TANDialog(WaitForTAN), false);

            Console.WriteLine("[ Balance ]");
            Console.WriteLine();
            Console.WriteLine(balance.Data.Balance);
            Console.WriteLine();

            #endregion

            Console.ReadLine();
        }

        [Fact]
        public void Test_Accounts()
        {
            var connectionDetails = new ConnectionDetails()
            {
                Blz = 76050101,
                Url = "https://banking-by1.s-fints-pt-by.de/fints30",
                HBCIVersion = 300,
                UserId = "xxx",
                Pin = "xxx"
            };
            Anonymous = false;

            /* Sync */

            HBCI.Assembly("libfintx", "0.1");

            HBCI.Tracing(true);

            var accounts = HBCI.Accounts(connectionDetails, new TANDialog(WaitForTAN), Anonymous);
            foreach (var acc in accounts.Data)
            {
                output.WriteLine(acc.ToString());
            }
        }

        [Fact]
        public void Test_Request_TANMediumName()
        {
            var connectionDetails = new ConnectionDetails()
            {
                Blz = 76050101,
                Url = "https://banking-by1.s-fints-pt-by.de/fints30",
                HBCIVersion = 300,
                UserId = "xxx",
                Pin = "xxx"
            };

            #region Sync

            /* Sync */
            libfintx.Main.Assembly("libfintx", "0.1");
            libfintx.Main.Tracing(true);

            #endregion

            #region tanmediumname

            /* TANMediumname */

            var tanmediumname = libfintx.Main.RequestTANMediumName(connectionDetails).Data?.FirstOrDefault();

            Console.WriteLine("[ TAN Medium Name ]");
            Console.WriteLine();
            Console.WriteLine(tanmediumname);
            Console.WriteLine();

            #endregion

            Console.ReadLine();
        }

        [Fact]
        public void Test_PhotoTAN()
        {
            var PhotoCode = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\assets\\matrixcode.txt");

            var mCode = new MatrixCode(PhotoCode);

            mCode.CodeImage.SaveAsPng(File.OpenWrite(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "matrixcode.png")));
        }

        [Fact]
        public void Test_PushTAN()
        {
            string receiver = string.Empty;
            string receiverIBAN = string.Empty;
            string receiverBIC = string.Empty;
            decimal amount = 0;
            string usage = string.Empty;

            bool anonymous = false;

            ConnectionDetails connectionDetails = new ConnectionDetails()
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

            if (HBCI.Synchronization(connectionDetails).IsSuccess)
            {
                Segment.HIRMS = "921"; // -> pushTAN

                var tanmediumname = HBCI.RequestTANMediumName(connectionDetails);
                Segment.HITAB = tanmediumname.Data.FirstOrDefault();

                System.Threading.Thread.Sleep(5000);

                Console.WriteLine(HBCI.Transfer(connectionDetails, new TANDialog(WaitForTAN), receiver, receiverIBAN, receiverBIC, amount, usage, Segment.HIRMS, anonymous));

                output.WriteLine(Segment.HITANS);
            }

            var timer = new System.Threading.Timer(
                e => HBCI.Transaction_Output(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(10));
        }

#if (WINDOWS)
        static bool anonymous = false;

        static string receiver = string.Empty;
        static string receiverIBAN = string.Empty;
        static string receiverBIC = string.Empty;
        static decimal amount = 0;
        static string usage = string.Empty;
        public static ConnectionDetails connectionDetails;

        public static PictureBox pictureBox { get; set; }

        [Fact]
        public void Test_Flicker()
        {
            connectionDetails = new ConnectionDetails()
            {
                AccountHolder = "Torsten Klinger",
                Blz = 76061482,
                BIC = "GENODEF1HSB",
                IBAN = "xxx",
                Url = "https://hbci11.fiducia.de/cgi-bin/hbciservlet",
                HBCIVersion = 300,
                UserId = "xxx",
                Pin = "xxx"
            };

            receiver = "Klinger";
            receiverIBAN = "xxx";
            receiverBIC = "SSKNDE77XXX";
            amount = 1.0m;
            usage = "TEST";

            HBCI.Assembly("libfintx", "1");

            HBCI.Tracing(true);

            if (HBCI.Synchronization(connectionDetails, anonymous))
            {
                Segment.HIRMS = "972"; // -> chip-TAN               

                Image flickerImage = null;
                output.WriteLine(EncodingHelper.ConvertToUTF8(HBCI.Transfer(connectionDetails, receiver, receiverIBAN, receiverBIC, amount, usage, Segment.HIRMS, anonymous, out flickerImage, 220, 160)));

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
        }

        void Output()
        {
            output.WriteLine(HBCI.Transaction_Output());
        }
#endif
        public static string WaitForTAN(TANDialog tanDialog)
        {
            foreach (var msg in tanDialog.DialogResult.Messages)
                Console.WriteLine(msg);

            return Console.ReadLine();
        }
    }
}