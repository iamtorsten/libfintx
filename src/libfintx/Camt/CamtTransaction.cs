/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2016 - 2021 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 *  
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *  
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU Affero General Public License for more details.
 *  
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program. If not, see <http://www.gnu.org/licenses/>.
 * 	
 */

/*
 *
 *	Based on Timotheus Pokorra's C# implementation of OpenPetraPlugin_BankimportCAMT
 *	available at https://github.com/SolidCharity/OpenPetraPlugin_BankimportCAMT/blob/master/Client/ParseCAMT.cs
 *
 */

using System;

namespace libfintx.Camt
{
    public class CamtTransaction
    {
        /// <summary>
        /// Pending (Vorgemerkt)
        /// </summary>
        public bool Pending { get; set; }

        public DateTime ValueDate { get; set; }

        public DateTime InputDate { get; set; }

        public decimal Amount { get; set; }

        public string Text { get; set; }

        /// <summary>
        /// Buchungsschlüssel
        /// </summary>
        public string TransactionTypeId { get; set; }

        /// <summary>
        /// Geschäftsvorfallcode
        /// </summary>
        public string TypeCode { get; set; }

        public string Primanota { get; set; }

        public string TextKeyAddition { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// BIC of the counterpart
        /// </summary>
        public string BankCode { get; set; }

        /// <summary>
        /// IBAN of the counterpart
        /// </summary>
        public string AccountCode { get; set; }

        /// <summary>
        /// Name of the counterpart
        /// </summary>
        public string PartnerName { get; set; }

        public string EndToEndId { get; set; }

        public string MandateId { get; set; }

        /// <summary>
        /// Unique Identifier (if available)
        /// </summary>
        public string ProprietaryRef { get; set; }

        public string CustomerRef { get; set; }

        public string PaymentInformationId { get; set; }

        public string MessageId { get; set; }

        public bool Storno { get; set; }
    }
}
