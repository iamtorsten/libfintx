using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using libfintx.Logger.Log;

namespace libfintx.FinTS
{
    public static partial class Helper
    {

        /// <summary>
        /// Fill given <code>TANDialog</code> and wait for user to enter a TAN.
        /// </summary>
        /// <param name="BankCode"></param>
        /// <param name="pictureBox"></param>
        /// <param name="flickerImage"></param>
        /// <param name="flickerWidth"></param>
        /// <param name="flickerHeight"></param>
        /// <param name="renderFlickerCodeAsGif"></param>
        public static async Task<string> WaitForTanAsync(FinTsClient client, HBCIDialogResult dialogResult, TanRequest tanRequest)
        {
            var BankCode_ = "HIRMS" + Parse_String(dialogResult.RawData, "'HIRMS", "'");
            string[] values = BankCode_.Split('+');
            foreach (var item in values)
            {
                if (!item.StartsWith("HIRMS"))
                    TransactionConsole.Output = item.Replace("::", ": ");
            }

            var HITAN = "HITAN" + Parse_String(dialogResult.RawData.Replace("?'", "").Replace("?:", ":").Replace("<br>", Environment.NewLine).Replace("?+", "??"), "'HITAN", "'");

            string HITANFlicker = string.Empty;

            var tanMethods = BPD.HITANS.Where(h => h.Version == client.HITANS).SelectMany(t => t.TanProcesses);

            var tanMethod = string.Empty;

            foreach (var item in tanMethods)
            {
                if (item.TanCode == Convert.ToInt32(client.HIRMS))
                    tanMethod = item.Name;
            }

            Log.Write($"Processing TAN process '{tanMethod}' ...");

            // Smart-TAN plus optisch
            // chipTAN optisch
            if (tanMethod.Equals("Smart-TAN plus optisch") || tanMethod.Contains("chipTAN optisch"))
            {
                HITANFlicker = HITAN;
            }

            String[] values_ = HITAN.Split('+');

            int i = 1;

            foreach (var item in values_)
            {
                i = i + 1;

                if (i == 6)
                {
                    TransactionConsole.Output = TransactionConsole.Output + "??" + item.Replace("::", ": ").TrimStart();

                    TransactionConsole.Output = TransactionConsole.Output.Replace("??", " ")
                            .Replace("0030: ", "")
                            .Replace("1.", Environment.NewLine + "1.")
                            .Replace("2.", Environment.NewLine + "2.")
                            .Replace("3.", Environment.NewLine + "3.")
                            .Replace("4.", Environment.NewLine + "4.")
                            .Replace("5.", Environment.NewLine + "5.")
                            .Replace("6.", Environment.NewLine + "6.")
                            .Replace("7.", Environment.NewLine + "7.")
                            .Replace("8.", Environment.NewLine + "8.");
                }
            }

            // chipTAN optisch
            if (tanMethod.Contains("chipTAN optisch"))
            {
                string FlickerCode = string.Empty;

                FlickerCode = "CHLGUC" + Helper.Parse_String(HITAN, "CHLGUC", "CHLGTEXT") + "CHLGTEXT";

                FlickerCode flickerCode = new FlickerCode(FlickerCode);
                var flickerCodeRenderer = new FlickerCodeRenderer(flickerCode.Render());
                return await tanRequest.WithFlickerAsync(flickerCodeRenderer);
            }

            // Smart-TAN plus optisch
            if (tanMethod.Equals("Smart-TAN plus optisch"))
            {
                HITANFlicker = HITAN.Replace("?@", "??");

                string FlickerCode = string.Empty;

                String[] values__ = HITANFlicker.Split('@');

                int ii = 1;

                foreach (var item in values__)
                {
                    ii = ii + 1;

                    if (ii == 4)
                    {
                        FlickerCode = item;
                    }
                }

                FlickerCode flickerCode = new FlickerCode(FlickerCode.Trim());
                var flickerCodeRenderer = new FlickerCodeRenderer(flickerCode.Render());
                return await tanRequest.WithFlickerAsync(flickerCodeRenderer);
            }

            // Smart-TAN photo
            if (tanMethod.Equals("Smart-TAN photo"))
            {
                var PhotoCode = Parse_String(dialogResult.RawData, ".+@", "'HNSHA");

                var mCode = new MatrixCode(PhotoCode.Substring(5, PhotoCode.Length - 5));

                return await tanRequest.WithMatrixAsync(mCode);
            }

            // PhotoTAN
            if (tanMethod.Equals("photoTAN-Verfahren"))
            {
                // HITAN:5:5:4+4++nmf3VmGQDT4qZ20190130091914641+Bitte geben Sie die photoTan ein+@3031@       image/pngÃŠÂ‰PNG
                var match = Regex.Match(dialogResult.RawData, @"HITAN.+@\d+@(.+)'HNHBS", RegexOptions.Singleline);
                if (match.Success)
                {
                    var pichtureBinaryDataString = match.Groups[1].Value;

                    var mCode = new MatrixCode(pichtureBinaryDataString);

                    return await tanRequest.WithMatrixAsync(mCode);
                }
            }

            return await tanRequest.WithUnknownAsync();
        }
    }
}
