namespace libfintx.Data
{
    public class ConnectionDetails
    {
        /// <summary>
        /// Url of the HBCI/FinTS endpoint. Can be retrieved from: https://www.hbci-zka.de/institute/institut_auswahl.htm
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// HBCI version number. E.g. '300' for FinTS 3.0
        /// </summary>
        public int HbciVersion { get; set; }

        /// <summary>
        /// Logon ID/username for the bank account
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Logon-Pin for the bank account
        /// </summary>
        public string Pin { get; set; }

        /// <summary>
        /// Name of the Accountholder
        /// </summary>
        public string AccountHolder { get; set; }

        /// <summary>
        /// Accountnumber of the bank account
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Bankcode of the bank account
        /// </summary>
        public int Blz { get; set; }

        /// <summary>
        /// Bankcode of the bank's headquarter (e.g. to be used in Hypovereinsbank)
        /// </summary>
        public int? BlzHeadquarter { get; set; }

        /// <summary>
        /// BLZ needed for message header (HNVSK, HNSHK) - either BlzHeaderquarter or Blz
        /// </summary>
        public int BlzPrimary => BlzHeadquarter ?? Blz;

        /// <summary>
        /// IBAN of the bank account
        /// </summary>
        public string Iban { get; set; }

        /// <summary>
        /// BIC of the bank account
        /// </summary>
        public string Bic { get; set; }

        /// <summary>
        /// System ID (Kundensystem-ID)
        /// </summary>
        public string CustomerSystemId { get; set; }

        public ConnectionDetails()
        {
            HbciVersion = 300;
        }
    }
}