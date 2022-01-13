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

namespace libfintx.FinTS.Swift
{
    public enum SepaPurpose
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
}
