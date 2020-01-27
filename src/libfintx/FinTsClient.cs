using libfintx.Camt;
using libfintx.Camt.Camt052;
using libfintx.Camt.Camt053;
using libfintx.Data;
using libfintx.Swift;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using static libfintx.HKCDE;

namespace libfintx
{
    public class FinTsClient
    {
        public bool Anonymous { get; }
        public ConnectionDetails ConnectionDetails { get; }
        public string SystemId { get; internal set; }
        public string HITAB { get; set; }

        internal string HIRMS { get; set; }
        internal string HIRMSf { get; set; }
        internal string HNHBK { get; set; }
        internal string HNHBS { get; set; }
        internal string HISALS { get; set; }
        internal string HISALSf { get; set; }
        internal string HITANS { get; set; }
        internal string HKKAZ { get; set; }
        internal string HKCAZ { get { return "1"; } }
        internal string HITAN { get; set; }
        internal int HISPAS { get; set; }

        public FinTsClient(ConnectionDetails conn, bool anon = false)
        {
            ConnectionDetails = conn;
            Anonymous = anon;
        }

        internal HBCIDialogResult InitializeConnection()
        {
            HBCIDialogResult result;
            string BankCode;
            try
            {
                // Check if the user provided a SystemID
                if (ConnectionDetails.CustomerSystemId == null)
                {
                    result = Synchronization();
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
                BankCode = Transaction.INI(this);
            }
            finally
            {
                HKTAN.SegmentId = null;
            }

            List<HBCIBankMessage> bankMessages = Helper.Parse_BankCode(BankCode);
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
        public HBCIDialogResult<string> Synchronization()
        {
            string BankCode = Transaction.HKSYN(this);

            List<HBCIBankMessage> messages = Helper.Parse_BankCode(BankCode);

            return new HBCIDialogResult<string>(messages, BankCode, SystemId);
        }

        /// <summary>
        /// Retrieves the accounts for this client
        /// </summary>
        /// <param name="tanDialog">The TAN Dialog</param>
        /// <returns>Gets informations about the accounts</returns>
        public HBCIDialogResult<List<AccountInformation>> Accounts(TANDialog tanDialog)
        {
            HBCIDialogResult result = InitializeConnection();
            if (!result.IsSuccess)
                return result.TypedResult<List<AccountInformation>>();

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<List<AccountInformation>>();

            return new HBCIDialogResult<List<AccountInformation>>(result.Messages, UPD.Value, UPD.HIUPD.AccountList);
        }

        /// <summary>
        /// Account balance
        /// </summary>
        /// <param name="tanDialog">The TAN Dialog</param>
        /// <returns>The balance for this account</returns>
        public HBCIDialogResult<AccountBalance> Balance(TANDialog tanDialog)
        {
            HBCIDialogResult result = InitializeConnection();
            if (!result.IsSuccess)
                return result.TypedResult<AccountBalance>();

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<AccountBalance>();

            // Success
            string BankCode = Transaction.HKSAL(this);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result.TypedResult<AccountBalance>();

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<AccountBalance>();

            BankCode = result.RawData;
            AccountBalance balance = Helper.Parse_Balance(BankCode);
            return result.TypedResult(balance);
        }

        private HBCIDialogResult ProcessSCA(HBCIDialogResult result, TANDialog tanDialog)
        {
            tanDialog.DialogResult = result;
            if (result.IsSCARequired)
            {
                string tan = Helper.WaitForTAN(this, result, tanDialog);
                if (tan == null)
                {
                    string BankCode = Transaction.HKEND(this, HNHBK);
                    result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
                }
                else
                {
                    result = TAN(tan);
                }
            }

            return result;
        }

        /// <summary>
        /// Confirm order with TAN
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz</param>
        /// <param name="TAN"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public HBCIDialogResult TAN(string TAN)
        {
            string BankCode = Transaction.TAN(this, TAN);
            HBCIDialogResult result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);

            return result;
        }

        /// <summary>
        /// Account transactions in SWIFT-format
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, Account, IBAN, BIC</param>  
        /// <param name="anonymous"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>
        /// Transactions
        /// </returns>
        public HBCIDialogResult<List<SwiftStatement>> Transactions(TANDialog tanDialog, DateTime? startDate = null, DateTime? endDate = null, bool saveMt940File = false)
        {
            HBCIDialogResult result = InitializeConnection();
            if (!result.IsSuccess)
                return result.TypedResult<List<SwiftStatement>>();

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<List<SwiftStatement>>();

            string startDateStr = startDate?.ToString("yyyyMMdd");
            string endDateStr = endDate?.ToString("yyyyMMdd");

            // Success
            string BankCode = Transaction.HKKAZ(this, startDateStr, endDateStr, null);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result.TypedResult<List<SwiftStatement>>();

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<List<SwiftStatement>>();

            BankCode = result.RawData;
            StringBuilder TransactionsMt940 = new StringBuilder();
            StringBuilder TransactionsMt942 = new StringBuilder();

            Regex regex = new Regex(@"HIKAZ:.+?@\d+@(?<mt940>.+?)(\+@\d+@(?<mt942>.+?))?('{1,2}H[A-Z]{4}:\d+:\d+)", RegexOptions.Singleline);
            Match match = regex.Match(BankCode);
            if (match.Success)
            {
                TransactionsMt940.Append(match.Groups["mt940"].Value);
                TransactionsMt942.Append(match.Groups["mt942"].Value);
            }

            string BankCode_ = BankCode;
            while (BankCode_.Contains("+3040::"))
            {
                Helper.Parse_Message(this, BankCode_);

                string Startpoint = new Regex(@"\+3040::[^:]+:(?<startpoint>[^']+)'").Match(BankCode_).Groups["startpoint"].Value;

                BankCode_ = Transaction.HKKAZ(this, startDateStr, endDateStr, Startpoint);
                result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode_), BankCode_);
                if (!result.IsSuccess)
                    return result.TypedResult<List<SwiftStatement>>();

                result = ProcessSCA(result, tanDialog);
                if (!result.IsSuccess)
                    return result.TypedResult<List<SwiftStatement>>();

                BankCode_ = result.RawData;
                match = regex.Match(BankCode_);
                if (match.Success)
                {
                    TransactionsMt940.Append(match.Groups["mt940"].Value);
                    TransactionsMt942.Append(match.Groups["mt942"].Value);
                }
            }

            List<SwiftStatement> swiftStatements = new List<SwiftStatement>();

            swiftStatements.AddRange(MT940.Serialize(TransactionsMt940.ToString(), ConnectionDetails.Account, saveMt940File));
            swiftStatements.AddRange(MT940.Serialize(TransactionsMt942.ToString(), ConnectionDetails.Account, saveMt940File, true));

            return result.TypedResult(swiftStatements);
        }

        /// <summary>
        /// Account transactions in camt format
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>
        /// Transactions
        /// </returns>
        public HBCIDialogResult<List<CamtStatement>> Transactions_camt(ConnectionDetails connectionDetails, TANDialog tanDialog, bool anonymous, CamtVersion camtVers,
            DateTime? startDate = null, DateTime? endDate = null, bool saveCamtFile = false)
        {
            HBCIDialogResult result = InitializeConnection();
            if (!result.IsSuccess)
                return result.TypedResult<List<CamtStatement>>();

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<List<CamtStatement>>();

            // Plain camt message
            string camt = string.Empty;

            string startDateStr = startDate?.ToString("yyyyMMdd");
            string endDateStr = endDate?.ToString("yyyyMMdd");

            // Success
            string BankCode = Transaction.HKCAZ(this, startDateStr, endDateStr, null, camtVers);
            result = new HBCIDialogResult<List<CamtStatement>>(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result.TypedResult<List<CamtStatement>>();

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<List<CamtStatement>>();

            BankCode = result.RawData;
            List<CamtStatement> statements = new List<CamtStatement>();

            Camt052Parser camt052Parser = null;
            Camt053Parser camt053Parser = null;
            Encoding encoding = Encoding.GetEncoding("ISO-8859-1");

            string BankCode_ = BankCode;

            // Es kann sein, dass in der payload mehrere Dokumente enthalten sind
            int xmlStartIdx = BankCode_.IndexOf("<?xml version=");
            int xmlEndIdx = BankCode_.IndexOf("</Document>") + "</Document>".Length;
            while (xmlStartIdx >= 0)
            {
                if (xmlStartIdx > xmlEndIdx)
                    break;

                camt = "<?xml version=" + Helper.Parse_String(BankCode_, "<?xml version=", "</Document>") + "</Document>";

                switch (camtVers)
                {
                    case CamtVersion.Camt052:
                        if (camt052Parser == null)
                            camt052Parser = new Camt052Parser();

                        if (saveCamtFile)
                        {
                            // Save camt052 statement to file
                            string camt052f = Camt052File.Save(connectionDetails.Account, camt, encoding);

                            // Process the camt052 file
                            camt052Parser.ProcessFile(camt052f);
                        }
                        else
                        {
                            camt052Parser.ProcessDocument(camt, encoding);
                        }

                        statements.AddRange(camt052Parser.statements);
                        break;
                    case CamtVersion.Camt053:
                        if (camt053Parser == null)
                            camt053Parser = new Camt053Parser();

                        if (saveCamtFile)
                        {
                            // Save camt053 statement to file
                            string camt053f = Camt053File.Save(connectionDetails.Account, camt, encoding);

                            // Process the camt053 file
                            camt053Parser.ProcessFile(camt053f);
                        }
                        else
                        {
                            camt053Parser.ProcessDocument(camt, encoding);
                        }

                        statements.AddRange(camt053Parser.statements);
                        break;
                }

                BankCode_ = BankCode_.Substring(xmlEndIdx);
                xmlStartIdx = BankCode_.IndexOf("<?xml version");
                xmlEndIdx = BankCode_.IndexOf("</Document>") + "</Document>".Length;
            }

            BankCode_ = BankCode;

            while (BankCode_.Contains("+3040::"))
            {
                string Startpoint = new Regex(@"\+3040::[^:]+:(?<startpoint>[^']+)'").Match(BankCode_).Groups["startpoint"].Value;
                BankCode_ = Transaction.HKCAZ(this, startDateStr, endDateStr, Startpoint, camtVers);
                result = new HBCIDialogResult<List<CamtStatement>>(Helper.Parse_BankCode(BankCode_), BankCode_);
                if (!result.IsSuccess)
                    return result.TypedResult<List<CamtStatement>>();

                BankCode_ = result.RawData;

                // Es kann sein, dass in der payload mehrere Dokumente enthalten sind
                xmlStartIdx = BankCode_.IndexOf("<?xml version=");
                xmlEndIdx = BankCode_.IndexOf("</Document>") + "</Document>".Length;

                while (xmlStartIdx >= 0)
                {
                    if (xmlStartIdx > xmlEndIdx)
                        break;

                    camt = "<?xml version=" + Helper.Parse_String(BankCode_, "<?xml version=", "</Document>") + "</Document>";

                    switch (camtVers)
                    {
                        case CamtVersion.Camt052:
                            // Save camt052 statement to file
                            string camt052f_ = Camt052File.Save(connectionDetails.Account, camt);

                            // Process the camt052 file
                            camt052Parser.ProcessFile(camt052f_);

                            // Add all items
                            statements.AddRange(camt052Parser.statements);
                            break;
                        case CamtVersion.Camt053:
                            // Save camt053 statement to file
                            string camt053f_ = Camt053File.Save(connectionDetails.Account, camt);

                            // Process the camt053 file
                            camt053Parser.ProcessFile(camt053f_);

                            // Add all items to existing statement
                            statements.AddRange(camt053Parser.statements);
                            break;
                    }

                    BankCode_ = BankCode_.Substring(xmlEndIdx);
                    xmlStartIdx = BankCode_.IndexOf("<?xml version");
                    xmlEndIdx = BankCode_.IndexOf("</Document>") + "</Document>".Length;
                }
            }

            return result.TypedResult(statements);
        }

        /// <summary>
        /// Account transactions in simplified libfintx-format
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, Account, IBAN, BIC</param>  
        /// <param name="anonymous"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>
        /// Transactions
        /// </returns>
        public HBCIDialogResult<List<AccountTransaction>> TransactionsSimple(TANDialog tanDialog, DateTime? startDate = null, DateTime? endDate = null)
        {
            HBCIDialogResult<List<SwiftStatement>> result = Transactions(tanDialog, startDate, endDate);
            if (!result.IsSuccess)
                return result.TypedResult<List<AccountTransaction>>();

            List<AccountTransaction> transactionList = new List<AccountTransaction>();
            foreach (SwiftStatement swiftStatement in result.Data)
            {
                foreach (SwiftTransaction swiftTransaction in swiftStatement.SwiftTransactions)
                {
                    transactionList.Add(new AccountTransaction()
                    {
                        OwnerAccount = swiftStatement.AccountCode,
                        OwnerBankCode = swiftStatement.BankCode,
                        PartnerBic = swiftTransaction.BankCode,
                        PartnerIban = swiftTransaction.AccountCode,
                        PartnerName = swiftTransaction.PartnerName,
                        RemittanceText = swiftTransaction.Description,
                        TransactionType = swiftTransaction.Text,
                        TransactionDate = swiftTransaction.InputDate,
                        ValueDate = swiftTransaction.ValueDate
                    });
                }
            }

            return result.TypedResult(transactionList);
        }


        /// <summary>
        /// Transfer money - General method
        /// </summary>
        /// <param name="receiverName">Name of the recipient</param>
        /// <param name="receiverIBAN">IBAN of the recipient</param>
        /// <param name="receiverBIC">BIC of the recipient</param>
        /// <param name="amount">Amount to transfer</param>
        /// <param name="purpose">Short description of the transfer (dt. Verwendungszweck)</param>      
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 911 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="flickerImage">(Out) reference to an image object that shall receive the FlickerCode as GIF image</param>
        /// <param name="flickerWidth">Width of the flicker code</param>
        /// <param name="flickerHeight">Height of the flicker code</param>
        /// <param name="renderFlickerCodeAsGif">Renders flicker code as GIF, if 'true'</param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public HBCIDialogResult Transfer(TANDialog tanDialog, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, string hirms)
        {
            HBCIDialogResult result = InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!string.IsNullOrEmpty(hirms))
                HIRMS = hirms;

            string BankCode = Transaction.HKCCS(this, receiverName, receiverIBAN, receiverBIC, amount, purpose);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);

            return result;
        }

        /// <summary>
        /// Collective transfer money - General method
        /// </summary>
        /// <param name="painData"></param>
        /// <param name="numberOfTransactions"></param>
        /// <param name="totalAmount"></param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="anonymous"></param>
        /// <param name="flickerImage">(Out) reference to an image object that shall receive the FlickerCode as GIF image</param>
        /// <param name="flickerWidth">Width of the flicker code</param>
        /// <param name="flickerHeight">Height of the flicker code</param>
        /// <param name="renderFlickerCodeAsGif">Renders flicker code as GIF, if 'true'</param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public HBCIDialogResult CollectiveTransfer(TANDialog tanDialog, List<Pain00100203CtData> painData,
            string numberOfTransactions, decimal totalAmount, string hirms)
        {
            HBCIDialogResult result = InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!string.IsNullOrEmpty(hirms))
                HIRMS = hirms;

            string BankCode = Transaction.HKCCM(this, painData, numberOfTransactions, totalAmount);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);

            return result;
        }

        /// <summary>
        /// Collective transfer money terminated - General method
        /// </summary>
        /// <param name="painData"></param>
        /// <param name="numberOfTransactions"></param>
        /// <param name="totalAmount"></param>
        /// <param name="ExecutionDay"></param>
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 911 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="flickerImage">(Out) reference to an image object that shall receive the FlickerCode as GIF image</param>
        /// <param name="flickerWidth">Width of the flicker code</param>
        /// <param name="flickerHeight">Height of the flicker code</param>
        /// <param name="renderFlickerCodeAsGif">Renders flicker code as GIF, if 'true'</param> 
        /// <returns>
        /// Bank return codes
        /// </returns>
        public HBCIDialogResult CollectiveTransfer_Terminated(TANDialog tanDialog, List<Pain00100203CtData> painData,
            string numberOfTransactions, decimal totalAmount, DateTime executionDay, string hirms)
        {
            HBCIDialogResult result = InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!string.IsNullOrEmpty(hirms))
                HIRMS = hirms;

            string BankCode = Transaction.HKCME(this, painData, numberOfTransactions, totalAmount, executionDay);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);

            return result;
        }

        /// <summary>
        /// Transfer money at a certain time - General method
        /// </summary>       
        /// <param name="receiverName">Name of the recipient</param>
        /// <param name="receiverIBAN">IBAN of the recipient</param>
        /// <param name="receiverBIC">BIC of the recipient</param>
        /// <param name="amount">Amount to transfer</param>
        /// <param name="purpose">Short description of the transfer (dt. Verwendungszweck)</param>      
        /// <param name="executionDay"></param>
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 911 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="flickerImage">(Out) reference to an image object that shall receive the FlickerCode as GIF image</param>
        /// <param name="flickerWidth">Width of the flicker code</param>
        /// <param name="flickerHeight">Height of the flicker code</param>
        /// <param name="renderFlickerCodeAsGif">Renders flicker code as GIF, if 'true'</param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public HBCIDialogResult Transfer_Terminated(ConnectionDetails connectionDetails, TANDialog tanDialog, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, DateTime executionDay, string hirms, bool anonymous)
        {
            HBCIDialogResult result = InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!string.IsNullOrEmpty(hirms))
                HIRMS = hirms;

            string BankCode = Transaction.HKCSE(this, receiverName, receiverIBAN, receiverBIC, amount, purpose, executionDay);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);

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
        public HBCIDialogResult Rebooking(TANDialog tanDialog, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, string hirms)
        {
            HBCIDialogResult result = InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!string.IsNullOrEmpty(hirms))
                HIRMS = hirms;

            string BankCode = Transaction.HKCUM(this, receiverName, receiverIBAN, receiverBIC, amount, purpose);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);

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
        public HBCIDialogResult Collect(TANDialog tanDialog, string payerName, string payerIBAN, string payerBIC,
            decimal amount, string purpose, DateTime settlementDate, string mandateNumber, DateTime mandateDate, string creditorIdNumber,
            string hirms)
        {
            HBCIDialogResult result = InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!string.IsNullOrEmpty(hirms))
                HIRMS = hirms;

            string BankCode = Transaction.HKDSE(this, payerName, payerIBAN, payerBIC, amount, purpose, settlementDate, mandateNumber, mandateDate, creditorIdNumber);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);

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
        public HBCIDialogResult CollectiveCollect(TANDialog tanDialog, DateTime settlementDate, List<Pain00800202CcData> painData,
           string numberOfTransactions, decimal totalAmount, string hirms)
        {
            HBCIDialogResult result = InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!string.IsNullOrEmpty(hirms))
                HIRMS = hirms;

            string BankCode = Transaction.HKDME(this, settlementDate, painData, numberOfTransactions, totalAmount);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);

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
        public HBCIDialogResult Prepaid(TANDialog tanDialog, int mobileServiceProvider, string phoneNumber,
            int amount, string hirms)
        {
            HBCIDialogResult result = InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!string.IsNullOrEmpty(hirms))
                HIRMS = hirms;

            string BankCode = Transaction.HKPPD(this, mobileServiceProvider, phoneNumber, amount);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);

            return result;
        }

        /// <summary>
        /// Submit bankers order - General method
        /// </summary>
        /// <param name="receiverName"></param>
        /// <param name="receiverIBAN"></param>
        /// <param name="receiverBIC"></param>
        /// <param name="amount">Amount to transfer</param>
        /// <param name="purpose">Short description of the transfer (dt. Verwendungszweck)</param>      
        /// <param name="firstTimeExecutionDay"></param>
        /// <param name="timeUnit"></param>
        /// <param name="rota"></param>
        /// <param name="executionDay"></param>
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 911 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="flickerImage">(Out) reference to an image object that shall receive the FlickerCode as GIF image</param>
        /// <param name="flickerWidth">Width of the flicker code</param>
        /// <param name="flickerHeight">Height of the flicker code</param>
        /// <param name="renderFlickerCodeAsGif">Renders flicker code as GIF, if 'true'</param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public HBCIDialogResult SubmitBankersOrder(TANDialog tanDialog, string receiverName, string receiverIBAN,
           string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, TimeUnit timeUnit, string rota,
           int executionDay, DateTime? lastExecutionDay, string hirms)
        {
            HBCIDialogResult result = InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!string.IsNullOrEmpty(hirms))
                HIRMS = hirms;

            string BankCode = Transaction.HKCDE(this, receiverName, receiverIBAN, receiverBIC, amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay, lastExecutionDay);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);

            return result;
        }

        public HBCIDialogResult ModifyBankersOrder(TANDialog tanDialog, string OrderId, string receiverName, string receiverIBAN,
           string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, TimeUnit timeUnit, string rota,
           int executionDay, DateTime? lastExecutionDay, string hirms)
        {
            HBCIDialogResult result = InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!string.IsNullOrEmpty(hirms))
                HIRMS = hirms;

            string BankCode = Transaction.HKCDN(this, OrderId, receiverName, receiverIBAN, receiverBIC, amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay, lastExecutionDay);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);

            return result;
        }

        public HBCIDialogResult DeleteBankersOrder(ConnectionDetails connectionDetails, TANDialog tanDialog, string orderId, string receiverName, string receiverIBAN,
            string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota, int executionDay, DateTime? lastExecutionDay, string hirms)
        {
            HBCIDialogResult result = InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!string.IsNullOrEmpty(hirms))
                HIRMS = hirms;

            string BankCode = Transaction.HKCDL(this, orderId, receiverName, receiverIBAN, receiverBIC, amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay, lastExecutionDay);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);

            return result;
        }

        /// <summary>
        /// Get banker's orders
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC</param>         
        /// <param name="anonymous"></param>
        /// <returns>
        /// Banker's orders
        /// </returns>
        public HBCIDialogResult<List<BankersOrder>> GetBankersOrders(ConnectionDetails connectionDetails, TANDialog tanDialog)
        {
            HBCIDialogResult result = InitializeConnection();
            if (!result.IsSuccess)
                return result.TypedResult<List<BankersOrder>>();

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<List<BankersOrder>>();

            // Success
            string BankCode = Transaction.HKCDB(this);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result.TypedResult<List<BankersOrder>>();

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<List<BankersOrder>>();

            BankCode = result.RawData;
            int startIdx = BankCode.IndexOf("HICDB");
            if (startIdx < 0)
                return result.TypedResult<List<BankersOrder>>();

            List<BankersOrder> data = new List<BankersOrder>();

            string BankCode_ = BankCode.Substring(startIdx);
            for (; ; )
            {
                Match match = Regex.Match(BankCode_, @"HICDB.+?(?<xml><\?xml.+?</Document>)\+(?<orderid>.*?)\+(?<firstdate>\d*):(?<turnus>[MW]):(?<rota>\d+):(?<execday>\d+)(:(?<lastdate>\d+))?", RegexOptions.Singleline);
                if (match.Success)
                {
                    string xml = match.Groups["xml"].Value;
                    // xml ist UTF-8
                    xml = Converter.ConvertEncoding(xml, Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8);

                    string orderId = match.Groups["orderid"].Value;

                    string firstExecutionDateStr = match.Groups["firstdate"].Value;
                    DateTime? firstExecutionDate = !string.IsNullOrWhiteSpace(firstExecutionDateStr) ? DateTime.ParseExact(firstExecutionDateStr, "yyyyMMdd", CultureInfo.InvariantCulture) : default(DateTime?);

                    string timeUnitStr = match.Groups["turnus"].Value;
                    TimeUnit timeUnit = timeUnitStr == "M" ? TimeUnit.Monthly : TimeUnit.Weekly;

                    string rota = match.Groups["rota"].Value;

                    string executionDayStr = match.Groups["execday"].Value;
                    int executionDay = Convert.ToInt32(executionDayStr);

                    string lastExecutionDateStr = match.Groups["lastdate"].Value;
                    DateTime? lastExecutionDate = !string.IsNullOrWhiteSpace(lastExecutionDateStr) ? DateTime.ParseExact(lastExecutionDateStr, "yyyyMMdd", CultureInfo.InvariantCulture) : default(DateTime?);

                    Pain00100103CtData painData = Pain00100103CtData.Create(xml);

                    if (firstExecutionDate.HasValue && executionDay > 0)
                    {
                        BankersOrder item = new BankersOrder(orderId, painData, firstExecutionDate.Value, timeUnit, rota, executionDay, lastExecutionDate);
                        data.Add(item);
                    }
                }

                int endIdx = BankCode_.IndexOf("'");
                if (BankCode_.Length <= endIdx + 1)
                    break;

                BankCode_ = BankCode_.Substring(endIdx + 1);
                startIdx = BankCode_.IndexOf("HICDB");
                if (startIdx < 0)
                    break;
            }

            // Success
            return result.TypedResult(data);
        }

        /// <summary>
        /// Get terminated transfers
        /// </summary>
        /// <returns>
        /// Banker's orders
        /// </returns>
        public HBCIDialogResult GetTerminatedTransfers(TANDialog tanDialog)
        {
            HBCIDialogResult result = InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            // Success
            string BankCode = Transaction.HKCSB(this);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);

            return result;
        }

        /// <summary>
        /// Confirm order with TAN
        /// </summary>
        /// <param name="TAN"></param>
        /// <param name="MediumName"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public HBCIDialogResult TAN4(string TAN, string MediumName)
        {
            string BankCode = Transaction.TAN4(this, TAN, MediumName);
            HBCIDialogResult result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);

            return result;
        }

        /// <summary>
        /// Request tan medium name
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz</param>
        /// <returns>
        /// TAN Medium Name
        /// </returns>
        public HBCIDialogResult<List<string>> RequestTANMediumName()
        {
            HKTAN.SegmentId = "HKTAB";

            HBCIDialogResult result = InitializeConnection();
            if (!result.IsSuccess)
                return result.TypedResult<List<string>>();

            // Should not be needed when processing HKTAB
            //result = ProcessSCA(connectionDetails, result, tanDialog);
            //if (!result.IsSuccess)
            //    return result.TypedResult<List<string>>();

            string BankCode = Transaction.HKTAB(this);
            result = new HBCIDialogResult<List<string>>(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result.TypedResult<List<string>>();

            // Should not be needed when processing HKTAB
            //result = ProcessSCA(connectionDetails, result, tanDialog);
            //if (!result.IsSuccess)
            //    return result.TypedResult<List<string>>();

            BankCode = result.RawData;
            string BankCode_ = "HITAB" + Helper.Parse_String(BankCode, "'HITAB", "'");
            return result.TypedResult(Helper.Parse_TANMedium(BankCode_));
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
