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

using System;
using System.Collections.Generic;


namespace libfintx
{
    public class Main
    {
        /// <summary>
        /// Synchronize bank connection
        /// </summary>
        /// <param name="BLZ"></param>
        /// <param name="URL"></param>
        /// <param name="HBCIVersion"></param>
        /// <param name="UserID"></param>
        /// <param name="PIN"></param>
        /// <param name="Anonymous"></param>
        /// <returns>
        /// Success or failure
        /// </returns>
        public static bool Synchronization(int BLZ, string URL, int HBCIVersion, string UserID, string PIN, bool Anonymous)
        {
            if (Transaction.INI(BLZ, URL, HBCIVersion, UserID, PIN, Anonymous) == true)
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Account balance
        /// </summary>
        /// <param name="Account"></param>
        /// <param name="BLZ"></param>
        /// <param name="IBAN"></param>
        /// <param name="BIC"></param>
        /// <param name="URL"></param>
        /// <param name="HBCIVersion"></param>
        /// <param name="UserID"></param>
        /// <param name="PIN"></param>
        /// <param name="Anonymous"></param>
        /// <returns>
        /// Balance
        /// </returns>
        public static string Balance(int Account, int BLZ, string IBAN, string BIC, string URL, int HBCIVersion,
            string UserID, string PIN, bool Anonymous)
        {
            if (Transaction.INI(BLZ, URL, HBCIVersion, UserID, PIN, Anonymous) == true)
            {
                // Success
                var BankCode = Transaction.HKSAL(Account, BLZ, IBAN, BIC, URL, HBCIVersion, UserID, PIN);

                if (BankCode.Contains("+0020::"))
                {
                    // Success
                    return "Kontostand: " + Helper.Parse_Balance(BankCode);
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
        /// Account transactions
        /// </summary>
        /// <param name="Account"></param>
        /// <param name="BLZ"></param>
        /// <param name="IBAN"></param>
        /// <param name="BIC"></param>
        /// <param name="URL"></param>
        /// <param name="HBCIVersion"></param>
        /// <param name="UserID"></param>
        /// <param name="PIN"></param>
        /// <param name="Anonymous"></param>
        /// <returns>
        /// Transactions
        /// </returns>
        public static string Transactions(int Account, int BLZ, string IBAN, string BIC, string URL, int HBCIVersion,
            string UserID, string PIN, bool Anonymous)
        {
            if (Transaction.INI(BLZ, URL, HBCIVersion, UserID, PIN, Anonymous) == true)
            {
                // Success
                var BankCode = Transaction.HKKAZ(Account, BLZ, IBAN, BIC, URL, HBCIVersion, UserID, PIN, null, null);

                var Transactions = ":20:STARTUMS" + Helper.Parse_String(BankCode, ":20:STARTUMS", "'HNSHA");

                MT940.Serialize(Transactions, Account);

                Further:

                if (BankCode.Contains("+3040::"))
                {
                    Helper.Parse_Message(BankCode);

                    var Startpoint = Helper.Parse_String(BankCode, "vor:", "'");

                    var BankCode_ = Transaction.HKKAZ(Account, BLZ, IBAN, BIC, URL, HBCIVersion, UserID, PIN, null, Startpoint);

                    var Transactions_ = ":20:STARTUMS" + Helper.Parse_String(BankCode_, ":20:STARTUMS", "'HNSHA");

                    MT940.Serialize(Transactions_, Account);

                    if (BankCode_.Contains("+3040::"))
                    {
                        BankCode = BankCode_;

                        goto Further;
                    }
                }

                return "OK";
            }
            else
                return "Error";
        }

        /// <summary>
        /// Transfer money
        /// </summary>
        /// <param name="BLZ"></param>
        /// <param name="AccountHolder"></param>
        /// <param name="AccountHolderIBAN"></param>
        /// <param name="AccountHolderBIC"></param>
        /// <param name="Receiver"></param>
        /// <param name="ReceiverIBAN"></param>
        /// <param name="ReceiverBIC"></param>
        /// <param name="Amount"></param>
        /// <param name="Purpose"></param>
        /// <param name="URL"></param>
        /// <param name="HBCIVersion"></param>
        /// <param name="UserID"></param>
        /// <param name="PIN"></param>
        /// <param name="HIRMS"></param>
        /// <param name="Anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string Transfer(int BLZ, string AccountHolder, string AccountHolderIBAN, string AccountHolderBIC, string Receiver, string ReceiverIBAN, string ReceiverBIC,
            string Amount, string Purpose, string URL, int HBCIVersion, string UserID, string PIN,
            string HIRMS, bool Anonymous)
        {
            if (Transaction.INI(BLZ, URL, HBCIVersion, UserID, PIN, Anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCCS(BLZ, AccountHolder, AccountHolderIBAN, AccountHolderBIC, Receiver, ReceiverIBAN, ReceiverBIC,
                    Convert.ToDecimal(Amount), Purpose, URL, HBCIVersion, UserID, PIN);

                if (BankCode.Contains("+0030::"))
                {
                    var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

                    String[] values = BankCode_.Split('+');

                    string msg = string.Empty;

                    foreach (var item in values)
                    {
                        if (!item.StartsWith("HIRMS"))
                            TransactionConsole.Output = item.Replace("::", ": ");
                    }

                    var HITAN = "HITAN" + Helper.Parse_String(BankCode.Replace("?'", "").Replace("?:", ":").Replace("<br>", Environment.NewLine).Replace("?+", "??"), "'HITAN", "'");

                    string HITANFlicker = string.Empty;

                    String[] values_ = HITAN.Split('+');

                    int i = 1;

                    foreach (var item in values_)
                    {
                        i = i + 1;

                        if (i == 6)
                            TransactionConsole.Output = TransactionConsole.Output + "??" + item.Replace("::", ": ").TrimStart();
                    }

                    return "OK";
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
        /// Transfer money at a certain time
        /// </summary>
        /// <param name="BLZ"></param>
        /// <param name="AccountHolder"></param>
        /// <param name="AccountHolderIBAN"></param>
        /// <param name="AccountHolderBIC"></param>
        /// <param name="Receiver"></param>
        /// <param name="ReceiverIBAN"></param>
        /// <param name="ReceiverBIC"></param>
        /// <param name="Amount"></param>
        /// <param name="Purpose"></param>
        /// <param name="ExecutionDay"></param>
        /// <param name="URL"></param>
        /// <param name="HBCIVersion"></param>
        /// <param name="UserID"></param>
        /// <param name="PIN"></param>
        /// <param name="HIRMS"></param>
        /// <param name="Anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string Transfer_Terminated(int BLZ, string AccountHolder, string AccountHolderIBAN, string AccountHolderBIC, string Receiver, string ReceiverIBAN, string ReceiverBIC,
			string Amount, string Purpose, string ExecutionDay, string URL, int HBCIVersion, string UserID,
            string PIN, string HIRMS, bool Anonymous)
		{
            if (Transaction.INI(BLZ, URL, HBCIVersion, UserID, PIN, Anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCCST(BLZ, AccountHolder, AccountHolderIBAN, AccountHolderBIC, Receiver, ReceiverIBAN, ReceiverBIC,
                    Convert.ToDecimal(Amount), Purpose, ExecutionDay, URL, HBCIVersion, UserID, PIN);

                if (BankCode.Contains("+0030::"))
                {
                    var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

                    String[] values = BankCode_.Split('+');

                    string msg = string.Empty;

                    foreach (var item in values)
                    {
                        if (!item.StartsWith("HIRMS"))
                            TransactionConsole.Output = item.Replace("::", ": ");
                    }

                    var HITAN = "HITAN" + Helper.Parse_String(BankCode.Replace("?'", "").Replace("?:", ":").Replace("<br>", Environment.NewLine).Replace("?+", "??"), "'HITAN", "'");

                    string HITANFlicker = string.Empty;

                    String[] values_ = HITAN.Split('+');

                    int i = 1;

                    foreach (var item in values_)
                    {
                        i = i + 1;

                        if (i == 6)
                            TransactionConsole.Output = TransactionConsole.Output + "??" + item.Replace("::", ": ").TrimStart();
                    }

                    return "OK";
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
        /// Collective transfer money
        /// </summary>
        /// <param name="BLZ"></param>
        /// <param name="AccountHolder"></param>
        /// <param name="AccountHolderIBAN"></param>
        /// <param name="AccountHolderBIC"></param>
        /// <param name="PainData"></param>
        /// <param name="NumberofTransactions"></param>
        /// <param name="TotalAmount"></param>
        /// <param name="URL"></param>
        /// <param name="HBCIVersion"></param>
        /// <param name="UserID"></param>
        /// <param name="PIN"></param>
        /// <param name="HIRMS"></param>
        /// <param name="Anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string CollectiveTransfer(int BLZ, string AccountHolder, string AccountHolderIBAN, string AccountHolderBIC, List<pain00100203_ct_data> PainData,
            string NumberofTransactions, decimal TotalAmount, string URL, int HBCIVersion, string UserID,
            string PIN, string HIRMS, bool Anonymous)
        {
            if (Transaction.INI(BLZ, URL, HBCIVersion, UserID, PIN, Anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCCM(BLZ, AccountHolder, AccountHolderIBAN, AccountHolderBIC, PainData, NumberofTransactions,
                    Convert.ToDecimal(TotalAmount), URL, HBCIVersion, UserID, PIN);

                if (BankCode.Contains("+0030::"))
                {
                    var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

                    String[] values = BankCode_.Split('+');

                    string msg = string.Empty;

                    foreach (var item in values)
                    {
                        if (!item.StartsWith("HIRMS"))
                            TransactionConsole.Output = item.Replace("::", ": ");
                    }

                    var HITAN = "HITAN" + Helper.Parse_String(BankCode.Replace("?'", "").Replace("?:", ":").Replace("<br>", Environment.NewLine).Replace("?+", "??"), "'HITAN", "'");

                    string HITANFlicker = string.Empty;

                    String[] values_ = HITAN.Split('+');

                    int i = 1;

                    foreach (var item in values_)
                    {
                        i = i + 1;

                        if (i == 6)
                            TransactionConsole.Output = TransactionConsole.Output + "??" + item.Replace("::", ": ").TrimStart();
                    }

                    return "OK";
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
        /// Collective transfer money terminated
        /// </summary>
        /// <param name="BLZ"></param>
        /// <param name="AccountHolder"></param>
        /// <param name="AccountHolderIBAN"></param>
        /// <param name="AccountHolderBIC"></param>
        /// <param name="PainData"></param>
        /// <param name="NumberofTransactions"></param>
        /// <param name="TotalAmount"></param>
        /// <param name="ExecutionDay"></param>
        /// <param name="URL"></param>
        /// <param name="HBCIVersion"></param>
        /// <param name="UserID"></param>
        /// <param name="PIN"></param>
        /// <param name="HIRMS"></param>
        /// <param name="Anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string CollectiveTransfer_Terminated(int BLZ, string AccountHolder, string AccountHolderIBAN, string AccountHolderBIC, List<pain00100203_ct_data> PainData,
            string NumberofTransactions, decimal TotalAmount, string ExecutionDay, string URL, int HBCIVersion,
            string UserID, string PIN, string HIRMS, bool Anonymous)
        {
            if (Transaction.INI(BLZ, URL, HBCIVersion, UserID, PIN, Anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCME(BLZ, AccountHolder, AccountHolderIBAN, AccountHolderBIC, PainData, NumberofTransactions,
                    Convert.ToDecimal(TotalAmount), ExecutionDay, URL, HBCIVersion, UserID, PIN);

                if (BankCode.Contains("+0030::"))
                {
                    var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

                    String[] values = BankCode_.Split('+');

                    string msg = string.Empty;

                    foreach (var item in values)
                    {
                        if (!item.StartsWith("HIRMS"))
                            TransactionConsole.Output = item.Replace("::", ": ");
                    }

                    var HITAN = "HITAN" + Helper.Parse_String(BankCode.Replace("?'", "").Replace("?:", ":").Replace("<br>", Environment.NewLine).Replace("?+", "??"), "'HITAN", "'");

                    string HITANFlicker = string.Empty;

                    String[] values_ = HITAN.Split('+');

                    int i = 1;

                    foreach (var item in values_)
                    {
                        i = i + 1;

                        if (i == 6)
                            TransactionConsole.Output = TransactionConsole.Output + "??" + item.Replace("::", ": ").TrimStart();
                    }

                    return "OK";
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
        /// Rebook money from one to another account
        /// </summary>
        /// <param name="BLZ"></param>
        /// <param name="AccountHolder"></param>
        /// <param name="AccountHolderIBAN"></param>
        /// <param name="AccountHolderBIC"></param>
        /// <param name="Receiver"></param>
        /// <param name="ReceiverIBAN"></param>
        /// <param name="ReceiverBIC"></param>
        /// <param name="Amount"></param>
        /// <param name="Purpose"></param>
        /// <param name="URL"></param>
        /// <param name="HBCIVersion"></param>
        /// <param name="UserID"></param>
        /// <param name="PIN"></param>
        /// <param name="HIRMS"></param>
        /// <param name="Anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string Rebooking(int BLZ, string AccountHolder, string AccountHolderIBAN, string AccountHolderBIC, string Receiver, string ReceiverIBAN, string ReceiverBIC,
            string Amount, string Purpose, string URL, int HBCIVersion, string UserID, string PIN,
            string HIRMS, bool Anonymous)
        {
            if (Transaction.INI(BLZ, URL, HBCIVersion, UserID, PIN, Anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCUM(BLZ, AccountHolder, AccountHolderIBAN, AccountHolderBIC, Receiver, ReceiverIBAN, ReceiverBIC,
                    Convert.ToDecimal(Amount), Purpose, URL, HBCIVersion, UserID, PIN);

                if (BankCode.Contains("+0030::"))
                {
                    var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

                    String[] values = BankCode_.Split('+');

                    string msg = string.Empty;

                    foreach (var item in values)
                    {
                        if (!item.StartsWith("HIRMS"))
                            TransactionConsole.Output = item.Replace("::", ": ");
                    }

                    var HITAN = "HITAN" + Helper.Parse_String(BankCode.Replace("?'", "").Replace("?:", ":").Replace("<br>", Environment.NewLine).Replace("?+", "??"), "'HITAN", "'");

                    string HITANFlicker = string.Empty;

                    String[] values_ = HITAN.Split('+');

                    int i = 1;

                    foreach (var item in values_)
                    {
                        i = i + 1;

                        if (i == 6)
                            TransactionConsole.Output = TransactionConsole.Output + "??" + item.Replace("::", ": ").TrimStart();
                    }

                    return "OK";
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
        /// Collect money from another account
        /// </summary>
        /// <param name="BLZ"></param>
        /// <param name="AccountHolder"></param>
        /// <param name="AccountHolderIBAN"></param>
        /// <param name="AccountHolderBIC"></param>
        /// <param name="Payer"></param>
        /// <param name="PayerIBAN"></param>
        /// <param name="PayerBIC"></param>
        /// <param name="Amount"></param>
        /// <param name="Purpose"></param>
        /// <param name="SettlementDate"></param>
        /// <param name="MandateNumber"></param>
        /// <param name="MandateDate"></param>
        /// <param name="CeditorIDNumber"></param>
        /// <param name="URL"></param>
        /// <param name="HBCIVersion"></param>
        /// <param name="UserID"></param>
        /// <param name="PIN"></param>
        /// <param name="HIRMS"></param>
        /// <param name="Anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string Collect(int BLZ, string AccountHolder, string AccountHolderIBAN, string AccountHolderBIC, string Payer, string PayerIBAN, string PayerBIC,
            decimal Amount, string Purpose, string SettlementDate, string MandateNumber, string MandateDate, string CeditorIDNumber, string URL, int HBCIVersion, string UserID,
            string PIN, string HIRMS, bool Anonymous)
        {			
            if (Transaction.INI(BLZ, URL, HBCIVersion, UserID, PIN, Anonymous) == true)
            {
				TransactionConsole.Output = string.Empty;

				if (!String.IsNullOrEmpty(HIRMS))
					Segment.HIRMS = HIRMS;
				
                var BankCode = Transaction.HKDSE(BLZ, AccountHolder, AccountHolderIBAN, AccountHolderBIC, Payer, PayerIBAN, PayerBIC,
                    Convert.ToDecimal(Amount), Purpose, SettlementDate, MandateNumber, MandateDate, CeditorIDNumber, URL, HBCIVersion, UserID, PIN);

                if (BankCode.Contains("+0030::"))
                {
                    var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

                    String[] values = BankCode_.Split('+');

                    foreach (var item in values)
                    {
                        if (!item.StartsWith("HIRMS"))
							TransactionConsole.Output = item.Replace("::", ": ");
                    }
                    
					var HITAN = "HITAN" + Helper.Parse_String(BankCode.Replace("?'", "").Replace("?:", ":").Replace("<br>", Environment.NewLine).Replace("?+", "??"), "'HITAN", "'");

                    string HITANFlicker = string.Empty;

                    String[] values_ = HITAN.Split('+');

                    int i = 1;

                    foreach (var item in values_)
                    {
                        i = i + 1;

                        if (i == 6)
                            TransactionConsole.Output = TransactionConsole.Output + "??" + item.Replace("::", ": ").TrimStart();
                    }

                    return "OK";
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
        /// Collective collect money from other accounts
        /// </summary>
        /// <param name="BLZ"></param>
        /// <param name="AccountHolder"></param>
        /// <param name="AccountHolderIBAN"></param>
        /// <param name="AccountHolderBIC"></param>
        /// <param name="SettlementDate"></param>
        /// <param name="PainData"></param>
        /// <param name="NumberofTransactions"></param>
        /// <param name="TotalAmount"></param>
        /// <param name="URL"></param>
        /// <param name="HBCIVersion"></param>
        /// <param name="UserID"></param>
        /// <param name="PIN"></param>
        /// <param name="HIRMS"></param>
        /// <param name="Anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string CollectiveCollect(int BLZ, string AccountHolder, string AccountHolderIBAN, string AccountHolderBIC, string SettlementDate, List<pain00800202_cc_data> PainData,
            string NumberofTransactions, decimal TotalAmount, string URL, int HBCIVersion, string UserID, string PIN,
            string HIRMS, bool Anonymous)
        {
            if (Transaction.INI(BLZ, URL, HBCIVersion, UserID, PIN, Anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKDME(BLZ, AccountHolder, AccountHolderIBAN, AccountHolderBIC, SettlementDate, PainData, NumberofTransactions,
                    Convert.ToDecimal(TotalAmount), URL, HBCIVersion, UserID, PIN);

                if (BankCode.Contains("+0030::"))
                {
                    var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

                    String[] values = BankCode_.Split('+');

                    foreach (var item in values)
                    {
                        if (!item.StartsWith("HIRMS"))
                            TransactionConsole.Output = item.Replace("::", ": ");
                    }

                    var HITAN = "HITAN" + Helper.Parse_String(BankCode.Replace("?'", "").Replace("?:", ":").Replace("<br>", Environment.NewLine).Replace("?+", "??"), "'HITAN", "'");

                    string HITANFlicker = string.Empty;

                    String[] values_ = HITAN.Split('+');

                    int i = 1;

                    foreach (var item in values_)
                    {
                        i = i + 1;

                        if (i == 6)
                            TransactionConsole.Output = TransactionConsole.Output + "??" + item.Replace("::", ": ").TrimStart();
                    }

                    return "OK";
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
        /// Load mobile phone prepaid card
        /// </summary>
        /// <param name="BLZ"></param>
        /// <param name="IBAN"></param>
        /// <param name="BIC"></param>
        /// <param name="MobileServiceProvider"></param>
        /// <param name="PhoneNumber"></param>
        /// <param name="Amount"></param>
        /// <param name="URL"></param>
        /// <param name="HBCIVersion"></param>
        /// <param name="UserID"></param>
        /// <param name="PIN"></param>
        /// <param name="HIRMS"></param>
        /// <param name="Anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string Prepaid(int BLZ, string IBAN, string BIC, int MobileServiceProvider, string PhoneNumber, int Amount, string URL, int HBCIVersion, string UserID, string PIN,
            string HIRMS, bool Anonymous)
        {
            if (Transaction.INI(BLZ, URL, HBCIVersion, UserID, PIN, Anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKPPD(BLZ, IBAN, BIC, MobileServiceProvider, PhoneNumber, Amount, URL, HBCIVersion, UserID, PIN);

                if (BankCode.Contains("+0030::"))
                {
                    var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

                    String[] values = BankCode_.Split('+');

                    string msg = string.Empty;

                    foreach (var item in values)
                    {
                        if (!item.StartsWith("HIRMS"))
                            TransactionConsole.Output = item.Replace("::", ": ");
                    }

                    var HITAN = "HITAN" + Helper.Parse_String(BankCode.Replace("?'", "").Replace("?:", ":").Replace("<br>", Environment.NewLine).Replace("?+", "??"), "'HITAN", "'");

                    string HITANFlicker = string.Empty;

                    String[] values_ = HITAN.Split('+');

                    int i = 1;

                    foreach (var item in values_)
                    {
                        i = i + 1;

                        if (i == 6)
                            TransactionConsole.Output = TransactionConsole.Output + "??" + item.Replace("::", ": ").TrimStart();
                    }

                    return "OK";
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
        /// Submit bankers order
        /// </summary>
        /// <param name="BLZ"></param>
        /// <param name="AccountHolder"></param>
        /// <param name="AccountHolderIBAN"></param>
        /// <param name="AccountHolderBIC"></param>
        /// <param name="Receiver"></param>
        /// <param name="ReceiverIBAN"></param>
        /// <param name="ReceiverBIC"></param>
        /// <param name="Amount"></param>
        /// <param name="Purpose"></param>
        /// <param name="FirstTimeExecutionDay"></param>
        /// <param name="TimeUnit"></param>
        /// <param name="Rota"></param>
        /// <param name="ExecutionDay"></param>
        /// <param name="URL"></param>
        /// <param name="HBCIVersion"></param>
        /// <param name="UserID"></param>
        /// <param name="PIN"></param>
        /// <param name="HIRMS"></param>
        /// <param name="Anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string SubmitBankersOrder(int BLZ, string AccountHolder, string AccountHolderIBAN, string AccountHolderBIC,
            string Receiver, string ReceiverIBAN, string ReceiverBIC, string Amount, string Purpose, string FirstTimeExecutionDay,
            string TimeUnit, string Rota, string ExecutionDay, string URL, int HBCIVersion, string UserID, string PIN, string HIRMS,
            bool Anonymous)
        {
            if (Transaction.INI(BLZ, URL, HBCIVersion, UserID, PIN, Anonymous) == true)
            {
                TransactionConsole.Output = string.Empty;

                if (!String.IsNullOrEmpty(HIRMS))
                    Segment.HIRMS = HIRMS;

                var BankCode = Transaction.HKCDE(BLZ, AccountHolder, AccountHolderIBAN, AccountHolderBIC, Receiver, ReceiverIBAN, ReceiverBIC,
                    Convert.ToDecimal(Amount), Purpose, FirstTimeExecutionDay, TimeUnit, Rota, ExecutionDay, URL, HBCIVersion, UserID, PIN);

                if (BankCode.Contains("+0030::"))
                {
                    var BankCode_ = "HIRMS" + Helper.Parse_String(BankCode, "'HIRMS", "'");

                    String[] values = BankCode_.Split('+');

                    string msg = string.Empty;

                    foreach (var item in values)
                    {
                        if (!item.StartsWith("HIRMS"))
                            TransactionConsole.Output = item.Replace("::", ": ");
                    }

                    var HITAN = "HITAN" + Helper.Parse_String(BankCode.Replace("?'", "").Replace("?:", ":").Replace("<br>", Environment.NewLine).Replace("?+", "??"), "'HITAN", "'");

                    string HITANFlicker = string.Empty;

                    String[] values_ = HITAN.Split('+');

                    int i = 1;

                    foreach (var item in values_)
                    {
                        i = i + 1;

                        if (i == 6)
                            TransactionConsole.Output = TransactionConsole.Output + "??" + item.Replace("::", ": ").TrimStart();
                    }

                    return "OK";
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
        /// Get banker's orders
        /// </summary>
        /// <param name="BLZ"></param>
        /// <param name="IBAN"></param>
        /// <param name="BIC"></param>
        /// <param name="URL"></param>
        /// <param name="HBCIVersion"></param>
        /// <param name="UserID"></param>
        /// <param name="PIN"></param>
        /// <param name="Anonymous"></param>
        /// <returns>
        /// Banker's orders
        /// </returns>
        public static string GetBankersOrders(int BLZ, string IBAN, string BIC, string URL, int HBCIVersion,
            string UserID, string PIN, bool Anonymous)
        {
            if (Transaction.INI(BLZ, URL, HBCIVersion, UserID, PIN, Anonymous) == true)
            {
                // Success
                var BankCode = Transaction.HKCSB(BLZ, IBAN, BIC, URL, HBCIVersion, UserID, PIN);

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
        /// <param name="TAN"></param>
        /// <param name="URL"></param>
        /// <param name="HBCIVersion"></param>
        /// <param name="BLZ"></param>
        /// <param name="UserID"></param>
        /// <param name="PIN"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string TAN(string TAN, string URL, int HBCIVersion, int BLZ, string UserID, string PIN)
        {			
            var BankCode = Transaction.TAN(TAN, URL, HBCIVersion, BLZ, UserID, PIN);

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
        /// <param name="TAN"></param>
        /// <param name="URL"></param>
        /// <param name="HBCIVersion"></param>
        /// <param name="BLZ"></param>
        /// <param name="UserID"></param>
        /// <param name="PIN"></param>
        /// <param name="MediumName"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public static string TAN4(string TAN, string URL, int HBCIVersion, int BLZ, string UserID, string PIN, string MediumName)
        {
            var BankCode = Transaction.TAN4(TAN, URL, HBCIVersion, BLZ, UserID, PIN, MediumName);

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
        /// Set assembly informations
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
    }
}