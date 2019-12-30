using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace libfintx_test
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += new ThreadExceptionEventHandler(HandleUncatchedException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
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
