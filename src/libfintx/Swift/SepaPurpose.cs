namespace libfintx.Swift
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