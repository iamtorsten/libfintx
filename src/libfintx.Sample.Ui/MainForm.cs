using libfintx.FinTS;
using libfintx.FinTS.Camt;
using libfintx.FinTS.Data;
using libfintx.FinTSConfig;
using libfintx.Globals;
using libfintx.Sepa;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace libfintx.Sample.Ui
{
    public partial class MainForm : Form
    {
        private List<Bank> _bankList;

        private bool _closing;

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Synchronisation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_synchronisation_Click(object sender, EventArgs e)
        {
            var connectionDetails = GetConnectionDetails();
            var client = new FinTsClient(connectionDetails);
            var result = await client.Synchronization();

            HBCIOutput(result.Messages);
        }

        /// <summary>
        /// HBCI-Nachricht ausgeben
        /// </summary>
        /// <param name="hbcimsg"></param>
        public void HBCIOutput(IEnumerable<HBCIBankMessage> hbcimsg)
        {
            foreach (var msg in hbcimsg)
            {
                txt_hbci_meldung.Invoke(new MethodInvoker
                (delegate ()
                {
                    txt_hbci_meldung.Text += "Code: " + msg.Code + " | " + "Typ: " + msg.Type + " | " + "Nachricht: " + msg.Message + Environment.NewLine;
                    txt_hbci_meldung.SelectionStart = txt_hbci_meldung.TextLength;
                    txt_hbci_meldung.ScrollToCaret();
                }));
            }
        }

        /// <summary>
        /// Einfache Nachricht ausgeben
        /// </summary>
        /// <param name="msg"></param>
        public void SimpleOutput(string msg)
        {
            txt_hbci_meldung.Invoke(new MethodInvoker
                (delegate ()
                {
                    txt_hbci_meldung.Text += msg + Environment.NewLine;
                    txt_hbci_meldung.SelectionStart = txt_hbci_meldung.TextLength;
                    txt_hbci_meldung.ScrollToCaret();
                }));
        }

        /// <summary>
        /// Bankdaten laden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lade_bankdaten_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "CSV|*.csv";
            openFileDialog1.Title = "Datei mit Bankdaten laden";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Lade Bankdaten falls vorhanden

                // Damit keine Zugangsdaten direkt im Code hinterlegt sind, kann optional eine Datei verwendet werden.
                // Datei liegt in C:/Users/<username>/libfintx_test_connection.csv

                var userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var connFile = Path.Combine(userDir, openFileDialog1.FileName);

                if (File.Exists(connFile))
                {
                    var lines = File.ReadAllLines(connFile).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
                    if (lines.Length != 2)
                    {
                        SimpleOutput($"Die Datei {connFile} existiert, hat aber das falsche Format.");
                        return;
                    }

                    var values = lines[1].Split(';');
                    if (values.Length < 8)
                    {
                        SimpleOutput($"Die Datei {connFile} existiert, hat aber das falsche Format.");
                        return;
                    }

                    txt_kontonummer.Text = values[0];
                    txt_bankleitzahl.Text = values[1];
                    txt_bic.Text = values[2];
                    txt_iban.Text = values[3];
                    txt_url.Text = values[4];
                    txt_hbci_version.Text = values[5];
                    txt_userid.Text = values[6];
                    txt_pin.Text = values[7];
                }
            }
        }

        /// <summary>
        /// Überweisungsdaten laden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lade_überweisungsdaten_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "CSV|*.csv";
            openFileDialog1.Title = "Datei mit Überweisungsdaten laden";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Lade Überweisungsdaten falls vorhanden

                var userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var connFile = Path.Combine(userDir, openFileDialog1.FileName);

                if (File.Exists(connFile))
                {
                    var lines = File.ReadAllLines(connFile).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
                    if (lines.Length != 2)
                    {
                        SimpleOutput($"Die Datei {connFile} existiert, hat aber das falsche Format.");
                        return;
                    }

                    var values = lines[1].Split(';');
                    if (values.Length < 5)
                    {
                        SimpleOutput($"Die Datei {connFile} existiert, hat aber das falsche Format.");
                        return;
                    }

                    txt_empfängername.Text = values[0];
                    txt_empfängeriban.Text = values[1].Replace(" ", "");
                    txt_empfängerbic.Text = values[2];
                    txt_betrag.Text = values[3];
                    txt_verwendungszweck.Text = values[4];
                    if (values.Length >= 6)
                        txt_tanverfahren.Text = values[5];
                }
            }
        }

        /// <summary>
        /// Kontostand abfragen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_kontostand_abfragen_Click(object sender, EventArgs e)
        {
            var connectionDetails = GetConnectionDetails();
            var client = new FinTsClient(connectionDetails);
            var sync = await client.Synchronization();

            HBCIOutput(sync.Messages);

            if (sync.IsSuccess)
            {
                // TAN-Verfahren
                client.HIRMS = txt_tanverfahren.Text;

                if (!await InitTANMedium(client))
                    return;

                var balance = await client.Balance(CreateTANDialog(client));

                HBCIOutput(balance.Messages);

                if (balance.IsSuccess)
                    SimpleOutput("Kontostand: " + Convert.ToString(balance.Data.Balance));
            }
        }

        /// <summary>
        /// Konten anzeigen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_konten_anzeigen_Click(object sender, EventArgs e)
        {
            var connectionDetails = GetConnectionDetails();
            var client = new FinTsClient(connectionDetails);
            var sync = await client.Synchronization();

            HBCIOutput(sync.Messages);

            if (sync.IsSuccess)
            {
                // TAN-Verfahren
                client.HIRMS = txt_tanverfahren.Text;

                if (!await InitTANMedium(client))
                    return;

                var accounts = await client.Accounts(CreateTANDialog(client));

                HBCIOutput(accounts.Messages);

                if (accounts.IsSuccess && !accounts.HasError)
                {
                    foreach (var acc in accounts.Data)
                    {
                        SimpleOutput("Inhaber: " + acc.AccountOwner + " | " + "Unterkontomerkmal: " + acc.SubAccountFeature + " | " + "IBAN: " + acc.AccountIban + " | " + "Typ: " + acc.AccountType);

                        foreach (var p in acc.AccountPermissions)
                        {
                            SimpleOutput("Segment: " + p.Segment + " | " + "Beschreibung: " + p.Description);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Zugelassene TAN-Verfahren
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_zugelassene_tanverfahren_Click(object sender, EventArgs e)
        {
            var connectionDetails = GetConnectionDetails();
            var client = new FinTsClient(connectionDetails);
            var sync = await client.Synchronization();

            HBCIOutput(sync.Messages);

            if (sync.IsSuccess)
            {
                foreach (var process in TanProcesses.Items)
                {
                    SimpleOutput("Name: " + process.ProcessName + " | " + "Nummer: " + process.ProcessNumber);
                }
            }
        }

        /// <summary>
        /// Umsätze abholen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_umsätze_abholen_Click(object sender, EventArgs e)
        {
            var connectionDetails = GetConnectionDetails();
            var client = new FinTsClient(connectionDetails);
            var sync = await client.Synchronization();

            HBCIOutput(sync.Messages);

            if (sync.IsSuccess)
            {
                // TAN-Verfahren
                client.HIRMS = txt_tanverfahren.Text;

                if (!await InitTANMedium(client))
                    return;

                DateTime? startDate = chk_umsatzabruf_von.Checked ? date_umsatzabruf_von.Value : (DateTime?)null;
                DateTime? endDate = chk_umsatzabruf_bis.Checked ? date_umsatzabruf_bis.Value : (DateTime?)null;

                int? maxDays = BPD.HIKAZS.OrderByDescending(s => s.Version).FirstOrDefault()?.Zeitraum;
                if (startDate != null && maxDays != null && DateTime.Now.AddDays(maxDays.Value * -1).Date > startDate.Value.Date)
                    MessageBox.Show($"Es können nur Umsätze abgeholt werden, die maximal {maxDays} Tage zurückliegen.");

                var transactions = await client.Transactions(CreateTANDialog(client), startDate, endDate);

                HBCIOutput(transactions.Messages);

                if (transactions.IsSuccess)
                {
                    foreach (var item in transactions.Data)
                    {
                        foreach (var i in item.SwiftTransactions)
                        {
                            SimpleOutput(
                                "Datum: " + i.InputDate + " | " +
                                "Buchungsschlüssel: " + i.TransactionTypeId + " | " +
                                "GV-Code: " + i.TypeCode + " | " +
                                "Empfänger / Auftraggeber: " + i.PartnerName + " | " +
                                "Verwendungszweck: " + i.Description + " | "
                                + "Betrag: " + i.Amount);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Umsätze im Format camt052 abholen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void camt_052_abholen_Click(object sender, EventArgs e)
        {
            var connectionDetails = GetConnectionDetails();
            var client = new FinTsClient(connectionDetails);
            var sync = await client.Synchronization();

            HBCIOutput(sync.Messages);

            if (sync.IsSuccess)
            {
                // TAN-Verfahren
                client.HIRMS = txt_tanverfahren.Text;

                if (!await InitTANMedium(client))
                    return;

                DateTime? startDate = chk_umsatzabruf_von.Checked ? date_umsatzabruf_von.Value : (DateTime?)null;

                var transactions = await client.Transactions_camt(CreateTANDialog(client), CamtVersion.Camt052, startDate);

                HBCIOutput(transactions.Messages);

                if (transactions.IsSuccess)
                {
                    foreach (var item in transactions.Data)
                    {
                        foreach (var i in item.Transactions)
                        {
                            SimpleOutput(
                                "Datum: " + i.InputDate + " | " +
                                "Buchungsschlüssel: " + i.TransactionTypeId + " | " +
                                "GV-Code: " + i.TypeCode + " | " +
                                "Empfänger / Auftraggeber: " + i.PartnerName + " | " +
                                "Verwendungszweck: " + i.Description + " | "
                                + "Betrag: " + String.Format("{0:0.00}", i.Amount));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Umsätze im Format camt053 abholen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void camt_053_abholen_Click(object sender, EventArgs e)
        {
            var connectionDetails = GetConnectionDetails();
            var client = new FinTsClient(connectionDetails);
            var sync = await client.Synchronization();

            HBCIOutput(sync.Messages);

            if (sync.IsSuccess)
            {
                // TAN-Verfahren
                client.HIRMS = txt_tanverfahren.Text;

                if (!await InitTANMedium(client))
                    return;

                DateTime? startDate = chk_umsatzabruf_von.Checked ? date_umsatzabruf_von.Value : (DateTime?)null;

                var transactions = await client.Transactions_camt(CreateTANDialog(client), CamtVersion.Camt053, startDate);

                HBCIOutput(transactions.Messages);

                if (transactions.IsSuccess)
                {
                    foreach (var item in transactions.Data)
                    {
                        foreach (var i in item.Transactions)
                        {
                            SimpleOutput("Datum: " + i.InputDate + " | " +
                                "Empfänger / Auftraggeber: " + i.PartnerName + " | " +
                                "Verwendungszweck: " + i.Text + " | "
                                + "Betrag: " + i.Amount);
                        }
                    }
                }
            }
        }

        private async void btn_daueraufträge_abholen_Click(object sender, EventArgs e)
        {
            var connectionDetails = GetConnectionDetails();
            var client = new FinTsClient(connectionDetails);
            var sync = await client.Synchronization();

            HBCIOutput(sync.Messages);

            if (sync.IsSuccess)
            {
                // TAN-Verfahren
                client.HIRMS = txt_tanverfahren.Text;

                if (!await InitTANMedium(client))
                    return;

                var bankersOrders = await client.GetBankersOrders(CreateTANDialog(client));

                HBCIOutput(bankersOrders.Messages);

                if (bankersOrders.IsSuccess)
                {
                    if (bankersOrders.Data != null && bankersOrders.Data.Count > 0)
                    {
                        foreach (var item in bankersOrders.Data)
                        {
                            Pain00100103CtData.PaymentInfo paymentData = item.SepaData.Payments.FirstOrDefault();
                            var txInfo = paymentData.CreditTxInfos.FirstOrDefault();

                            SimpleOutput("Auftrags-Id: " + item.OrderId + " | " +
                                "Empfänger: " + txInfo.Creditor + " | " +
                                "Betrag: " + txInfo.Amount + " | " +
                                "Verwendungszweck: " + txInfo.RemittanceInformation + " | " +
                                "Erste Ausführung: " + $"{item.FirstExecutionDate:d}" + " | " +
                                "Nächste Ausführung: " + $"{paymentData.RequestedExecutionDate:d}" + " | " +
                                "Letzte Ausführung: " + $"{item.LastExecutionDate:d}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Überweisen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_überweisen_Click(object sender, EventArgs e)
        {
            Config.Logging(true);

            var connectionDetails = GetConnectionDetails();
            var client = new FinTsClient(connectionDetails);
            var sync = await client.Synchronization();

            HBCIOutput(sync.Messages);

            if (sync.IsSuccess)
            {
                // TAN-Verfahren
                client.HIRMS = txt_tanverfahren.Text;

                await InitTANMedium(client);

                var transfer = await client.Transfer(CreateTANDialog(client), txt_empfängername.Text, Regex.Replace(txt_empfängeriban.Text, @"\s+", ""), txt_empfängerbic.Text,
                    decimal.Parse(txt_betrag.Text), txt_verwendungszweck.Text, client.HIRMS);

                // Out image is needed e. g. for photoTAN
                //var transfer = Main.Transfer(connectionDetails, txt_empfängername.Text, txt_empfängeriban.Text, txt_empfängerbic.Text,
                //    decimal.Parse(txt_betrag.Text), txt_verwendungszweck.Text, Segment.HIRMS, pBox_tan, false);

                HBCIOutput(transfer.Messages);
            }
        }

        /// <summary>
        /// TAN-Medium-Name abfragen -> Notwendig bsp. für pushTAN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_tan_medium_name_abfragen_Click(object sender, EventArgs e)
        {
            var connectionDetails = GetConnectionDetails();
            var client = new FinTsClient(connectionDetails);
            var sync = await client.Synchronization();

            HBCIOutput(sync.Messages);

            if (sync.IsSuccess)
            {
                client.HIRMS = txt_tanverfahren.Text;
                var result = await client.RequestTANMediumName();

                HBCIOutput(result.Messages);

                if (result.IsSuccess)
                    SimpleOutput(string.Join(", ", result.Data));
            }
        }

        private bool _tanReady;

        /// <summary>
        /// Auftrag mit TAN bestätigen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_auftrag_bestätigen_tan_Click(object sender, EventArgs e)
        {
            _tanReady = true;
        }

        private void chk_Tracing_CheckedChanged(object sender, EventArgs e)
        {
            Config.Tracing(chk_tracing.Checked, false, chk_tracingMaskCredentials.Checked);
            if (chk_tracing.Checked)
            {
                MessageBox.Show("Achtung: Die Nachrichten werden im Klartext (inkl. PIN, Benutzerkennung, TAN) in eine Textdatei geschrieben!");
            }
            chk_tracingFormatted.Visible = chk_tracing.Checked;
        }

        private void chk_tracingFormatted_CheckedChanged(object sender, EventArgs e)
        {
            Config.Tracing(chk_tracing.Checked, chk_tracingFormatted.Checked);
        }

        private ConnectionDetails GetConnectionDetails()
        {
            var result = new ConnectionDetails()
            {
                AccountHolder = txt_empfängername.Text,
                Account = txt_kontonummer.Text,
                Blz = Convert.ToInt32(txt_bankleitzahl.Text),
                BlzHeadquarter = string.IsNullOrWhiteSpace(txt_bankleitzahl_zentrale.Text) ? (int?)null : Convert.ToInt32(txt_bankleitzahl_zentrale.Text),
                Bic = txt_bic.Text,
                Iban = Regex.Replace(txt_iban.Text, @"\s+", ""),
                Url = txt_url.Text,
                HbciVersion = Convert.ToInt32(txt_hbci_version.Text),
                UserId = txt_userid.Text,
                Pin = txt_pin.Text
            };

            return result;
        }

        private void Txt_bankleitzahl_TextChanged(object sender, EventArgs e)
        {
            var bank = _bankList.FirstOrDefault(b => b.Blz == txt_bankleitzahl.Text);
            if (bank != null)
            {
                txt_bankleitzahl_zentrale.Text = bank.BlzZentrale;
                txt_bic.Text = bank.Bic;
                txt_url.Text = bank.Url;
                txt_hbci_version.Text = "300";
            }

            UpdateIban();
        }

        private void UpdateIban()
        {
            if (!string.IsNullOrEmpty(txt_bankleitzahl.Text) && !string.IsNullOrWhiteSpace(txt_kontonummer.Text))
            {
                txt_iban.Text = CreateIban(txt_bankleitzahl.Text, txt_kontonummer.Text);
            }
        }

        public static string CreateIban(string blz, string kntnr, bool groupedReturn = true)
        {
            string lkz = "DE";

            string bban = blz.PadLeft(8, '0') + kntnr.PadLeft(10, '0');

            string sum = bban + lkz.Aggregate("", (current, c) => current + (c - 55).ToString()) + "00";

            try
            {
                var d = decimal.Parse(sum);
                var checksum = 98 - (d % 97);
                string iban = lkz + checksum.ToString().PadLeft(2, '0') + bban;
                return groupedReturn ? FormatIBAN(iban) : iban;
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static string FormatIBAN(string iban)
        {
            iban = iban?.ToUpper()?.Trim();
            if (iban == null)
                return string.Empty;

            iban = Regex.Replace(iban, @"\s+", "");

            return iban.Select((c, i) => (i % 4 == 3) ? c + " " : c + "").Aggregate("", (current, c) => current + c);
        }

        private void Txt_kontonummer_TextChanged(object sender, EventArgs e)
        {
            UpdateIban();
        }

        public async Task<string> WaitForTanAsync(TANDialog tanDialog)
        {
            HBCIOutput(tanDialog.DialogResult.Messages);

            if (tanDialog.IsDecoupled)
            {
                _tanReady = true;
                return await Task.FromResult((string)null);
            }

            if (tanDialog.MatrixImage != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    tanDialog.MatrixImage.SaveAsBmp(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    pBox_tan.Image = new System.Drawing.Bitmap(memoryStream);
                }
            }

            txt_tan.BackColor = System.Drawing.Color.LightYellow;
            txt_tan.Focus();

            while (!_tanReady && !_closing)
            {
                Application.DoEvents();
            }
            var tan = txt_tan.Text;

            txt_tan.BackColor = System.Drawing.Color.White;
            txt_tan.Text = string.Empty;

            _tanReady = false;

            return await Task.FromResult(tan);
        }

        private async Task<bool> InitTANMedium(FinTsClient client)
        {
            // TAN-Medium-Name
            client.HITAB = txt_tan_medium.Text;
            var accounts = await client.Accounts(CreateTANDialog(client));
            if (!accounts.IsSuccess)
            {
                HBCIOutput(accounts.Messages);
                return false;
            }
            var conn = client.ConnectionDetails;
            AccountInformation accountInfo = UPD.GetAccountInformations(conn.Account, conn.Blz.ToString());
            if (accountInfo != null && accountInfo.IsSegmentPermitted("HKTAB"))
            {
                client.HITAB = txt_tan_medium.Text;
            }

            return true;
        }

        private static readonly string AccountFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "account.csv");

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _closing = true;

            string account = $"{txt_kontonummer.Text};{txt_bankleitzahl.Text};{txt_bankleitzahl_zentrale.Text};{txt_bic.Text};{txt_iban.Text};{txt_url.Text};{txt_hbci_version.Text};{txt_userid.Text};{txt_tanverfahren.Text};{txt_tan_medium.Text}";

            File.WriteAllText(AccountFile, account);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _bankList = Bank.GetBankList();

            if (chk_tracing.Checked)
                Config.Tracing(true, chk_tracingFormatted.Checked);

            if (File.Exists(AccountFile))
            {
                var content = File.ReadAllText(AccountFile);
                var fields = content.Split(';');
                if (fields.Length == 10)
                {
                    txt_kontonummer.Text = fields[0];
                    txt_bankleitzahl.Text = fields[1];
                    txt_bankleitzahl_zentrale.Text = fields[2];
                    txt_bic.Text = fields[3];
                    txt_iban.Text = fields[4];
                    txt_url.Text = fields[5];
                    txt_hbci_version.Text = fields[6];
                    txt_userid.Text = fields[7];
                    txt_tanverfahren.Text = fields[8];
                    txt_tan_medium.Text = fields[9];
                    txt_pin.Focus();
                }
            }

            var dir = FinTsGlobals.ProgramBaseDir;
            var productIdFile = Path.Combine(dir, "Product_Id.txt");

            if (File.Exists(productIdFile))
                FinTsGlobals.ProductId = File.ReadAllText(productIdFile);

            chk_umsatzabruf_von.Checked = true;
            date_umsatzabruf_von.Value = DateTime.Now.AddDays(-90);
        }

        private TANDialog CreateTANDialog(FinTsClient client)
        {
            var dialog = new TANDialog(WaitForTanAsync, pBox_tan);
            if (client.HIRMS == "922")
                dialog.IsDecoupled = true;

            return dialog;
        }

        private async void btn_terminueberweisungen_abholen_Click(object sender, EventArgs e)
        {
            var connectionDetails = GetConnectionDetails();
            var client = new FinTsClient(connectionDetails);
            var sync = await client.Synchronization();

            HBCIOutput(sync.Messages);

            if (sync.IsSuccess)
            {
                // TAN-Verfahren
                client.HIRMS = txt_tanverfahren.Text;

                if (!await InitTANMedium(client))
                    return;

                var result = await client.GetTerminatedTransfers(CreateTANDialog(client));

                HBCIOutput(result.Messages);

                if (result.Data != null && result.Data.Count > 0)
                {
                    foreach (var item in result.Data)
                    {
                        Pain00100103CtData.PaymentInfo paymentData = item.SepaData.Payments.FirstOrDefault();
                        var txInfo = paymentData.CreditTxInfos.FirstOrDefault();

                        SimpleOutput(
                            "Auftrags-Id: " + item.OrderId + " | " +
                            (item.Deleteable != null ? "Löschbar: " + item.Deleteable + " | " : null) +
                            (item.Modifiable != null ? "Löschbar: " + item.Modifiable + " | " : null) +
                            "Empfänger: " + txInfo.Creditor + " | " +
                            "Betrag: " + txInfo.Amount + " | " +
                            "Verwendungszweck: " + txInfo.RemittanceInformation + " | " +
                            "Ausführung: " + $"{paymentData.RequestedExecutionDate:d}");
                    }
                }
            }
        }
    }

}
