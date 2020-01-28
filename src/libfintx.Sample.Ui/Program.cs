using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace libfintx.Sample.Ui
{
    internal static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.ThreadException += new ThreadExceptionEventHandler(HandleUncatchedException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void HandleUncatchedException(object sender, ThreadExceptionEventArgs t)
        {
            var errorMessages = new List<string>();
            var exception = t.Exception;
            while (exception != null)
            {
                errorMessages.Add(exception.Message);
                exception = exception.InnerException;
            }

            Console.WriteLine($"{t.Exception}: {t.Exception.Message}{Environment.NewLine}{t.Exception.StackTrace}");

            MessageBox.Show($"Unbehandelter Fehler: {string.Join(" -> ", errorMessages)}{Environment.NewLine}{t.Exception.StackTrace}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
