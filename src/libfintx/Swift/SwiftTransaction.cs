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

using System;
using System.Collections.Generic;

namespace libfintx.Swift
{
    public class SwiftTransaction
    {
        public DateTime ValueDate { get; set; }

        public DateTime InputDate { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// Buchungsschlüssel, siehe https://www.hettwer-beratung.de/business-portfolio/zahlungsverkehr/elektr-kontoinformationen-swift-mt-940/
        /// </summary>
        public string TransactionTypeId { get; set; }

        public string CustomerReference { get; set; }

        public string BankReference { get; set; }

        public string OtherInformation { get; set; }

        public string Text { get; set; }

        public string Primanota { get; set; }

        public string TypeCode { get; set; }

        public string Description { get; set; }

        public Dictionary<SepaPurpose, string> SepaPurposes { get; set; }

        public string BankCode { get; set; }

        public string AccountCode { get; set; }

        public string PartnerName { get; set; }

        public string TextKeyAddition { get; set; }

        //Ende-zu-Ende Referenz
        public string EREF { get; set; }
        //Kundenreferenz
        public string KREF { get; set; }
        //Mandatsreferenz
        public string MREF { get; set; }
        //Bankreferenz
        public string BREF { get; set; }
        //Retourenreferenz
        public string RREF { get; set; }
        //Creditor-ID
        public string CRED { get; set; }
        //Debitor-ID
        public string DEBT { get; set; }
        //Zinskompensationsbetrag
        public string COAM { get; set; }
        //Ursprünglicher Umsatzbetrag
        public string OAMT { get; set; }
        //Verwendungszweck
        public string SVWZ { get; set; }
        //Abweichender Auftraggeber
        public string ABWA { get; set; }
        //Abweichender Empfänger
        public string ABWE { get; set; }
        //IBAN des Auftraggebers
        public string IBAN { get; set; }
        //BIC des Auftraggebers
        public string BIC { get; set; }

        public SwiftTransaction()
        {
            SepaPurposes = new Dictionary<SepaPurpose, string>();
        }
    }
}
