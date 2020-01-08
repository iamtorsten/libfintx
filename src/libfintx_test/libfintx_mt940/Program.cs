using libfintx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libfintx_mt940
{
    class Program
    {
        const string Mt940File = @"C:\Users\abidh\Desktop\Trace.txt";

        static void Main(string[] args)
        {
            var mt940 = File.ReadAllText(Mt940File);

            var statemtents = MT940.Serialize(mt940, null);

            var transactions = statemtents.SelectMany(s => s.SwiftTransactions);

            foreach (var tx in transactions)
            {
                Console.WriteLine($"{tx.InputDate};{tx.ValueDate};{tx.Amount};{tx.Description};{tx.Text}");
            }

            Console.ReadLine();
        }
    }
}
