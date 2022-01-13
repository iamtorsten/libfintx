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

namespace libfintx.FinTS
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
        public string OwnerBankCode { get; set; }

        /// <summary>
        /// IBAN of the transaction partner
        /// </summary>
        public string PartnerIban { get; set; }

        /// <summary>
        /// Name of the transaction partner
        /// </summary>
        public string PartnerName { get; set; }

        /// <summary>
        /// BIC of the transaction partner
        /// </summary>
        public string PartnerBic { get; set; }

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
