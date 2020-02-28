using libfintx.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace libfintx
{
    public partial class FinTsClient
    {
        public bool Anonymous { get; }
        public ConnectionDetails ConnectionDetails { get; }
        public string SystemId { get; internal set; }
        public string HITAB { get; set; }
        public string HIRMS { get; set; }
        public string HITANS { get; set; }

        internal int SEGNUM { get; set; }
        internal string HIRMSf { get; set; }
        internal string HNHBK { get; set; }
        internal string HNHBS { get; set; }
        internal string HISALS { get; set; }
        internal string HISALSf { get; set; }
        internal string HKKAZ { get; set; }
        internal string HKCAZ { get { return "1"; } }
        internal string HITAN { get; set; }
        internal int HISPAS { get; set; }

        public FinTsClient(ConnectionDetails conn, bool anon = false)
        {
            ConnectionDetails = conn;
            Anonymous = anon;
        }

        internal async Task<HBCIDialogResult> InitializeConnection()
        {
            if (HKTAN.SegmentId == null)
                HKTAN.SegmentId = "HKIDN";

            HBCIDialogResult result;
            string BankCode;
            try
            {
                // Check if the user provided a SystemID
                if (ConnectionDetails.CustomerSystemId == null)
                {
                    result = await Synchronization();
                    if (!result.IsSuccess)
                    {
                        Log.Write("Synchronisation failed.");
                        return result;
                    }
                }
                else
                {
                    SystemId = ConnectionDetails.CustomerSystemId;
                }
                BankCode = await Transaction.INI(this);
            }
            finally
            {
                HKTAN.SegmentId = null;
            }

            var bankMessages = Helper.Parse_BankCode(BankCode);
            result = new HBCIDialogResult(bankMessages, BankCode);
            if (!result.IsSuccess)
                Log.Write("Initialisation failed: " + result);

            return result;
        }

        /// <summary>
        /// Synchronize bank connection
        /// </summary>
        /// <param name="conn">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz</param>
        /// <returns>
        /// Customer System ID
        /// </returns>
        public async Task<HBCIDialogResult<string>> Synchronization()
        {
            string BankCode = await Transaction.HKSYN(this);

            var messages = Helper.Parse_BankCode(BankCode);

            return new HBCIDialogResult<string>(messages, BankCode, SystemId);
        }

        /// <summary>
        /// Retrieves the accounts for this client
        /// </summary>
        /// <param name="tanDialog">The TAN Dialog</param>
        /// <returns>Gets informations about the accounts</returns>
        public async Task<HBCIDialogResult<List<AccountInformation>>> Accounts(TANDialog tanDialog)
        {
            var result = await InitializeConnection();
            if (!result.IsSuccess)
                return result.TypedResult<List<AccountInformation>>();

            result = await ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<List<AccountInformation>>();

            return new HBCIDialogResult<List<AccountInformation>>(result.Messages, UPD.Value, UPD.HIUPD.AccountList);
        }

        /// <summary>
        /// Account balance
        /// </summary>
        /// <param name="tanDialog">The TAN Dialog</param>
        /// <returns>The balance for this account</returns>
        public async Task<HBCIDialogResult<AccountBalance>> Balance(TANDialog tanDialog)
        {
            HBCIDialogResult result = await InitializeConnection();
            if (!result.IsSuccess)
                return result.TypedResult<AccountBalance>();

            result = await ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<AccountBalance>();

            // Success
            string BankCode = await Transaction.HKSAL(this);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result.TypedResult<AccountBalance>();

            result = await ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<AccountBalance>();

            BankCode = result.RawData;
            AccountBalance balance = Helper.Parse_Balance(BankCode);
            return result.TypedResult(balance);
        }

        private async Task<HBCIDialogResult> ProcessSCA(HBCIDialogResult result, TANDialog tanDialog)
        {
            tanDialog.DialogResult = result;
            if (result.IsSCARequired)
            {
                string tan = Helper.WaitForTAN(this, result, tanDialog);
                if (tan == null)
                {
                    string BankCode = await Transaction.HKEND(this, HNHBK);
                    result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
                }
                else
                {
                    result = await TAN(tan);
                }
            }

            return result;
        }

        /// <summary>
        /// Rebook money from one to another account - General method
        /// </summary>
        /// <param name="receiverName">Name of the recipient</param>
        /// <param name="receiverIBAN">IBAN of the recipient</param>
        /// <param name="receiverBIC">BIC of the recipient</param>
        /// <param name="purpose">Short description of the transfer (dt. Verwendungszweck)</param>      
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 911 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="anonymous"></param>
        /// <param name="flickerImage">(Out) reference to an image object that shall receive the FlickerCode as GIF image</param>
        /// <param name="flickerWidth">Width of the flicker code</param>
        /// <param name="flickerHeight">Height of the flicker code</param>
        /// <param name="renderFlickerCodeAsGif">Renders flicker code as GIF, if 'true'</param>  
        /// <returns>
        /// Bank return codes
        /// </returns>
        public async Task<HBCIDialogResult> Rebooking(TANDialog tanDialog, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, string hirms)
        {
            var result = await InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = await ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!string.IsNullOrEmpty(hirms))
                HIRMS = hirms;

            string BankCode = await Transaction.HKCUM(this, receiverName, receiverIBAN, receiverBIC, amount, purpose);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = await ProcessSCA(result, tanDialog);

            return result;
        }

        /// <summary>
        /// Collect money from another account - General method
        /// </summary>
        /// <param name="payerName">Name of the payer</param>
        /// <param name="payerIBAN">IBAN of the payer</param>
        /// <param name="payerBIC">BIC of the payer</param>         
        /// <param name="amount">Amount to transfer</param>
        /// <param name="purpose">Short description of the transfer (dt. Verwendungszweck)</param>    
        /// <param name="settlementDate"></param>
        /// <param name="mandateNumber"></param>
        /// <param name="mandateDate"></param>
        /// <param name="creditorIdNumber"></param>
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 911 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="flickerImage">(Out) reference to an image object that shall receive the FlickerCode as GIF image</param>
        /// <param name="flickerWidth">Width of the flicker code</param>
        /// <param name="flickerHeight">Height of the flicker code</param>
        /// <param name="renderFlickerCodeAsGif">Renders flicker code as GIF, if 'true'</param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public async Task<HBCIDialogResult> Collect(TANDialog tanDialog, string payerName, string payerIBAN, string payerBIC,
            decimal amount, string purpose, DateTime settlementDate, string mandateNumber, DateTime mandateDate, string creditorIdNumber,
            string hirms)
        {
            var result = await InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = await ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!string.IsNullOrEmpty(hirms))
                HIRMS = hirms;

            string BankCode = await Transaction.HKDSE(this, payerName, payerIBAN, payerBIC, amount, purpose, settlementDate, mandateNumber, mandateDate, creditorIdNumber);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = await ProcessSCA(result, tanDialog);

            return result;
        }

        /// <summary>
        /// Collective collect money from other accounts - General method
        /// </summary>
        /// <param name="settlementDate"></param>
        /// <param name="painData"></param>
        /// <param name="numberOfTransactions"></param>
        /// <param name="totalAmount"></param>        
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 911 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="flickerImage">(Out) reference to an image object that shall receive the FlickerCode as GIF image</param>
        /// <param name="flickerWidth">Width of the flicker code</param>
        /// <param name="flickerHeight">Height of the flicker code</param>
        /// <param name="renderFlickerCodeAsGif">Renders flicker code as GIF, if 'true'</param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public async Task<HBCIDialogResult> CollectiveCollect(TANDialog tanDialog, DateTime settlementDate, List<Pain00800202CcData> painData,
           string numberOfTransactions, decimal totalAmount, string hirms)
        {
            var result = await InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = await ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!string.IsNullOrEmpty(hirms))
                HIRMS = hirms;

            string BankCode = await Transaction.HKDME(this, settlementDate, painData, numberOfTransactions, totalAmount);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = await ProcessSCA(result, tanDialog);

            return result;
        }

        /// <summary>
        /// Load mobile phone prepaid card - General method
        /// </summary>
        /// <param name="mobileServiceProvider"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="amount">Amount to transfer</param>            
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 911 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="flickerImage">(Out) reference to an image object that shall receive the FlickerCode as GIF image</param>
        /// <param name="flickerWidth">Width of the flicker code</param>
        /// <param name="flickerHeight">Height of the flicker code</param>
        /// <param name="renderFlickerCodeAsGif">Renders flicker code as GIF, if 'true'</param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public async Task<HBCIDialogResult> Prepaid(TANDialog tanDialog, int mobileServiceProvider, string phoneNumber,
            int amount, string hirms)
        {
            var result = await InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = await ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!string.IsNullOrEmpty(hirms))
                HIRMS = hirms;

            string BankCode = await Transaction.HKPPD(this, mobileServiceProvider, phoneNumber, amount);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = await ProcessSCA(result, tanDialog);

            return result;
        }

        /// <summary>
        /// Synchronize bank connection RDH
        /// </summary>
        /// <param name="BLZ"></param>
        /// <param name="URL"></param>
        /// <param name="Port"></param>
        /// <param name="HBCIVersion"></param>
        /// <param name="UserID"></param>
        /// <returns>
        /// Success or failure
        /// </returns>
        private bool Synchronization_RDH(int BLZ, string URL, int Port, int HBCIVersion, string UserID, string FilePath, string Password)
        {
            if (Transaction.INI_RDH(this, BLZ, URL, Port, HBCIVersion, UserID, FilePath, Password) == true)
            {
                return true;
            }
            else
                return false;
        }
    }
}
