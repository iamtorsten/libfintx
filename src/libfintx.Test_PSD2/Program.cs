using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libfintx;
using libfintx.Data;

namespace libfintx.Test_PSD2
{
	class Program
	{
		static bool Anonymous;

		static void Main(string[] args)
		{
			var connectionDetails = new ConnectionDetails()
			{
				Account = "xxx",
				Blz = 76061482,
				Bic = "GENODEF1HSB",
				Iban = "xxx",
				Url = "https://hbci11.fiducia.de/cgi-bin/hbciservlet",
				HbciVersion = 300,
				UserId = "xxx",
				Pin = "xxx"
			};

			Anonymous = false;

			// Sync
			HBCIOutput(libfintx.Main.Synchronization(connectionDetails).Messages);

			// Transaktionen abfragen
			var trx = libfintx.Main.Transactions(connectionDetails, new TANDialog(WaitForTAN), Anonymous, new DateTime(2019, 10, 1), DateTime.Now);

			HBCIOutput(trx.Messages);

			if (trx.Data != null) // RawData
			{
				foreach (var swift in trx.Data) // SWIFT
				{
					if (swift.SwiftTransactions.Count != 0)
					{
						foreach (var swifttrx in swift.SwiftTransactions) // SWIFT Transaktionen
						{
							// Einzelne Transaktion
							Console.WriteLine(swifttrx.partnerName);
						}
					}
				}
			}

			Console.ReadLine();
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

		public static string WaitForTAN(TANDialog tanDialog)
		{
			foreach (var msg in tanDialog.DialogResult.Messages)
				Console.WriteLine(msg);

			return Console.ReadLine();
		}
	}
}
