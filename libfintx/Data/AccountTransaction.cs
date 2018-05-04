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


using System;

namespace libfintx
{
    /// <summary>
    /// Simple account transaction object in libfintx-format
    /// </summary>
    public class AccountTransaction
    {
        /// <summary>
        /// Bankaccount of the owner
        /// </summary>
        public string OwnerAccount { get; set; }

        /// <summary>
        /// Bankcode of the owner's bankaccount
        /// </summary>
        public string OwnerBankcode{ get; set; }

        /// <summary>
        /// IBAN of the transaction partner
        /// </summary>
        public string PartnerIBAN { get; set; }

        /// <summary>        
        /// Name of the transaction partner
        /// </summary>
        public string PartnerName { get; set; }


        /// <summary>
        /// BIC of the transaction partner
        /// </summary>
        public string PartnerBIC { get; set; }

        /// <summary>
        /// Text/description of the transaction
        /// </summary>
        public string RemittanceText { get; set; }


        /// <summary>
        /// Type of the transaction (e.g. FOLGELASTSCHRIFT, AUSZAHLUNG, etc.)
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Date of transaction request
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Date of booking (valuta-date)
        /// </summary>
        public DateTime ValueDate { get; set; }
    }
}
