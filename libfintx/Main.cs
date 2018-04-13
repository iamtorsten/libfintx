/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2017 Torsten Klinger
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
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace libfintx
{
    public class Main
    {
        /// <summary>
        /// Synchronize bank connection
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz</param>
        /// <param name="anonymous"></param>
        /// <returns>
        /// Success or failure
        /// </returns>
        public static bool Synchronization(ConnectionDetails connectionDetails, bool anonymous)
        {
            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                return true;
            }
            else
                return false;
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
        public static bool Synchronization_RDH(int BLZ, string URL, int Port, int HBCIVersion, string UserID, string FilePath, string Password)
        {
            if (Transaction.INI_RDH(BLZ, URL, Port, HBCIVersion, UserID, FilePath, Password) == true)
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Account balance
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, Account, IBAN, BIC</param>
        /// <param name="Anonymous"></param>
        /// <returns>
        /// Structured information about balance, creditline and used currency
        /// </returns>
        public static AccountBalance Balance(ConnectionDetails connectionDetails, bool anonymous)
        {            
            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                // Success
                var BankCode = Transaction.HKSAL(connectionDetails);
                return Helper.Parse_Balance(BankCode);
            }
            else {
                Log.Write("Error in initialization.");
                throw new Exception("Error in initialization.");
            }
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
        public static List<SWIFTStatement> Transactions(ConnectionDetails connectionDetails, bool anonymous, DateTime? startDate = null, DateTime? endDate = null)
        {
            var swiftStatements = new List<SWIFTStatement>();

            if (Transaction.INI(connectionDetails, anonymous) == true)
            {

                var startDateStr = startDate?.ToString("yyyyMMdd");
                var endDateStr = endDate?.ToString("yyyyMMdd");

                // Success
                var BankCode = Transaction.HKKAZ(connectionDetails, startDateStr, endDateStr, null);

                var Transactions = ":20:STARTUMS" + Helper.Parse_String(BankCode, ":20:STARTUMS", "'HNSHA");

                swiftStatements.AddRange(MT940.Serialize(Transactions, connectionDetails.Account));


                string BankCode_ = BankCode;
                while (BankCode_.Contains("+3040::"))
                {
                    Helper.Parse_Message(BankCode_);

                    var Startpoint = new Regex(@"\+3040::[^:]+:(?<startpoint>[^']+)'").Match(BankCode_).Groups["startpoint"].Value;

                    BankCode_ = Transaction.HKKAZ(connectionDetails, startDateStr, endDateStr, Startpoint);

                    var Transactions_ = ":20:STARTUMS" + Helper.Parse_String(BankCode_, ":20:STARTUMS", "'HNSHA");

                    swiftStatements.AddRange(MT940.Serialize(Transactions_, connectionDetails.Account));
                }
                return swiftStatements;
            }
            else
            {
                Log.Write("Initialization/sync failed");
                throw new Exception("Initialization/sync failed");
            }                
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
        public static List<AccountTransaction> TransactionsSimple(ConnectionDetails connectionDetails, bool anonymous, DateTime? startDate = null, DateTime? endDate = null)
        {
            var transactionList = new List<AccountTransaction>();

            foreach (var swiftStatement in Transactions(connectionDetails, anonymous, startDate, endDate))
            {
                foreach (var swiftTransaction in swiftStatement.SWIFTTransactions)
                {
                    transactionList.Add(new AccountTransaction()
                    {
                         OwnerAccount = swiftStatement.accountCode,
                         OwnerBankcode= swiftStatement.bankCode,
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
            return transactionList;
        }

        /// <summary>
        /// Transfer money - render FlickerCode in WinForms
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
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string Transfer(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, string HIRMS, PictureBox pictureBox, bool anonymous)
        {
            Image img = null;
            return Transfer(connectionDetails, receiverName, receiverIBAN, receiverBIC,
            amount, purpose, HIRMS, pictureBox, anonymous, out img);
        }

        /// <summary>
        /// Transfer money - render FlickerCode as Gif
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>  
        /// <param name="receiverName">Name of the recipient</param>
        /// <param name="receiverIBAN">IBAN of the recipient</param>
        /// <param name="receiverBIC">BIC of the recipient</param>
        /// <param name="amount">Amount to transfer</param>
        /// <param name="purpose">Short description of the transfer (dt. Verwendungszweck)</param>      
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 911 for "Sparkasse chipTan optisch"</param>        
        /// <param name="anonymous"></param>
        /// <param name="flickerImage">(Out) reference to an image object that shall receive the FlickerCode as GIF image</param>
        /// <param name="flickerWidth">Width of the flicker code</param>
        /// <param name="flickerHeight">Height of the flicker code</param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string Transfer(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, string HIRMS, bool anonymous, out Image flickerImage, int flickerWidth, int flickerHeight)
        {
            return Transfer(connectionDetails, receiverName, receiverIBAN, receiverBIC,
            amount, purpose, HIRMS, null, anonymous, out flickerImage, flickerWidth, flickerHeight, true);
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
        public static string Transfer(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, string HIRMS, PictureBox pictureBox, bool anonymous, out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120, bool renderFlickerCodeAsGif = false)
        {
            flickerImage = null;
            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCCS(connectionDetails, receiverName, receiverIBAN, receiverBIC, amount, purpose);

                if (BankCode.Contains("+0030::"))
                {
                    // Gif image instead of picture box
                    Helper.Parse_BankCode(BankCode, pictureBox, flickerImage, flickerWidth, flickerHeight, renderFlickerCodeAsGif);
                    
                    return "OK";
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);
                    
                    Log.Write(msg);

                    return msg;
                }
            }
            else
                return "Error";
        }

        /// <summary>
        /// Transfer money at a certain time
        /// </summary>       
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>  
        /// <param name="receiverName">Name of the recipient</param>
        /// <param name="receiverIBAN">IBAN of the recipient</param>
        /// <param name="receiverBIC">BIC of the recipient</param>
        /// <param name="amount">Amount to transfer</param>
        /// <param name="purpose">Short description of the transfer (dt. Verwendungszweck)</param>      
        /// <param name="executionDay"></param>
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 972 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string Transfer_Terminated(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, DateTime executionDay, string HIRMS, PictureBox pictureBox, bool anonymous)
        {
            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCCST(connectionDetails, receiverName, receiverIBAN, receiverBIC, amount, purpose, executionDay);

                if (BankCode.Contains("+0030::"))
                {
                    Helper.Parse_BankCode(BankCode, pictureBox);

                    return "OK";
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);

                    Log.Write(msg);

                    return msg;
                }
            }
            else
                return "Error";
        }

        /// <summary>
        /// Collective transfer money
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>  
        /// <param name="painData"></param>
        /// <param name="numberOfTransactions"></param>
        /// <param name="totalAmount"></param>
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 972 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string CollectiveTransfer(ConnectionDetails connectionDetails, List<pain00100203_ct_data> painData, string numberOfTransactions, decimal totalAmount, string HIRMS, PictureBox pictureBox, bool anonymous)
        {
            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCCM(connectionDetails, painData, numberOfTransactions, totalAmount);

                if (BankCode.Contains("+0030::"))
                {
                    Helper.Parse_BankCode(BankCode, pictureBox);

                    return "OK";
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);

                    Log.Write(msg);

                    return msg;
                }
            }
            else
                return "Error";
        }

        /// <summary>
        /// Collective transfer money terminated
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>  
        /// <param name="painData"></param>
        /// <param name="numberOfTransactions"></param>
        /// <param name="totalAmount"></param>
        /// <param name="ExecutionDay"></param>
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 972 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string CollectiveTransfer_Terminated(ConnectionDetails connectionDetails, List<pain00100203_ct_data> painData,
            string numberOfTransactions, decimal totalAmount, DateTime executionDay, string HIRMS, PictureBox pictureBox, bool anonymous)
        {
            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCME(connectionDetails, painData, numberOfTransactions, totalAmount, executionDay);

                if (BankCode.Contains("+0030::"))
                {
                    Helper.Parse_BankCode(BankCode, pictureBox);

                    return "OK";
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);

                    Log.Write(msg);

                    return msg;
                }
            }
            else
                return "Error";
        }

        /// <summary>
        /// Rebook money from one to another account
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>  
        /// <param name="receiverName">Name of the recipient</param>
        /// <param name="receiverIBAN">IBAN of the recipient</param>
        /// <param name="receiverBIC">BIC of the recipient</param>
        /// <param name="amount">Amount to transfer</param>
        /// <param name="purpose">Short description of the transfer (dt. Verwendungszweck)</param>      
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 972 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string Rebooking(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose, string HIRMS, PictureBox pictureBox, bool anonymous)
        {
            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCUM(connectionDetails, receiverName, receiverIBAN, receiverBIC, amount, purpose);

                if (BankCode.Contains("+0030::"))
                {
                    Helper.Parse_BankCode(BankCode, pictureBox);

                    return "OK";
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);

                    Log.Write(msg);

                    return msg;
                }
            }
            else
                return "Error";
        }

        /// <summary>
        /// Collect money from another account
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
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 972 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string Collect(ConnectionDetails connectionDetails, string payerName, string payerIBAN, string payerBIC, decimal amount, string purpose, DateTime settlementDate, 
                                     string mandateNumber, DateTime mandateDate, string creditorIdNumber, string HIRMS, PictureBox pictureBox, bool anonymous)
        {
            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKDSE(connectionDetails, payerName, payerIBAN, payerBIC, amount, purpose, settlementDate, mandateNumber, mandateDate, creditorIdNumber);

                if (BankCode.Contains("+0030::"))
                {
                    Helper.Parse_BankCode(BankCode, pictureBox);

                    return "OK";
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);

                    Log.Write(msg);

                    return msg;
                }
            }
            else
                return "Error";
        }

        /// <summary>
        /// Collective collect money from other accounts
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>  
        /// <param name="settlementDate"></param>
        /// <param name="painData"></param>
        /// <param name="numberOfTransactions"></param>
        /// <param name="totalAmount"></param>        
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 972 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string CollectiveCollect(ConnectionDetails connectionDetails, DateTime settlementDate, List<pain00800202_cc_data> painData,
            string numberOfTransactions, decimal totalAmount, string HIRMS, PictureBox pictureBox, bool anonymous)
        {
            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKDME(connectionDetails, settlementDate, painData, numberOfTransactions, totalAmount);

                if (BankCode.Contains("+0030::"))
                {
                    Helper.Parse_BankCode(BankCode, pictureBox);

                    return "OK";
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);

                    Log.Write(msg);

                    return msg;
                }
            }
            else
                return "Error";
        }

        /// <summary>
        /// Load mobile phone prepaid card
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC</param>  
        /// <param name="mobileServiceProvider"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="amount">Amount to transfer</param>            
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 972 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string Prepaid(ConnectionDetails connectionDetails, int mobileServiceProvider, string phoneNumber, int amount, string HIRMS, PictureBox pictureBox, bool anonymous)
        {
            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKPPD(connectionDetails, mobileServiceProvider, phoneNumber, amount);

                if (BankCode.Contains("+0030::"))
                {
                    Helper.Parse_BankCode(BankCode, pictureBox);

                    return "OK";
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);

                    Log.Write(msg);

                    return msg;
                }
            }
            else
                return "Error";
        }

        /// <summary>
        /// Submit bankers order
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>  
        /// <param name="mobileServiceProvider"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="amount"></param>      
        /// <param name="receiverName"></param>
        /// <param name="receiverIBAN"></param>
        /// <param name="receiverBIC"></param>
        /// <param name="amount">Amount to transfer</param>
        /// <param name="purpose">Short description of the transfer (dt. Verwendungszweck)</param>      
        /// <param name="firstTimeExecutionDay"></param>
        /// <param name="timeUnit"></param>
        /// <param name="rota"></param>
        /// <param name="executionDay"></param>
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 972 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="anonymous"></param>        
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string SubmitBankersOrder(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay,
                                                HKCDE.TimeUnit timeUnit, string rota, int executionDay, string HIRMS, PictureBox pictureBox, bool anonymous)
        {
            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCDE(connectionDetails, receiverName, receiverIBAN, receiverBIC, amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay);

                if (BankCode.Contains("+0030::"))
                {
                    Helper.Parse_BankCode(BankCode, pictureBox);

                    return "OK";
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);

                    Log.Write(msg);

                    return msg;
                }
            }
            else
                return "Error";
        }

        /// <summary>
        /// Get banker's orders
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC</param>         
        /// <param name="anonymous"></param>
        /// <returns>
        /// Banker's orders
        /// </returns>
        public static string GetBankersOrders(ConnectionDetails connectionDetails, bool anonymous)
        {
            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                // Success
                var BankCode = Transaction.HKCSB(connectionDetails);

                if (BankCode.Contains("+0020::"))
                {
                    // Success
                    return BankCode;
                }
                else
                {
                    // Error
                    var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

                    String[] values = BankCode_.Split('+');

                    string msg = string.Empty;

                    foreach (var item in values)
                    {
                        if (!item.StartsWith("HIRMS"))
                            msg = msg + "??" + item.Replace("::", ": ");
                    }

                    Log.Write(msg);

                    return msg;
                }
            }
            else
                return "Error";
        }

        /// <summary>
        /// Confirm order with TAN
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz</param>
        /// <param name="TAN"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string TAN(ConnectionDetails connectionDetails, string TAN)
        {
            var BankCode = Transaction.TAN(connectionDetails, TAN);

            if (BankCode.Contains("+0020::"))
            {
                var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

                String[] values = BankCode_.Split('+');

                string msg = string.Empty;

                foreach (var item in values)
                {
                    if (!item.StartsWith("HIRMS"))
                        msg = msg + "??" + item.Replace("::", ": ");
                }

                return msg;
            }
            else
            {
                // Error
                var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

                String[] values = BankCode_.Split('+');

                string msg = string.Empty;

                foreach (var item in values)
                {
                    if (!item.StartsWith("HIRMS"))
                        msg = msg + "??" + item.Replace("::", ": ");
                }

                Log.Write(msg);

                return msg;
            }
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
        public static string TAN4(ConnectionDetails connectionDetails, string TAN, string MediumName)
        {
            var BankCode = Transaction.TAN4(connectionDetails, TAN, MediumName);

            if (BankCode.Contains("+0020::"))
            {
                var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

                String[] values = BankCode_.Split('+');

                string msg = string.Empty;

                foreach (var item in values)
                {
                    if (!item.StartsWith("HIRMS"))
                        msg = msg + "??" + item.Replace("::", ": ");
                }

                return msg;
            }
            else
            {
                // Error
                var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

                String[] values = BankCode_.Split('+');

                string msg = string.Empty;

                foreach (var item in values)
                {
                    if (!item.StartsWith("HIRMS"))
                        msg = msg + "??" + item.Replace("::", ": ");
                }

                Log.Write(msg);

                return msg;
            }
        }

        /// <summary>
        /// Request tan medium name
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz</param>
        /// <returns>
        /// TAN Medium Name
        /// </returns>
        public static string RequestTANMediumName(ConnectionDetails connectionDetails)
        {
            var BankCode = Transaction.HKTAB(connectionDetails);

            if (BankCode.Contains("+0020::"))
            {
                var BankCode_ = "HITAB" + Helper.Parse_String(BankCode, "'HITAB", "'");

                var msg = BankCode;

                msg = Helper.Parse_String(msg, "+A:1", "'").Replace(":", "");

                return msg;
            }
            else
            {
                // Error
                var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

                String[] values = BankCode_.Split('+');

                string msg = string.Empty;

                foreach (var item in values)
                {
                    if (!item.StartsWith("HIRMS"))
                        msg = msg + "??" + item.Replace("::", ": ");
                }

                Log.Write(msg);

                return msg;
            }
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
        public static void Tracing(bool Enabled)
        {
            Trace.Enabled = Enabled;
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
        public static void Logging(bool Enabled)
        {
            Log.Enabled = Enabled;
        }
    }
}