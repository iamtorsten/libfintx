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

//#define WINDOWS

using libfintx.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using static libfintx.HKCDE;

#if WINDOWS
using System.Windows.Forms;
#endif

namespace libfintx
{
    public class Main
    {
        public const string Error = "Error";

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
        /// Account balance
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, Account, IBAN, BIC</param>
        /// <param name="Anonymous"></param>
        /// <returns>
        /// Structured information about balance, creditline and used currency
        /// </returns>
        public static HBCIDialogResult<AccountBalance> Balance(ConnectionDetails connectionDetails, bool anonymous)
        {
            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                // Success
                var BankCode = Transaction.HKSAL(connectionDetails);
                return HBCIDialogResult<AccountBalance>.Success(Helper.Parse_Balance(BankCode));
            }
            else
            {
                Log.Write("Error in initialization.");
                return HBCIDialogResult<AccountBalance>.Error(TransactionConsole.Output);
            }
        }

        public static HBCIDialogResult<List<AccountInformations>> Accounts(ConnectionDetails connectionDetails, bool anonymous)
        {
            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                // Success
                var upd = Helper.GetUPD(connectionDetails.Blz, connectionDetails.UserId);
                List<AccountInformations> result = new List<AccountInformations>();
                Helper.Parse_Accounts(upd, result);
                return HBCIDialogResult<List<AccountInformations>>.Success(result);
            }
            else
            {
                Log.Write("Error in initialization.");
                return HBCIDialogResult<List<AccountInformations>>.Error(TransactionConsole.Output);
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
        public static HBCIDialogResult<List<SWIFTStatement>> Transactions(ConnectionDetails connectionDetails, bool anonymous, DateTime? startDate = null, DateTime? endDate = null)
        {
            var swiftStatements = new List<SWIFTStatement>();

            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                var startDateStr = startDate?.ToString("yyyyMMdd");
                var endDateStr = endDate?.ToString("yyyyMMdd");

                // Success
                var BankCode = Transaction.HKKAZ(connectionDetails, startDateStr, endDateStr, null);

                var Transactions = string.Empty;

                if (BankCode.Contains("HNSHA"))
                    Transactions = ":20:" + Helper.Parse_String(BankCode, ":20:", "'HNSHA");
                else // -> Postbank finishes with HNHBS
                    Transactions = ":20:" + Helper.Parse_String(BankCode, ":20:", "'HNHBS");

                swiftStatements.AddRange(MT940.Serialize(Transactions, connectionDetails.Account));

                string BankCode_ = BankCode;
                while (BankCode_.Contains("+3040::"))
                {
                    Helper.Parse_Message(BankCode_);

                    var Startpoint = new Regex(@"\+3040::[^:]+:(?<startpoint>[^']+)'").Match(BankCode_).Groups["startpoint"].Value;

                    BankCode_ = Transaction.HKKAZ(connectionDetails, startDateStr, endDateStr, Startpoint);

                    var Transactions_ = ":20:" + Helper.Parse_String(BankCode_, ":20:", "'HNSHA");

                    swiftStatements.AddRange(MT940.Serialize(Transactions_, connectionDetails.Account));
                }
                return HBCIDialogResult<List<SWIFTStatement>>.Success(swiftStatements);
            }
            else
            {
                Log.Write("Initialization/sync failed");

                return HBCIDialogResult<List<SWIFTStatement>>.Error(TransactionConsole.Output);
            }                
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
        public static HBCIDialogResult<List<TStatement>> Transactions_camt(ConnectionDetails connectionDetails, bool anonymous, camtVersion camtVers,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                // Plain camt message
                var camt = string.Empty;

                var startDateStr = startDate?.ToString("yyyyMMdd");
                var endDateStr = endDate?.ToString("yyyyMMdd");

                // Success
                var BankCode = Transaction.HKCAZ(connectionDetails, startDateStr, endDateStr, null, camtVers);

                List<TStatement> result = new List<TStatement>();

                TCAM052TParser CAMT052Parser = null;
                TCAM053TParser CAMT053Parser = null;

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
                            // Save camt052 statement to file
                            var camt052f = camt052File.Save(connectionDetails.Account, camt);

                            // Process the camt053 file
                            if (CAMT052Parser == null)
                                CAMT052Parser = new TCAM052TParser();
                            CAMT052Parser.ProcessFile(camt052f);

                            result.AddRange(CAMT052Parser.statements);
                            break;
                        case camtVersion.camt053:
                            // Save camt053 statement to file
                            var camt053f = camt053File.Save(connectionDetails.Account, camt);

                            // Process the camt053 file
                            if (CAMT053Parser == null)
                                CAMT053Parser = new TCAM053TParser();
                            CAMT053Parser.ProcessFile(camt053f);

                            result.AddRange(CAMT052Parser.statements);
                            break;
                    }

                    BankCode_ = BankCode_.Substring(xmlEndIdx);
                    xmlStartIdx = BankCode_.IndexOf("<?xml version");
                    xmlEndIdx = BankCode_.IndexOf("</Document>") + "</Document>".Length;
                }

                BankCode_ = BankCode;

                string Startpoint = string.Empty;

                while (BankCode_.Contains("+3040::"))
                {
                    switch (camtVers)
                    {
                        case camtVersion.camt052:
                            Helper.Parse_Message(BankCode_);

                            Startpoint = new Regex(@"\+3040::[^:]+:(?<startpoint>[^']+)'").Match(BankCode_).Groups["startpoint"].Value;

                            BankCode_ = Transaction.HKCAZ(connectionDetails, startDateStr, endDateStr, Startpoint, camtVers);

                            var camt052_ = "<?xml version=" + Helper.Parse_String(BankCode, "<?xml version=", "</Document>") + "</Document>";

                            // Save camt052 statement to file
                            var camt052f_ = camt052File.Save(connectionDetails.Account, camt052_);

                            // Process the camt052 file
                            CAMT052Parser.ProcessFile(camt052f_);

                            // Add all items
                            result.AddRange(CAMT052Parser.statements);
                            break;
                        case camtVersion.camt053:
                            Helper.Parse_Message(BankCode_);

                            Startpoint = new Regex(@"\+3040::[^:]+:(?<startpoint>[^']+)'").Match(BankCode_).Groups["startpoint"].Value;

                            BankCode_ = Transaction.HKCAZ(connectionDetails, startDateStr, endDateStr, Startpoint, camtVers);

                            var camt053_ = "<?xml version=" + Helper.Parse_String(BankCode, "<?xml version=", "</Document>") + "</Document>";

                            // Save camt053 statement to file
                            var camt053f_ = camt053File.Save(connectionDetails.Account, camt053_);

                            // Process the camt053 file
                            CAMT053Parser.ProcessFile(camt053f_);

                            // Add all items to existing statement
                            result.AddRange(CAMT053Parser.statements);
                            break;
                    }
                }

                return HBCIDialogResult<List<TStatement>>.Success(result);
            }
            else
            {
                return HBCIDialogResult<List<TStatement>>.Error(TransactionConsole.Output);
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
        public static HBCIDialogResult<List<AccountTransaction>> TransactionsSimple(ConnectionDetails connectionDetails, bool anonymous, DateTime? startDate = null, DateTime? endDate = null)
        {
            var transactionList = new List<AccountTransaction>();

            var res = Transactions(connectionDetails, anonymous, startDate, endDate);
            if (res.IsSuccess)
            {
                foreach (var swiftStatement in res.Data)
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
                return HBCIDialogResult<List<AccountTransaction>>.Success(transactionList);
            }

            return HBCIDialogResult<List<AccountTransaction>>.Error(res.ErrorMessage);
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

#if WINDOWS
        public static string Transfer(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, string HIRMS, PictureBox pictureBox, bool anonymous)
#else
        public static HBCIDialogResult Transfer(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, string HIRMS, object pictureBox, bool anonymous)
#endif

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
        public static HBCIDialogResult Transfer(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
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

#if WINDOWS
        public static string Transfer(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, string HIRMS, PictureBox pictureBox, bool anonymous, out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120, bool renderFlickerCodeAsGif = false)
#else
        public static HBCIDialogResult Transfer(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, string HIRMS, object pictureBox, bool anonymous, out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120, bool renderFlickerCodeAsGif = false)
#endif

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
                    Helper.Parse_BankCode(BankCode, pictureBox, out flickerImage, flickerWidth, flickerHeight, renderFlickerCodeAsGif);
                    
                    return HBCIDialogResult.DefaultSuccess();
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);
                    
                    Log.Write(msg);

                    return HBCIDialogResult.DefaultError(msg);
                }
            }
            else
            {
                return HBCIDialogResult.DefaultError(TransactionConsole.Output);
            }
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

#if WINDOWS
        public static string Transfer_Terminated(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, DateTime executionDay, string HIRMS, PictureBox pictureBox, bool anonymous,
            out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120, bool renderFlickerCodeAsGif = false)
#else
        public static HBCIDialogResult Transfer_Terminated(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, DateTime executionDay, string HIRMS, object pictureBox, bool anonymous,
            out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120, bool renderFlickerCodeAsGif = false)
#endif
        {
            flickerImage = null;

            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCSE(connectionDetails, receiverName, receiverIBAN, receiverBIC, amount, purpose, executionDay);

                if (BankCode.Contains("+0030::"))
                {
                    Helper.Parse_BankCode(BankCode, pictureBox, out flickerImage, flickerWidth, flickerHeight, renderFlickerCodeAsGif);

                    return HBCIDialogResult.DefaultSuccess();
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);

                    Log.Write(msg);

                    return HBCIDialogResult.DefaultError(msg);
                }
            }
            else
                return HBCIDialogResult.DefaultError(TransactionConsole.Output);
        }

        /// <summary>
        /// Transfer money at a certain time - render FlickerCode in WinForms
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
        /// <returns>
        /// Bank return codes
        /// </returns>

#if WINDOWS
        public static string Transfer_Terminated(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, DateTime executionDay, string HIRMS, PictureBox pictureBox, bool anonymous)
#else
        public static HBCIDialogResult Transfer_Terminated(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, DateTime executionDay, string HIRMS, object pictureBox, bool anonymous)
#endif

        {
            Image img = null;
            return Transfer_Terminated(connectionDetails, receiverName, receiverIBAN, receiverBIC,
            amount, purpose, executionDay, HIRMS, pictureBox, anonymous, out img);
        }

        /// <summary>
        /// Transfer money at a certain time - render FlickerCode as Gif
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>  
        /// <param name="receiverName">Name of the recipient</param>
        /// <param name="receiverIBAN">IBAN of the recipient</param>
        /// <param name="receiverBIC">BIC of the recipient</param>
        /// <param name="amount">Amount to transfer</param>
        /// <param name="purpose">Short description of the transfer (dt. Verwendungszweck)</param>
        /// <param name="executionDay"></param>
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 911 for "Sparkasse chipTan optisch"</param>        
        /// <param name="anonymous"></param>
        /// <param name="flickerImage">(Out) reference to an image object that shall receive the FlickerCode as GIF image</param>
        /// <param name="flickerWidth">Width of the flicker code</param>
        /// <param name="flickerHeight">Height of the flicker code</param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static HBCIDialogResult Transfer_Terminated(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, DateTime executionDay, string HIRMS, bool anonymous, out Image flickerImage, int flickerWidth, int flickerHeight)
        {
            return Transfer_Terminated(connectionDetails, receiverName, receiverIBAN, receiverBIC,
            amount, purpose, executionDay, HIRMS, null, anonymous, out flickerImage, flickerWidth, flickerHeight, true);
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

#if WINDOWS
        public static string CollectiveTransfer(ConnectionDetails connectionDetails, List<pain00100203_ct_data> painData,
            string numberOfTransactions, decimal totalAmount, string HIRMS, PictureBox pictureBox, bool anonymous,
            out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120, bool renderFlickerCodeAsGif = false)
#else
        public static HBCIDialogResult CollectiveTransfer(ConnectionDetails connectionDetails, List<pain00100203_ct_data> painData,
            string numberOfTransactions, decimal totalAmount, string HIRMS, object pictureBox, bool anonymous,
            out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120, bool renderFlickerCodeAsGif = false)
#endif
        {
            flickerImage = null;

            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCCM(connectionDetails, painData, numberOfTransactions, totalAmount);

                if (BankCode.Contains("+0030::"))
                {
                    Helper.Parse_BankCode(BankCode, pictureBox, out flickerImage, flickerWidth, flickerHeight, renderFlickerCodeAsGif);

                    return HBCIDialogResult.DefaultSuccess();
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);

                    Log.Write(msg);

                    return HBCIDialogResult.DefaultError(msg);
                }
            }
            else
                return HBCIDialogResult.DefaultError(TransactionConsole.Output);
        }

        /// <summary>
        /// Collective transfer money - render FlickerCode in WinForms
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
        /// <returns>
        /// Bank return codes
        /// </returns>

#if WINDOWS
        public static string CollectiveTransfer(ConnectionDetails connectionDetails, List<pain00100203_ct_data> painData,
            string numberOfTransactions, decimal totalAmount, string HIRMS, object pictureBox, bool anonymous)
#else
        public static HBCIDialogResult CollectiveTransfer(ConnectionDetails connectionDetails, List<pain00100203_ct_data> painData,
            string numberOfTransactions, decimal totalAmount, string HIRMS, object pictureBox, bool anonymous)
#endif

        {
            Image img = null;
            return CollectiveTransfer(connectionDetails, painData, numberOfTransactions, totalAmount, HIRMS, pictureBox, anonymous, out img);
        }

        /// <summary>
        /// Collective transfer money - render FlickerCode as Gif
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>  
        /// <param name="receiverName">Name of the recipient</param>
        /// <param name="receiverIBAN">IBAN of the recipient</param>
        /// <param name="receiverBIC">BIC of the recipient</param>
        /// <param name="amount">Amount to transfer</param>
        /// <param name="purpose">Short description of the transfer (dt. Verwendungszweck)</param>
        /// <param name="executionDay"></param>
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 911 for "Sparkasse chipTan optisch"</param>        
        /// <param name="anonymous"></param>
        /// <param name="flickerImage">(Out) reference to an image object that shall receive the FlickerCode as GIF image</param>
        /// <param name="flickerWidth">Width of the flicker code</param>
        /// <param name="flickerHeight">Height of the flicker code</param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static HBCIDialogResult CollectiveTransfer(ConnectionDetails connectionDetails, List<pain00100203_ct_data> painData,
            string numberOfTransactions, decimal totalAmount, string HIRMS, bool anonymous, out Image flickerImage, int flickerWidth,
            int flickerHeight)
        {
            return CollectiveTransfer(connectionDetails, painData, numberOfTransactions, totalAmount, HIRMS, null, anonymous,
                out flickerImage, flickerWidth, flickerHeight, true);
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

#if WINDOWS
        public static string CollectiveTransfer_Terminated(ConnectionDetails connectionDetails, List<pain00100203_ct_data> painData,
            string numberOfTransactions, decimal totalAmount, DateTime executionDay, string HIRMS, PictureBox pictureBox, bool anonymous,
            out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120, bool renderFlickerCodeAsGif = false)
#else
        public static HBCIDialogResult CollectiveTransfer_Terminated(ConnectionDetails connectionDetails, List<pain00100203_ct_data> painData,
            string numberOfTransactions, decimal totalAmount, DateTime executionDay, string HIRMS, object pictureBox, bool anonymous,
            out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120, bool renderFlickerCodeAsGif = false)
#endif
        {
            flickerImage = null;

            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCME(connectionDetails, painData, numberOfTransactions, totalAmount, executionDay);

                if (BankCode.Contains("+0030::"))
                {
                    Helper.Parse_BankCode(BankCode, pictureBox, out flickerImage, flickerWidth, flickerHeight, renderFlickerCodeAsGif);

                    return HBCIDialogResult.DefaultSuccess();
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);

                    Log.Write(msg);

                    return HBCIDialogResult.DefaultError(msg);
                }
            }
            else
                return HBCIDialogResult.DefaultError(TransactionConsole.Output);
        }

        /// <summary>
        /// Collective transfer money terminated - render FlickerCode in WinForms
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
        /// <returns>
        /// Bank return codes
        /// </returns>

#if WINDOWS
        public static string CollectiveTransfer_Terminated(ConnectionDetails connectionDetails, List<pain00100203_ct_data> painData,
            string numberOfTransactions, decimal totalAmount, DateTime executionDay, string HIRMS, object pictureBox, bool anonymous)
#else
        public static HBCIDialogResult CollectiveTransfer_Terminated(ConnectionDetails connectionDetails, List<pain00100203_ct_data> painData,
            string numberOfTransactions, decimal totalAmount, DateTime executionDay, string HIRMS, object pictureBox, bool anonymous)
#endif

        {
            Image img = null;
            return CollectiveTransfer_Terminated(connectionDetails, painData, numberOfTransactions, totalAmount, executionDay, 
                HIRMS, pictureBox, anonymous, out img);
        }

        /// <summary>
        /// Collective transfer money terminated - render FlickerCode as Gif
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>  
        /// <param name="receiverName">Name of the recipient</param>
        /// <param name="receiverIBAN">IBAN of the recipient</param>
        /// <param name="receiverBIC">BIC of the recipient</param>
        /// <param name="amount">Amount to transfer</param>
        /// <param name="purpose">Short description of the transfer (dt. Verwendungszweck)</param>
        /// <param name="executionDay"></param>
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 911 for "Sparkasse chipTan optisch"</param>        
        /// <param name="anonymous"></param>
        /// <param name="flickerImage">(Out) reference to an image object that shall receive the FlickerCode as GIF image</param>
        /// <param name="flickerWidth">Width of the flicker code</param>
        /// <param name="flickerHeight">Height of the flicker code</param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static HBCIDialogResult CollectiveTransfer_Terminated(ConnectionDetails connectionDetails, List<pain00100203_ct_data> painData,
            string numberOfTransactions, decimal totalAmount, DateTime executionDay, string HIRMS, bool anonymous, out Image flickerImage, int flickerWidth,
            int flickerHeight)
        {
            return CollectiveTransfer_Terminated(connectionDetails, painData, numberOfTransactions, totalAmount, executionDay, HIRMS, null, anonymous,
                out flickerImage, flickerWidth, flickerHeight, true);
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

#if WINDOWS
        public static string Rebooking(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, string HIRMS, PictureBox pictureBox, bool anonymous, out Image flickerImage,
            int flickerWidth = 320, int flickerHeight = 120, bool renderFlickerCodeAsGif = false)
#else
        public static HBCIDialogResult Rebooking(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, string HIRMS, object pictureBox, bool anonymous, out Image flickerImage,
            int flickerWidth = 320, int flickerHeight = 120, bool renderFlickerCodeAsGif = false)
#endif
        {
            flickerImage = null;

            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCUM(connectionDetails, receiverName, receiverIBAN, receiverBIC, amount, purpose);

                if (BankCode.Contains("+0030::"))
                {
                    Helper.Parse_BankCode(BankCode, pictureBox, out flickerImage, flickerWidth, flickerHeight, renderFlickerCodeAsGif);

                    return HBCIDialogResult.DefaultSuccess();
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);

                    Log.Write(msg);

                    return HBCIDialogResult.DefaultError(msg);
                }
            }
            else
                return HBCIDialogResult.DefaultError(TransactionConsole.Output);
        }

        /// <summary>
        /// Rebook money from one to another account - render FlickerCode as Gif
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
        public static HBCIDialogResult Rebooking(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, string HIRMS, bool anonymous, out Image flickerImage, int flickerWidth, int flickerHeight)
        {
            return Rebooking(connectionDetails, receiverName, receiverIBAN, receiverBIC,
            amount, purpose, HIRMS, null, anonymous, out flickerImage, flickerWidth, flickerHeight, true);
        }

        /// <summary>
        /// Rebook money from one to another account - render FlickerCode in WinForms
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

#if WINDOWS
        public static string Rebooking(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, string HIRMS, PictureBox pictureBox, bool anonymous)
#else
        public static HBCIDialogResult Rebooking(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose, string HIRMS, object pictureBox, bool anonymous)
#endif

        {
            Image img = null;
            return Rebooking(connectionDetails, receiverName, receiverIBAN, receiverBIC,
            amount, purpose, HIRMS, pictureBox, anonymous, out img);
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

#if WINDOWS
        public static string Collect(ConnectionDetails connectionDetails, string payerName, string payerIBAN, string payerBIC,
            decimal amount, string purpose, DateTime settlementDate, string mandateNumber, DateTime mandateDate, string creditorIdNumber,
            string HIRMS, PictureBox pictureBox, bool anonymous, out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120,
            bool renderFlickerCodeAsGif = false)
#else
        public static HBCIDialogResult Collect(ConnectionDetails connectionDetails, string payerName, string payerIBAN, string payerBIC,
            decimal amount, string purpose, DateTime settlementDate, string mandateNumber, DateTime mandateDate, string creditorIdNumber,
            string HIRMS, object pictureBox, bool anonymous, out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120,
            bool renderFlickerCodeAsGif = false)
#endif
        {
            flickerImage = null;

            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKDSE(connectionDetails, payerName, payerIBAN, payerBIC, amount, purpose, settlementDate,
                    mandateNumber, mandateDate, creditorIdNumber);

                if (BankCode.Contains("+0030::"))
                {
                    Helper.Parse_BankCode(BankCode, pictureBox, out flickerImage, flickerWidth, flickerHeight, renderFlickerCodeAsGif);

                    return HBCIDialogResult.DefaultSuccess();
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);

                    Log.Write(msg);

                    return HBCIDialogResult.DefaultError(msg);
                }
            }
            else
                return HBCIDialogResult.DefaultError(TransactionConsole.Output);
        }

        /// <summary>
        /// Collect money from another account - render FlickerCode as Gif
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
        public static HBCIDialogResult Collect(ConnectionDetails connectionDetails, string payerName, string payerIBAN, string payerBIC,
            decimal amount, string purpose, DateTime settlementDate, string mandateNumber, DateTime mandateDate, string creditorIdNumber,
            string HIRMS, object pictureBox, bool anonymous, out Image flickerImage, int flickerWidth, int flickerHeight)
        {
            return Collect(connectionDetails, payerName, payerIBAN, payerBIC,
            amount, purpose, settlementDate, mandateNumber, mandateDate, creditorIdNumber, HIRMS, null, anonymous,
            out flickerImage, flickerWidth, flickerHeight, true);
        }

        /// <summary>
        /// Collect money from another account - render FlickerCode in WinForms
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
        /// <returns>
        /// Bank return codes
        /// </returns>

#if WINDOWS
        public static string Collect(ConnectionDetails connectionDetails, string payerName, string payerIBAN, string payerBIC,
            decimal amount, string purpose, DateTime settlementDate, string mandateNumber, DateTime mandateDate, string creditorIdNumber,
            string HIRMS, object pictureBox, bool anonymous)
#else
        public static HBCIDialogResult Collect(ConnectionDetails connectionDetails, string payerName, string payerIBAN, string payerBIC,
            decimal amount, string purpose, DateTime settlementDate, string mandateNumber, DateTime mandateDate, string creditorIdNumber,
            string HIRMS, object pictureBox, bool anonymous)
#endif

        {
            Image img = null;
            return Collect(connectionDetails, payerName, payerIBAN, payerBIC,
            amount, purpose, settlementDate, mandateNumber, mandateDate, creditorIdNumber, HIRMS, null, anonymous, out img);
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

#if WINDOWS
        public static string CollectiveCollect(ConnectionDetails connectionDetails, DateTime settlementDate, List<pain00800202_cc_data> painData,
            string numberOfTransactions, decimal totalAmount, string HIRMS, PictureBox pictureBox, bool anonymous, out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120,
            bool renderFlickerCodeAsGif = false)
#else
        public static HBCIDialogResult CollectiveCollect(ConnectionDetails connectionDetails, DateTime settlementDate, List<pain00800202_cc_data> painData,
           string numberOfTransactions, decimal totalAmount, string HIRMS, object pictureBox, bool anonymous, out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120,
           bool renderFlickerCodeAsGif = false)
#endif
        {
            flickerImage = null;

            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKDME(connectionDetails, settlementDate, painData, numberOfTransactions, totalAmount);

                if (BankCode.Contains("+0030::"))
                {
                    Helper.Parse_BankCode(BankCode, pictureBox, out flickerImage, flickerWidth, flickerHeight, renderFlickerCodeAsGif);

                    return HBCIDialogResult.DefaultSuccess();
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);

                    Log.Write(msg);

                    return HBCIDialogResult.DefaultError(msg);
                }
            }
            else
                return HBCIDialogResult.DefaultError(TransactionConsole.Output);
        }

        /// <summary>
        /// Collective collect money from other accounts - render FlickerCode as Gif
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
        public static HBCIDialogResult CollectiveCollect(ConnectionDetails connectionDetails, DateTime settlementDate, List<pain00800202_cc_data> painData,
           string numberOfTransactions, decimal totalAmount, string HIRMS, object pictureBox, bool anonymous, out Image flickerImage, int flickerWidth, int flickerHeight)
        {
            return CollectiveCollect(connectionDetails, settlementDate, painData, numberOfTransactions,
            totalAmount, HIRMS, null, anonymous, out flickerImage, flickerWidth, flickerHeight, true);
        }

        /// <summary>
        /// Collective collect money from other accounts - render FlickerCode in WinForms
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC, AccountHolder</param>  
        /// <param name="settlementDate"></param>
        /// <param name="painData"></param>
        /// <param name="numberOfTransactions"></param>
        /// <param name="totalAmount"></param>        
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 911 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>

#if WINDOWS
        public static string Collect(ConnectionDetails connectionDetails, DateTime settlementDate, List<pain00800202_cc_data> painData,
           string numberOfTransactions, decimal totalAmount, string HIRMS, object pictureBox, bool anonymous)
#else
        public static HBCIDialogResult Collect(ConnectionDetails connectionDetails, DateTime settlementDate, List<pain00800202_cc_data> painData,
           string numberOfTransactions, decimal totalAmount, string HIRMS, object pictureBox, bool anonymous)
#endif

        {
            Image img = null;
            return CollectiveCollect(connectionDetails, settlementDate, painData, numberOfTransactions,
            totalAmount, HIRMS, null, anonymous, out img);
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

#if WINDOWS
        public static string Prepaid(ConnectionDetails connectionDetails, int mobileServiceProvider, string phoneNumber,
            int amount, string HIRMS, PictureBox pictureBox, bool anonymous, out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120,
            bool renderFlickerCodeAsGif = false)
#else
        public static HBCIDialogResult Prepaid(ConnectionDetails connectionDetails, int mobileServiceProvider, string phoneNumber,
            int amount, string HIRMS, object pictureBox, bool anonymous, out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120,
            bool renderFlickerCodeAsGif = false)
#endif
        {
            flickerImage = null;

            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKPPD(connectionDetails, mobileServiceProvider, phoneNumber, amount);

                if (BankCode.Contains("+0030::"))
                {
                    Helper.Parse_BankCode(BankCode, pictureBox, out flickerImage, flickerWidth, flickerHeight, renderFlickerCodeAsGif);

                    return HBCIDialogResult.DefaultSuccess();
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);

                    Log.Write(msg);

                    return HBCIDialogResult.DefaultError(msg);
                }
            }
            else
                return HBCIDialogResult.DefaultError(TransactionConsole.Output);
        }

        /// <summary>
        /// Load mobile phone prepaid card - render FlickerCode in WinForms
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC</param>  
        /// <param name="mobileServiceProvider"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="amount">Amount to transfer</param>            
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 911 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>

#if WINDOWS
        public static string Prepaid(ConnectionDetails connectionDetails, int mobileServiceProvider, string phoneNumber,
            int amount, string HIRMS, PictureBox pictureBox, bool anonymous)
#else
        public static HBCIDialogResult Prepaid(ConnectionDetails connectionDetails, int mobileServiceProvider, string phoneNumber,
            int amount, string HIRMS, object pictureBox, bool anonymous)
#endif

        {
            Image img = null;
            return Prepaid(connectionDetails, mobileServiceProvider, phoneNumber,
            amount, HIRMS, pictureBox, anonymous, out img);
        }

        /// <summary>
        /// Load mobile phone prepaid card - render FlickerCode as Gif
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
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static HBCIDialogResult Prepaid(ConnectionDetails connectionDetails, int mobileServiceProvider, string phoneNumber,
            int amount, string HIRMS, object pictureBox, bool anonymous, out Image flickerImage, int flickerWidth, int flickerHeight)
        {
            return Prepaid(connectionDetails, mobileServiceProvider, phoneNumber,
            amount, HIRMS, pictureBox, anonymous, out flickerImage, flickerWidth, flickerHeight, true);
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

#if WINDOWS
        public static string SubmitBankersOrder(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN,
            string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota,
            int executionDay, string HIRMS, PictureBox pictureBox, bool anonymous, out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120,
            bool renderFlickerCodeAsGif = false)
#else
        public static HBCIDialogResult SubmitBankersOrder(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN,
           string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, TimeUnit timeUnit, string rota,
           int executionDay, string HIRMS, object pictureBox, bool anonymous, out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120,
           bool renderFlickerCodeAsGif = false)
#endif
        {
            flickerImage = null;

            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCDE(connectionDetails, receiverName, receiverIBAN, receiverBIC, amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay);

                if (BankCode.Contains("+0030::"))
                {
                    Helper.Parse_BankCode(BankCode, pictureBox, out flickerImage, flickerWidth, flickerHeight, renderFlickerCodeAsGif);

                    return HBCIDialogResult.DefaultSuccess();
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);

                    Log.Write(msg);

                    return HBCIDialogResult.DefaultError(msg);
                }
            }
            else
                return HBCIDialogResult.DefaultError(TransactionConsole.Output);
        }

        /// <summary>
        /// Submit bankers order - render FlickerCode in WinForms
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
        /// <returns>
        /// Bank return codes
        /// </returns>

#if WINDOWS
        public static string SubmitBankersOrder(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN,
           string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota,
           int executionDay, string HIRMS, object pictureBox, bool anonymous)
#else
        public static HBCIDialogResult SubmitBankersOrder(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN,
           string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota,
           int executionDay, string HIRMS, object pictureBox, bool anonymous)
#endif

        {
            Image img = null;
            return SubmitBankersOrder(connectionDetails, receiverName, receiverIBAN, receiverBIC,
            amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay, HIRMS, pictureBox, anonymous, out img);
        }

        /// <summary>
        /// Submit bankers order - render FlickerCode as Gif
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
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static HBCIDialogResult SubmitBankersOrder(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN,
           string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota,
           int executionDay, string HIRMS, object pictureBox, bool anonymous, out Image flickerImage, int flickerWidth, int flickerHeight)
        {
            return SubmitBankersOrder(connectionDetails, receiverName, receiverIBAN, receiverBIC,
            amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay, HIRMS, pictureBox, anonymous, out flickerImage, flickerWidth, flickerHeight, true);
        }

#if WINDOWS
        public static string ModifyBankersOrder(ConnectionDetails connectionDetails, string receiverName, string receiverIBAN,
            string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota,
            int executionDay, string HIRMS, PictureBox pictureBox, bool anonymous, out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120,
            bool renderFlickerCodeAsGif = false)
#else
        public static HBCIDialogResult ModifyBankersOrder(ConnectionDetails connectionDetails, string OrderId, string receiverName, string receiverIBAN,
           string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, TimeUnit timeUnit, string rota,
           int executionDay, string HIRMS, object pictureBox, bool anonymous, out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120,
           bool renderFlickerCodeAsGif = false)
#endif
        {
            flickerImage = null;

            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCDN(connectionDetails, OrderId, receiverName, receiverIBAN, receiverBIC, amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay);

                if (BankCode.Contains("+0030::"))
                {
                    Helper.Parse_BankCode(BankCode, pictureBox, out flickerImage, flickerWidth, flickerHeight, renderFlickerCodeAsGif);

                    return HBCIDialogResult.DefaultSuccess();
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);

                    Log.Write(msg);

                    return HBCIDialogResult.DefaultError(msg);
                }
            }
            else
                return HBCIDialogResult.DefaultError(TransactionConsole.Output);
        }

        /// <summary>
        /// Modify bankers order - render FlickerCode in WinForms
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
        /// <returns>
        /// Bank return codes
        /// </returns>

#if WINDOWS
        public static string ModifyBankersOrder(ConnectionDetails connectionDetails, string orderId, string receiverName, string receiverIBAN,
           string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota,
           int executionDay, string HIRMS, object pictureBox, bool anonymous)
#else
        public static HBCIDialogResult ModifyBankersOrder(ConnectionDetails connectionDetails, string orderId, string receiverName, string receiverIBAN,
           string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota,
           int executionDay, string HIRMS, object pictureBox, bool anonymous)
#endif

        {
            Image img = null;
            return ModifyBankersOrder(connectionDetails, orderId, receiverName, receiverIBAN, receiverBIC,
            amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay, HIRMS, pictureBox, anonymous, out img);
        }

        /// <summary>
        /// Modify bankers order - render FlickerCode as Gif
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
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static HBCIDialogResult ModifyBankersOrder(ConnectionDetails connectionDetails, string orderId, string receiverName, string receiverIBAN,
           string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota,
           int executionDay, string HIRMS, object pictureBox, bool anonymous, out Image flickerImage, int flickerWidth, int flickerHeight)
        {
            return ModifyBankersOrder(connectionDetails, orderId, receiverName, receiverIBAN, receiverBIC,
            amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay, HIRMS, pictureBox, anonymous, out flickerImage, flickerWidth, flickerHeight, true);
        }

        public static HBCIDialogResult DeleteBankersOrder(ConnectionDetails connectionDetails, string orderId, string receiverName, string receiverIBAN,
            string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota, int executionDay, string HIRMS,
            object pictureBox, bool anonymous, 
            out Image flickerImage, int flickerWidth = 320, int flickerHeight = 120, bool renderFlickerCodeAsGif = false)
        {
            flickerImage = null;

            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCDL(connectionDetails, orderId, receiverName, receiverIBAN, receiverBIC, amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay);

                if (BankCode.Contains("+0030::"))
                {
                    Helper.Parse_BankCode(BankCode, pictureBox, out flickerImage, flickerWidth, flickerHeight, renderFlickerCodeAsGif);

                    return HBCIDialogResult.DefaultSuccess();
                }
                else
                {
                    var msg = Helper.Parse_BankCode_Error(BankCode);

                    Log.Write(msg);

                    return HBCIDialogResult.DefaultError(msg);
                }
            }
            else
                return HBCIDialogResult.DefaultError(TransactionConsole.Output);
        }

        public static HBCIDialogResult DeleteBankersOrder(ConnectionDetails conn, string orderId, string receiverName, string receiverIBAN,
            string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota, int executionDay, string HIRMS,
            object pictureBox, bool anonymous)
        {
            Image img = null;
            return DeleteBankersOrder(conn, orderId, receiverName, receiverIBAN, receiverBIC, amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay, HIRMS, pictureBox, anonymous, out img);
        }

        public static HBCIDialogResult DeleteBankersOrder(ConnectionDetails conn, string orderId, string receiverName, string receiverIBAN,
            string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota, int executionDay, string HIRMS,
            object pictureBox, bool anonymous,
            out Image flickerImage, int flickerWidth, int flickerHeight)
        {
            return DeleteBankersOrder(conn, orderId, receiverName, receiverIBAN, receiverBIC, amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay, HIRMS, pictureBox, anonymous, out flickerImage, flickerWidth, flickerHeight, true);
        }

        /// <summary>
        /// Get banker's orders
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC</param>         
        /// <param name="anonymous"></param>
        /// <returns>
        /// Banker's orders
        /// </returns>
        public static HBCIDialogResult<List<BankersOrder>> GetBankersOrders(ConnectionDetails connectionDetails, bool anonymous)
        {
            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                // Success
                var BankCode = Transaction.HKCDB(connectionDetails);

                if (BankCode.Contains("+0020::"))
                {
                    List<BankersOrder> data = new List<BankersOrder>();

                    var BankCode_ = BankCode;

                    var startIdx = BankCode_.IndexOf("HICDB");
                    if (startIdx < 0)
                        return HBCIDialogResult<List<BankersOrder>>.Success(data);
                    
                    BankCode_ = BankCode_.Substring(startIdx);
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
                    return HBCIDialogResult<List<BankersOrder>>.Success(data);
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

                    return HBCIDialogResult<List<BankersOrder>>.Error(msg);
                }
            }
            else
                return HBCIDialogResult<List<BankersOrder>>.Error(TransactionConsole.Output);
        }

        /// <summary>
        /// Get terminated transfers
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, IBAN, BIC</param>         
        /// <param name="anonymous"></param>
        /// <returns>
        /// Banker's orders
        /// </returns>
        public static HBCIDialogResult<string> GetTerminatedTransfers(ConnectionDetails connectionDetails, bool anonymous)
        {
            if (Transaction.INI(connectionDetails, anonymous) == true)
            {
                // Success
                var BankCode = Transaction.HKCSB(connectionDetails);

                if (BankCode.Contains("+0020::"))
                {
                    // Success
                    return HBCIDialogResult<string>.Success(BankCode);
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

                    return HBCIDialogResult<string>.Error(msg);
                }
            }
            else
                return HBCIDialogResult<string>.Error(TransactionConsole.Output);
        }

        /// <summary>
        /// Confirm order with TAN
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz</param>
        /// <param name="TAN"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static HBCIDialogResult<string> TAN(ConnectionDetails connectionDetails, string TAN)
        {
            var BankCode = Transaction.TAN(connectionDetails, TAN);

            if (BankCode.Contains("+0010::") || BankCode.Contains("+0020::"))
            {
                var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

                String[] values = BankCode_.Split('+');

                string msg = string.Empty;

                foreach (var item in values)
                {
                    if (!item.StartsWith("HIRMS"))
                        msg = msg + "??" + item.Replace("::", ": ");
                }

                return HBCIDialogResult<string>.Success(msg);
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

                return HBCIDialogResult<string>.Error(msg);
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
        public static HBCIDialogResult TAN4(ConnectionDetails connectionDetails, string TAN, string MediumName)
        {
            var BankCode = Transaction.TAN4(connectionDetails, TAN, MediumName);

            if (BankCode.Contains("+0010::") || BankCode.Contains("+0020::"))
            {
                var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

                String[] values = BankCode_.Split('+');

                string msg = string.Empty;

                foreach (var item in values)
                {
                    if (!item.StartsWith("HIRMS"))
                        msg = msg + "??" + item.Replace("::", ": ");
                }

                return HBCIDialogResult<string>.Success(msg);
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

                return HBCIDialogResult<string>.Error(msg);
            }
        }

        /// <summary>
        /// Request tan medium name
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz</param>
        /// <returns>
        /// TAN Medium Name
        /// </returns>
        public static HBCIDialogResult<string> RequestTANMediumName(ConnectionDetails connectionDetails)
        {
            var BankCode = Transaction.HKTAB(connectionDetails);

            if (BankCode.Contains("+0020::"))
            {
                var BankCode_ = "HITAB" + Helper.Parse_String(BankCode, "'HITAB", "'");

                return HBCIDialogResult<string>.Success(Helper.Parse_TANMedium(BankCode_));
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

                return HBCIDialogResult<string>.Error(msg);
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
        public static void Tracing(bool Enabled, bool Formatted = false)
        {
            Trace.Enabled = Enabled;
            Trace.Formatted = Formatted;
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