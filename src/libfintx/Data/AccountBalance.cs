using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libfintx
{
    /// <summary>
    /// Returns an account's balance including currency and status information
    /// </summary>
    public class AccountBalance
    {

        /// <summary>
        /// Type of the requested account
        /// </summary>
        public AccountInformations AccountType { get; set; }
        
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
