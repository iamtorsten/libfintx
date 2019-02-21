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
using System.Collections.Generic;

namespace libfintx
{
    public class SWIFTTransaction
    {
        public enum SEPAPurpose
        {
            // https://www.hettwer-beratung.de/sepa-spezialwissen/sepa-technische-anforderungen/sepa-gesch%C3%A4ftsvorfallcodes-gvc-mt-940/

            IBAN, // SEPA IBAN Auftraggeber
            BIC, // SEPA BIC Auftraggeber
            EREF, // SEPA End to End-Referenz
            KREF, // Kundenreferenz
            MREF, // SEPA Mandatsreferenz
            CRED, // SEPA Creditor Identifier
            DEBT, // Originator Identifier
            COAM, // Zinskompensationsbetrag
            OAMT, // Ursprünglicher Umsatzbetrag
            SVWZ, // SEPA Verwendungszweck
            ABWA, // Abweichender SEPA Auftraggeber
            ABWE, // Abweichender SEPA Empfänger
            BREF, // Bankreferenz, Instruction ID
            RREF // Retourenreferenz
        }

        public SWIFTTransaction()
        {
            SEPAPurposes = new Dictionary<SEPAPurpose, string>();
        }

        public DateTime valueDate;

        public DateTime inputDate;

        public decimal amount;

        public string text;

        public string typecode;

        public string description;

        public Dictionary<SEPAPurpose, string> SEPAPurposes;

        public string bankCode;

        public string accountCode;

        public string partnerName;

		//Ende-zu-Ende Referenz
		public string EREF;
		//Kundenreferenz
		public string KREF;
		//Mandatsreferenz
		public string MREF;
		//Bankreferenz
		public string BREF;
		//Retourenreferenz
		public string RREF;
		//Creditor-ID
		public string CRED;
		//Debitor-ID
		public string DEBT;
		//Zinskompensationsbetrag
		public string COAM;
		//Ursprünglicher Umsatzbetrag
		public string OAMT;
		//Verwendungszweck
		public string SVWZ;
		//Abweichender Auftraggeber
		public string ABWA;
		//Abweichender Empfänger
		public string ABWE;
		//IBAN des Auftraggebers
		public string IBAN;
		//BIC des Auftraggebers
		public string BIC;
	}
}
