using System;
using System.IO;

using libfintx.Model;

namespace libfintx.Bank
{
    public class Bank
    {
        /// <summary>
        /// Bankauswahl
        /// </summary>
        /// <param name="BLZoderBIC"></param>
        /// <returns></returns>
        public MBank Auswahl(String BLZoderBIC)
        {
            var lines = File.ReadAllLines("Banken.csv");

            foreach (var line in lines)
            {
                if (line.Split(';')[0].Equals(BLZoderBIC) ||
                    line.Split(';')[1].Equals(BLZoderBIC))
                {
                    // BLZ
                    if (BLZoderBIC.StartsWith("0") ||
                    BLZoderBIC.StartsWith("1") ||
                    BLZoderBIC.StartsWith("2") ||
                    BLZoderBIC.StartsWith("3") ||
                    BLZoderBIC.StartsWith("4") ||
                    BLZoderBIC.StartsWith("5") ||
                    BLZoderBIC.StartsWith("6") ||
                    BLZoderBIC.StartsWith("7") ||
                    BLZoderBIC.StartsWith("8") ||
                    BLZoderBIC.StartsWith("9"))
                    {
                        return new MBank() { BLZ = BLZoderBIC, BIC = line.Split(';')[1], BANK = line.Split(';')[2], HBCIVERSION = line.Split(';')[3], URL = line.Split(';')[4] };
                    }
                    else // BIC
                    {
                        return new MBank() { BLZ = line.Split(';')[0], BIC = BLZoderBIC, BANK = line.Split(';')[2], HBCIVERSION = line.Split(';')[3], URL = line.Split(';')[4] };
                    }
                }
                else
                    return null;
            }

            return null;
        }
    }
}
