/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2016 - 2022 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 *  
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 3 of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software Foundation,
 *  Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 * 	
 */

using System;
using System.IO;

using libfintx.FinTS.Model;

namespace libfintx.FinTS.Bank
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
