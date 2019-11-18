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

using libfintx.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using static libfintx.HKCDE;

namespace libfintx
{
    public class Main
    {
        /// <summary>
        /// Resets all temporary values. Should be used when switching to another bank connection.
        /// </summary>
        public static void Reset()
        {
            Segment.Reset();
            TransactionConsole.Output = null;
        }

        /// <summary>
        /// Synchronize bank connection
        /// </summary>
        /// <param name="conn">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz</param>
        /// <returns>
        /// Customer System ID
        /// </returns>
        public static HBCIDialogResult<string> Synchronization(ConnectionDetails conn)
        {
            string BankCode = Transaction.HKSYN(conn);

            var messages = Helper.Parse_BankCode(BankCode);

            return new HBCIDialogResult<string>(messages, BankCode, Segment.HISYN);
        }

        private static HBCIDialogResult Init(ConnectionDetails conn, bool anonymous)
        {
            if (HKTAN.SegmentId == null)
                HKTAN.SegmentId = "HKIDN";

            HBCIDialogResult result;
            string BankCode;
            try
            {
                if (conn.CustomerSystemId == null)
                {
                    result = Synchronization(conn);
                    if (!result.IsSuccess)
                    {
                        Log.Write("Synchronisation failed.");
                        return result;
                    }
                }
                else
                {
                    Segment.HISYN = conn.CustomerSystemId;
                }
                BankCode = Transaction.INI(conn, anonymous);
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
        /// Account balance
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, Account, IBAN, BIC</param>
        /// <param name="Anonymous"></param>
        /// <returns>
        /// Structured information about balance, creditline and used currency
        /// </returns>
        public static HBCIDialogResult<AccountBalance> Balance(ConnectionDetails connectionDetails, TANDialog tanDialog, bool anonymous)
        {
            HBCIDialogResult result = Init(connectionDetails, anonymous);
            if (!result.IsSuccess)
                return result.TypedResult<AccountBalance>();

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<AccountBalance>();

            // Success
            var BankCode = Transaction.HKSAL(connectionDetails);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result.TypedResult<AccountBalance>();

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<AccountBalance>();

            BankCode = result.RawData;
            var balance = Helper.Parse_Balance(BankCode);
            return result.TypedResult(balance);
        }

        public static HBCIDialogResult<List<AccountInformations>> Accounts(ConnectionDetails connectionDetails, TANDialog tanDialog, bool anonymous)
        {
            HBCIDialogResult result = Init(connectionDetails, anonymous);
            if (!result.IsSuccess)
                return result.TypedResult<List<AccountInformations>>();

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<List<AccountInformations>>();

            return new HBCIDialogResult<List<AccountInformations>>(result.Messages, UPD.Value, UPD.HIUPD.AccountList);
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
        public static HBCIDialogResult<List<SWIFTStatement>> Transactions(ConnectionDetails connectionDetails, TANDialog tanDialog, bool anonymous, DateTime? startDate = null, DateTime? endDate = null, bool saveMt940File = false)
        {
            HBCIDialogResult result = Init(connectionDetails, anonymous);
            if (!result.IsSuccess)
                return result.TypedResult<List<SWIFTStatement>>();

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<List<SWIFTStatement>>();

            var startDateStr = startDate?.ToString("yyyyMMdd");
            var endDateStr = endDate?.ToString("yyyyMMdd");

            // Success
            var BankCode = Transaction.HKKAZ(connectionDetails, startDateStr, endDateStr, null);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result.TypedResult<List<SWIFTStatement>>();

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<List<SWIFTStatement>>();

            BankCode = result.RawData;
            StringBuilder Transactions = new StringBuilder();

            var regex = new Regex(@"HIKAZ:.+?@\d+@(?<payload>.+?)('HNSHA|''HNHBS)", RegexOptions.Singleline);
            var match = regex.Match(BankCode);
            if (match.Success)
                Transactions.Append(match.Groups["payload"].Value);

            string BankCode_ = BankCode;
            while (BankCode_.Contains("+3040::"))
            {
                Helper.Parse_Message(BankCode_);

                var Startpoint = new Regex(@"\+3040::[^:]+:(?<startpoint>[^']+)'").Match(BankCode_).Groups["startpoint"].Value;

                BankCode_ = Transaction.HKKAZ(connectionDetails, startDateStr, endDateStr, Startpoint);
                result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode_), BankCode_);
                if (!result.IsSuccess)
                    return result.TypedResult<List<SWIFTStatement>>();

                result = ProcessSCA(connectionDetails, result, tanDialog);
                if (!result.IsSuccess)
                    return result.TypedResult<List<SWIFTStatement>>();

                BankCode_ = result.RawData;
                match = regex.Match(BankCode_);
                if (match.Success)
                    Transactions.Append(match.Groups["payload"].Value);
            }

            var swiftStatements = new List<SWIFTStatement>();

            swiftStatements.AddRange(MT940.Serialize(Transactions.ToString(), connectionDetails.Account, saveMt940File));

            return result.TypedResult(swiftStatements);
        }

        /// <summary>
        /// Account transactions in camt format
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, Account, IBAN, BIC</param>  
        /// <param name="anonymous"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>
        /// Transactions
        /// </returns>
        public static HBCIDialogResult<List<TStatement>> Transactions_camt(ConnectionDetails connectionDetails, TANDialog tanDialog, bool anonymous, camtVersion camtVers,
            DateTime? startDate = null, DateTime? endDate = null, bool saveCamtFile = false)
        {
            HBCIDialogResult result = Init(connectionDetails, anonymous);
            if (!result.IsSuccess)
                return result.TypedResult<List<TStatement>>();

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<List<TStatement>>();

            // Plain camt message
            var camt = string.Empty;

            var startDateStr = startDate?.ToString("yyyyMMdd");
            var endDateStr = endDate?.ToString("yyyyMMdd");

            // Success
            var BankCode = Transaction.HKCAZ(connectionDetails, startDateStr, endDateStr, null, camtVers);
            result = new HBCIDialogResult<List<TStatement>>(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result.TypedResult<List<TStatement>>();

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<List<TStatement>>();

            BankCode = result.RawData;
            List<TStatement> statements = new List<TStatement>();

            TCAM052TParser CAMT052Parser = null;
            TCAM053TParser CAMT053Parser = null;
            Encoding encoding = Encoding.GetEncoding("ISO-8859-1");

            string BankCode_ = BankCode;

            // Es kann sein, dass in der payload mehrere Dokumente enthalten sind
            var xmlStartIdx = BankCode_.IndexOf("<?xml version=");
            var xmlEndIdx = BankCode_.IndexOf("</Document>") + "</Document>".Length;
            while (xmlStartIdx >= 0)
            {
                if (xmlStartIdx > xmlEndIdx)
                    break;

                camt = "<?xml version=" + Helper.Parse_String(BankCode_, "<?xml version=", "</Document>") + "</Document>";

                switch (camtVers)
                {
                    case camtVersion.camt052:
                        if (CAMT052Parser == null)
                            CAMT052Parser = new TCAM052TParser();

                        if (saveCamtFile)
                        {
                            // Save camt052 statement to file
                            var camt052f = camt052File.Save(connectionDetails.Account, camt, encoding);
                            
                            // Process the camt052 file
                            CAMT052Parser.ProcessFile(camt052f);
                        }
                        else
                        {
                            CAMT052Parser.ProcessDocument(camt, encoding);
                        }

                        statements.AddRange(CAMT052Parser.statements);
                        break;
                    case camtVersion.camt053:
                        if (CAMT053Parser == null)
                            CAMT053Parser = new TCAM053TParser();

                        if (saveCamtFile)
                        {
                            // Save camt053 statement to file
                            var camt053f = camt053File.Save(connectionDetails.Account, camt, encoding);

                            // Process the camt053 file
                            CAMT053Parser.ProcessFile(camt053f);
                        }
                        else
                        {
                            CAMT053Parser.ProcessDocument(camt, encoding);
                        }

                        statements.AddRange(CAMT053Parser.statements);
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
                BankCode_ = Transaction.HKCAZ(connectionDetails, startDateStr, endDateStr, Startpoint, camtVers);
                result = new HBCIDialogResult<List<TStatement>>(Helper.Parse_BankCode(BankCode_), BankCode_);
                if (!result.IsSuccess)
                    return result.TypedResult<List<TStatement>>();

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
                        case camtVersion.camt052:
                            // Save camt052 statement to file
                            var camt052f_ = camt052File.Save(connectionDetails.Account, camt);

                            // Process the camt052 file
                            CAMT052Parser.ProcessFile(camt052f_);

                            // Add all items
                            statements.AddRange(CAMT052Parser.statements);
                            break;
                        case camtVersion.camt053:
                            // Save camt053 statement to file
                            var camt053f_ = camt053File.Save(connectionDetails.Account, camt);

                            // Process the camt053 file
                            CAMT053Parser.ProcessFile(camt053f_);

                            // Add all items to existing statement
                            statements.AddRange(CAMT053Parser.statements);
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
        public static HBCIDialogResult<List<AccountTransaction>> TransactionsSimple(ConnectionDetails connectionDetails, TANDialog tanDialog, bool anonymous, DateTime? startDate = null, DateTime? endDate = null)
        {
            var result = Transactions(connectionDetails, tanDialog, anonymous, startDate, endDate);
            if (!result.IsSuccess)
                return result.TypedResult<List<AccountTransaction>>();

            var transactionList = new List<AccountTransaction>();
            foreach (var swiftStatement in result.Data)
            {
                foreach (var swiftTransaction in swiftStatement.SWIFTTransactions)
                {
                    transactionList.Add(new AccountTransaction()
                    {
                        OwnerAccount = swiftStatement.accountCode,
                        OwnerBankcode = swiftStatement.bankCode,
                        PartnerBIC = swiftTransaction.bankCode,
                        PartnerIBAN = swiftTransaction.accountCode,
                        PartnerName = swiftTransaction.partnerName,
                        RemittanceText = swiftTransaction.description,
                        TransactionType = swiftTransaction.text,
                        TransactionDate = swiftTransaction.inputDate,
                        ValueDate = swiftTransaction.valueDate
                    });
                }
            }

            return result.TypedResult(transactionList);
        }

        /// <summary>
        /// Transfer money - General method
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>  
        /// <param name="receiverName">Name of the recipient</param>
        /// <param name="receiverIBAN">IBAN of the recipient</param>
        /// <param name="receiverBIC">BIC of the recipient</param>
        /// <param name="amount">Amount to transfer</param>
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

        public static HBCIDialogResult Transfer(ConnectionDetails connectionDetails, TANDialog tanDialog, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, string HIRMS, bool anonymous)
        {
            HBCIDialogResult result = Init(connectionDetails, anonymous);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!String.IsNullOrEmpty(HIRMS))
                Segment.HIRMS = HIRMS;

            var BankCode = Transaction.HKCCS(connectionDetails, receiverName, receiverIBAN, receiverBIC, amount, purpose);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);

            return result;
        }

        /// <summary>
        /// Transfer money at a certain time - General method
        /// </summary>       
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>  
        /// <param name="receiverName">Name of the recipient</param>
        /// <param name="receiverIBAN">IBAN of the recipient</param>
        /// <param name="receiverBIC">BIC of the recipient</param>
        /// <param name="amount">Amount to transfer</param>
        /// <param name="purpose">Short description of the transfer (dt. Verwendungszweck)</param>      
        /// <param name="executionDay"></param>
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

        public static HBCIDialogResult Transfer_Terminated(ConnectionDetails connectionDetails, TANDialog tanDialog, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, DateTime executionDay, string HIRMS, bool anonymous)
        {
            var result = Init(connectionDetails, anonymous);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!String.IsNullOrEmpty(HIRMS))
                Segment.HIRMS = HIRMS;

            var BankCode = Transaction.HKCSE(connectionDetails, receiverName, receiverIBAN, receiverBIC, amount, purpose, executionDay);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);

            return result;
        }

        /// <summary>
        /// Collective transfer money - General method
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>  
        /// <param name="painData"></param>
        /// <param name="numberOfTransactions"></param>
        /// <param name="totalAmount"></param>
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

        public static HBCIDialogResult CollectiveTransfer(ConnectionDetails connectionDetails, TANDialog tanDialog, List<pain00100203_ct_data> painData,
            string numberOfTransactions, decimal totalAmount, string HIRMS, bool anonymous)
        {
            var result = Init(connectionDetails, anonymous);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!String.IsNullOrEmpty(HIRMS))
                Segment.HIRMS = HIRMS;

            var BankCode = Transaction.HKCCM(connectionDetails, painData, numberOfTransactions, totalAmount);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);

            return result;
        }

        /// <summary>
        /// Collective transfer money terminated - General method
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>  
        /// <param name="painData"></param>
        /// <param name="numberOfTransactions"></param>
        /// <param name="totalAmount"></param>
        /// <param name="ExecutionDay"></param>
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

        public static HBCIDialogResult CollectiveTransfer_Terminated(ConnectionDetails connectionDetails, TANDialog tanDialog, List<pain00100203_ct_data> painData,
            string numberOfTransactions, decimal totalAmount, DateTime executionDay, string HIRMS, bool anonymous)
        {
            var result = Init(connectionDetails, anonymous);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!String.IsNullOrEmpty(HIRMS))
                Segment.HIRMS = HIRMS;

            var BankCode = Transaction.HKCME(connectionDetails, painData, numberOfTransactions, totalAmount, executionDay);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);

            return result;
        }

        /// <summary>
        /// Rebook money from one to another account - General method
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>  
        /// <param name="receiverName">Name of the recipient</param>
        /// <param name="receiverIBAN">IBAN of the recipient</param>
        /// <param name="receiverBIC">BIC of the recipient</param>
        /// <param name="amount">Amount to transfer</param>
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

        public static HBCIDialogResult Rebooking(ConnectionDetails connectionDetails, TANDialog tanDialog, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, string HIRMS, bool anonymous)
        {
            var result = Init(connectionDetails, anonymous);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!String.IsNullOrEmpty(HIRMS))
                Segment.HIRMS = HIRMS;

            var BankCode = Transaction.HKCUM(connectionDetails, receiverName, receiverIBAN, receiverBIC, amount, purpose);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);

            return result;
        }

        /// <summary>
        /// Collect money from another account - General method
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>  
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
        /// <param name="anonymous"></param>
        /// <param name="flickerImage">(Out) reference to an image object that shall receive the FlickerCode as GIF image</param>
        /// <param name="flickerWidth">Width of the flicker code</param>
        /// <param name="flickerHeight">Height of the flicker code</param>
        /// <param name="renderFlickerCodeAsGif">Renders flicker code as GIF, if 'true'</param>
        /// <returns>
        /// Bank return codes
        /// </returns>

        public static HBCIDialogResult Collect(ConnectionDetails connectionDetails, TANDialog tanDialog, string payerName, string payerIBAN, string payerBIC,
            decimal amount, string purpose, DateTime settlementDate, string mandateNumber, DateTime mandateDate, string creditorIdNumber,
            string HIRMS, bool anonymous)
        {
            var result = Init(connectionDetails, anonymous);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!String.IsNullOrEmpty(HIRMS))
                Segment.HIRMS = HIRMS;

            var BankCode = Transaction.HKDSE(connectionDetails, payerName, payerIBAN, payerBIC, amount, purpose, settlementDate, mandateNumber, mandateDate, creditorIdNumber);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);

            return result;
        }

        /// <summary>
        /// Collective collect money from other accounts - General method
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>  
        /// <param name="settlementDate"></param>
        /// <param name="painData"></param>
        /// <param name="numberOfTransactions"></param>
        /// <param name="totalAmount"></param>        
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

        public static HBCIDialogResult CollectiveCollect(ConnectionDetails connectionDetails, TANDialog tanDialog, DateTime settlementDate, List<pain00800202_cc_data> painData,
           string numberOfTransactions, decimal totalAmount, string HIRMS, bool anonymous)
        {
            var result = Init(connectionDetails, anonymous);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!String.IsNullOrEmpty(HIRMS))
                Segment.HIRMS = HIRMS;

            var BankCode = Transaction.HKDME(connectionDetails, settlementDate, painData, numberOfTransactions, totalAmount);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);

            return result;
        }

        /// <summary>
        /// Load mobile phone prepaid card - General method
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC</param>  
        /// <param name="mobileServiceProvider"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="amount">Amount to transfer</param>            
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

        public static HBCIDialogResult Prepaid(ConnectionDetails connectionDetails, TANDialog tanDialog, int mobileServiceProvider, string phoneNumber,
            int amount, string HIRMS, bool anonymous)
        {
            var result = Init(connectionDetails, anonymous);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!String.IsNullOrEmpty(HIRMS))
                Segment.HIRMS = HIRMS;

            var BankCode = Transaction.HKPPD(connectionDetails, mobileServiceProvider, phoneNumber, amount);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);

            return result;
        }

        /// <summary>
        /// Submit bankers order - General method
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>       
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
        /// <param name="anonymous"></param>
        /// <param name="flickerImage">(Out) reference to an image object that shall receive the FlickerCode as GIF image</param>
        /// <param name="flickerWidth">Width of the flicker code</param>
        /// <param name="flickerHeight">Height of the flicker code</param>
        /// <param name="renderFlickerCodeAsGif">Renders flicker code as GIF, if 'true'</param>
        /// <returns>
        /// Bank return codes
        /// </returns>

        public static HBCIDialogResult SubmitBankersOrder(ConnectionDetails connectionDetails, TANDialog tanDialog, string receiverName, string receiverIBAN,
           string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, TimeUnit timeUnit, string rota,
           int executionDay, string HIRMS, bool anonymous)
        {
            var result = Init(connectionDetails, anonymous);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!String.IsNullOrEmpty(HIRMS))
                Segment.HIRMS = HIRMS;

            var BankCode = Transaction.HKCDE(connectionDetails, receiverName, receiverIBAN, receiverBIC, amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);

            return result;
        }

        public static HBCIDialogResult ModifyBankersOrder(ConnectionDetails connectionDetails, TANDialog tanDialog, string OrderId, string receiverName, string receiverIBAN,
           string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, TimeUnit timeUnit, string rota,
           int executionDay, string HIRMS, bool anonymous)
        {
            var result = Init(connectionDetails, anonymous);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!String.IsNullOrEmpty(HIRMS))
                Segment.HIRMS = HIRMS;

            var BankCode = Transaction.HKCDN(connectionDetails, OrderId, receiverName, receiverIBAN, receiverBIC, amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);

            return result;
        }

        public static HBCIDialogResult DeleteBankersOrder(ConnectionDetails connectionDetails, TANDialog tanDialog, string orderId, string receiverName, string receiverIBAN,
            string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota, int executionDay, string HIRMS,
            bool anonymous)
        {
            var result = Init(connectionDetails, anonymous);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!String.IsNullOrEmpty(HIRMS))
                Segment.HIRMS = HIRMS;

            var BankCode = Transaction.HKCDL(connectionDetails, orderId, receiverName, receiverIBAN, receiverBIC, amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);

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
        public static HBCIDialogResult<List<BankersOrder>> GetBankersOrders(ConnectionDetails connectionDetails, TANDialog tanDialog, bool anonymous)
        {
            var result = Init(connectionDetails, anonymous);
            if (!result.IsSuccess)
                return result.TypedResult<List<BankersOrder>>();

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<List<BankersOrder>>();

            // Success
            var BankCode = Transaction.HKCDB(connectionDetails);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result.TypedResult<List<BankersOrder>>();

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<List<BankersOrder>>();

            BankCode = result.RawData;
            var startIdx = BankCode.IndexOf("HICDB");
            if (startIdx < 0)
                return result.TypedResult<List<BankersOrder>>();

            List<BankersOrder> data = new List<BankersOrder>();

            var BankCode_ = BankCode.Substring(startIdx);
            for (; ; )
            {
                var match = Regex.Match(BankCode_, @"HICDB.+?(<\?xml.+?</Document>)\+(.*?)\+(\d*):([MW]):(\d+):(\d+)", RegexOptions.Singleline);
                if (match.Success)
                {
                    var xml = match.Groups[1].Value;
                    // xml ist UTF-8
                    xml = Converter.ConvertEncoding(xml, Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8);

                    var orderId = match.Groups[2].Value;

                    var firstExecutionDateStr = match.Groups[3].Value;
                    DateTime? firstExecutionDate = !string.IsNullOrWhiteSpace(firstExecutionDateStr) ? DateTime.ParseExact(firstExecutionDateStr, "yyyyMMdd", CultureInfo.InvariantCulture) : default(DateTime?);

                    var timeUnitStr = match.Groups[4].Value;
                    TimeUnit timeUnit = timeUnitStr == "M" ? TimeUnit.Monthly : TimeUnit.Weekly;

                    var rota = match.Groups[5].Value;

                    var executionDayStr = match.Groups[6].Value;
                    int executionDay = Convert.ToInt32(executionDayStr);

                    var painData = pain00100103_ct_data.Create(xml);

                    if (firstExecutionDate.HasValue && executionDay > 0)
                    {
                        var item = new BankersOrder(orderId, painData, firstExecutionDate.Value, timeUnit, rota, executionDay);
                        data.Add(item);
                    }
                }

                var endIdx = BankCode_.IndexOf("'");
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
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC</param>         
        /// <param name="anonymous"></param>
        /// <returns>
        /// Banker's orders
        /// </returns>
        public static HBCIDialogResult GetTerminatedTransfers(ConnectionDetails connectionDetails, TANDialog tanDialog, bool anonymous)
        {
            HBCIDialogResult result = Init(connectionDetails, anonymous);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);
            if (!result.IsSuccess)
                return result;

            // Success
            var BankCode = Transaction.HKCSB(connectionDetails);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(connectionDetails, result, tanDialog);

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
        public static HBCIDialogResult TAN(ConnectionDetails connectionDetails, string TAN)
        {
            var BankCode = Transaction.TAN(connectionDetails, TAN);
            var result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);

            return result;
        }

        /// <summary>
        /// Confirm order with TAN
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz</param>
        /// <param name="TAN"></param>
        /// <param name="MediumName"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static HBCIDialogResult TAN4(ConnectionDetails connectionDetails, string TAN, string MediumName)
        {
            var BankCode = Transaction.TAN4(connectionDetails, TAN, MediumName);
            var result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);

            return result;
        }

        /// <summary>
        /// Request tan medium name
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz</param>
        /// <returns>
        /// TAN Medium Name
        /// </returns>
        public static HBCIDialogResult<List<string>> RequestTANMediumName(ConnectionDetails connectionDetails)
        {
            HKTAN.SegmentId = "HKTAB";

            HBCIDialogResult result = Init(connectionDetails, false);
            if (!result.IsSuccess)
                return result.TypedResult<List<string>>();

            // Should not be needed when processing HKTAB
            //result = ProcessSCA(connectionDetails, result, tanDialog);
            //if (!result.IsSuccess)
            //    return result.TypedResult<List<string>>();

            var BankCode = Transaction.HKTAB(connectionDetails);
            result = new HBCIDialogResult<List<string>>(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result.TypedResult<List<string>>();

            // Should not be needed when processing HKTAB
            //result = ProcessSCA(connectionDetails, result, tanDialog);
            //if (!result.IsSuccess)
            //    return result.TypedResult<List<string>>();

            BankCode = result.RawData;
            var BankCode_ = "HITAB" + Helper.Parse_String(BankCode, "'HITAB", "'");
            return result.TypedResult(Helper.Parse_TANMedium(BankCode_));
        }

        /// <summary>
        /// TAN scheme
        /// </summary>
        /// <returns>
        /// TAN mechanism
        /// </returns>
        public static string TAN_Scheme()
        {
            return Segment.HIRMSf;
        }

        /// <summary>
        /// Set assembly information
        /// </summary>
        /// <param name="Buildname"></param>
        /// <param name="Version"></param>
        public static void Assembly(string Buildname, string Version)
        {
            Program.Buildname = Buildname;
            Program.Version = Version;

            Log.Write(Buildname);
            Log.Write(Version);
        }


        /// <summary>
        /// Set assembly information automatically
        /// </summary>
        public static void Assembly()
        {
            var assemInfo = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            Program.Buildname = assemInfo.Name;
            Program.Version = $"{assemInfo.Version.Major}.{assemInfo.Version.Minor}";

            Log.Write(Program.Buildname);
            Log.Write(Program.Version);
        }


        /// <summary>
        /// Get assembly buildname
        /// </summary>
        /// <returns>
        /// Buildname
        /// </returns>
        public static string Buildname()
        {
            return Program.Buildname;
        }

        /// <summary>
        /// Get assembly version
        /// </summary>
        /// <returns>
        /// Version
        /// </returns>
        public static string Version()
        {
            return Program.Version;
        }

        /// <summary>
        /// Transactions output console
        /// </summary>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string Transaction_Output()
        {
            return TransactionConsole.Output;
        }

        /// <summary>
        /// Enable / Disable Tracing
        /// </summary>
        public static void Tracing(bool Enabled, bool Formatted = false, int maxFileSizeMB = 10)
        {
            Trace.Enabled = Enabled;
            Trace.Formatted = Formatted;
            Trace.MaxFileSize = maxFileSizeMB;
        }

        /// <summary>
        /// Enable / Disable Debugging
        /// </summary>
        public static void Debugging(bool Enabled)
        {
            DEBUG.Enabled = Enabled;
        }

        /// <summary>
        /// Enable / Disable Logging
        /// </summary>
        public static void Logging(bool Enabled, int maxFileSizeMB = 10)
        {
            Log.Enabled = Enabled;
            Log.MaxFileSize = maxFileSizeMB;
        }

        private static HBCIDialogResult ProcessSCA(ConnectionDetails conn, HBCIDialogResult result, TANDialog tanDialog)
        {
            tanDialog.DialogResult = result;
            if (result.IsSCARequired)
            {
                var tan = Helper.WaitForTAN(result, tanDialog);
                if (tan == null)
                {
                    var BankCode = Transaction.HKEND(conn, Segment.HNHBK);
                    result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
                }
                else
                {
                    result = TAN(conn, tan);
                }
            }

            return result;
        }
    }
}