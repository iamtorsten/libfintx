/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2021 - 2022 Abid Hussain
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

namespace libfintx.FinTS
{
    /// <summary>
    /// Returns an account's balance including currency and status information
    /// </summary>
    public class AccountBalance
    {

        /// <summary>
        /// Type of the requested account
        /// </summary>
        public AccountInformation AccountType { get; set; }

        /// <summary>
        /// Balance as decimal value
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// (optional) Transactions which are noted, but not already booked. Will be null if not delivered out by bank.
        /// </summary>
        public decimal? MarkedTransactions { get; set; }

        /// <summary>
        /// (optional) CreditLine - maximum possible credit. Will be null if not delivered out by bank.
        /// </summary>
        public decimal? CreditLine { get; set; }

        /// <summary>
        /// (optional) Balance available including credit line. Will be null if not delivered out by bank.
        /// </summary>
        public decimal? AvailableBalance { get; set; }

        /// <summary>
        /// Returns status of balance request
        /// </summary>
        public bool Successful { get; set; }

        /// <summary>
        /// Returns error message if balance request failed (Successful==false) or status message if request was successful
        /// </summary>
        public string Message { get; set; }
    }
}
