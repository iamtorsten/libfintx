/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2018 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 * 	
 * 	libfintx is free software; you can redistribute it and/or
 *	modify it under the terms of the GNU Lesser General Public
 * 	License as published by the Free Software Foundation; either
 * 	version 2.1 of the License, or (at your option) any later version.
 *	
 * 	libfintx is distributed in the hope that it will be useful,
 * 	but WITHOUT ANY WARRANTY; without even the implied warranty of
 * 	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * 	Lesser General Public License for more details.
 *	
 * 	You should have received a copy of the GNU Lesser General Public
 * 	License along with libfintx; if not, write to the Free Software
 * 	Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 * 	
 */

/*
 *
 *	Based on Timotheus Pokorra's C# implementation of OpenPetraPlugin_BankimportCAMT
 *	available at https://github.com/SolidCharity/OpenPetraPlugin_BankimportCAMT/blob/master/Client/ParseCAMT.cs
 *
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace libfintx
{
    /// todoComment
    public class TStatement
    {
        /// todoComment
        public string id;

        /// todoComment
        public string bankCode;

        /// todoComment
        public string accountCode;

        /// todoComment
        public string currency;

        /// todoComment
        public decimal startBalance;

        /// todoComment
        public decimal endBalance;

        /// todoComment
        public DateTime date;

        /// across several years
        public bool severalYears;

        /// todoComment
        public List<TTransaction> transactions = new List<TTransaction>();

        public override string ToString()
        {
            return new StringBuilder()
                .Append($"Id: {id}")
                .Append($", BankCode: {bankCode}")
                .Append($", AccountCode: {accountCode}")
                .Append($", Currency: {currency}")
                .Append($", StartBalance: {startBalance}")
                .Append($", EndBalance: {endBalance}")
                .Append($", Date: {date}")
                .Append($", SeveralYears: {severalYears}")
                .Append($", Transactions: {transactions.Count}")
                .ToString();
        }
    }
}
