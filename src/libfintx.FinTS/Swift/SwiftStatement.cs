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
using System.Collections.Generic;

namespace libfintx.FinTS.Swift
{
    public class SwiftStatement
    {
        public string Type { get; set; }

        public string Id { get; set; }

        public string BankCode { get; set; }

        public string AccountCode { get; set; }

        public string Currency { get; set; }

        public decimal StartBalance { get; set; }

        public DateTime StartDate { get; set; }

        public decimal EndBalance { get; set; }

        public DateTime EndDate { get; set; }

        // Begin MT942

        public bool Pending { get; set; }

        public decimal SmallestAmount { get; set; }

        public decimal SmallestCreditAmount { get; set; }

        public DateTime CreationDate { get; set; }

        public int CountDebit { get; set; }

        public decimal AmountDebit { get; set; }

        public int CountCredit { get; set; }

        public decimal AmountCredit { get; set; }

        // End MT942

        public List<SwiftTransaction> SwiftTransactions { get; set; }

        public List<SwiftLine> Lines { get; set; }

        public SwiftStatement()
        {
            SwiftTransactions = new List<SwiftTransaction>();
            Lines = new List<SwiftLine>();
        }
    }
}
