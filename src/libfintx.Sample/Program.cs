using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;
using libfintx.FinTS;
using libfintx.FinTS.Data;

namespace libfintx.Sample
{
    class Program
    {
        enum OperationType
        {
            Accounts,
            Balance,
            Transactions
        }

        class Options
        {
            [Option('b', "bic", Required = true, HelpText = "Set the FinTS bank code.")]
            public int BankCode { get; set; }

            [Option('i', "userid", Required = true, HelpText = "Set the FinTS user id.")]
            public string UserId { get; set; }

            [Option('a', "accountnumber", Required = true, HelpText = "Set the FinTS bank account number.")]
            public string Account { get; set; }

            [Option('u', "url", Required = true, HelpText = "Set the FinTS URL.")]
            public string Url { get; set; }

            [Option('p', "pin", Required = true, HelpText = "Set the FinTS PIN.")]
            public string Pin { get; set; }

            [Option('o', "operation", Required = true, HelpText = "Set the FinTS transaction type.")]
            public OperationType Operation { get; set; }
        }

        static async Task<string> WaitForTanAsync(TANDialog tanDialog)
        {
            foreach (var msg in tanDialog.DialogResult.Messages)
                Console.WriteLine(msg);

            return await Task.FromResult(Console.ReadLine());
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Parser.Default.ParseArguments<Options>(args)
            .WithParsed(o =>
            {
                var details = new ConnectionDetails()
                {
                    Url = o.Url,
                    Account = o.Account,
                    Blz = o.BankCode,
                    Pin = o.Pin,
                    UserId = o.UserId,
                };

                var client = new FinTsClient(details);
                switch (o.Operation)
                {
                    case OperationType.Accounts:
                        Accounts(client);
                        break;
                    case OperationType.Balance:
                        Balance(client);
                        break;
                    case OperationType.Transactions:
                        Transactions(client);
                        break;
                }
            });

            Console.ReadLine();
        }

        private static async void Accounts(FinTsClient client)
        {
            var result = await client.Accounts(new TANDialog(WaitForTanAsync));
            if (!result.IsSuccess)
            {
                HBCIOutput(result.Messages);
                return;
            }

            Console.WriteLine("Account count: {0}", result.Data.Count);
            foreach (var account in result.Data)
            {
                Console.WriteLine("Account - Holder: {0}, Number: {1}", account.AccountOwner, account.AccountNumber);
            }
        }

        private static async void Balance(FinTsClient client)
        {
            var result = await client.Balance(new TANDialog(WaitForTanAsync));
            if (!result.IsSuccess)
            {
                HBCIOutput(result.Messages);
                return;
            }

            Console.WriteLine("Balance is: {0}\u20AC", result.Data.Balance);
        }

        private static async void Transactions(FinTsClient client)
        {
            var result = await client.Transactions(new TANDialog(WaitForTanAsync));
            if (!result.IsSuccess)
            {
                HBCIOutput(result.Messages);
                return;
            }

            Console.WriteLine("Transaction count:", result.Data.Count);
            foreach (var trans in result.Data)
            {
                Console.WriteLine("Transaction - Start Date: {0}, Amount: {1}\u20AC", trans.StartDate, trans.EndBalance - trans.StartBalance);
            }
        }

        /// <summary>
        /// HBCI-Nachricht ausgeben
        /// </summary>
        /// <param name="hbcimsg"></param>
        private static void HBCIOutput(IEnumerable<HBCIBankMessage> hbcimsg)
        {
            foreach (var msg in hbcimsg)
            {
                Console.WriteLine("Code: " + msg.Code + " | " + "Typ: " + msg.Type + " | " + "Nachricht: " + msg.Message);
            }
        }
    }
}
