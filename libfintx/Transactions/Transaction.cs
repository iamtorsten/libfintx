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
    public static class Transaction
    {
        /// <summary>
        /// INI
        /// </summary>
        public static bool INI(int BLZ, string URL, int HBCIVersion, string UserID, string PIN, bool Anonymous)
        {
            if (!Anonymous)
            {
                /// <summary>
                /// Sync
                /// </summary>
                try
                {
                    Log.Write("Starting Synchronisation");

                    string segments;

                    if (HBCIVersion == 220)
                    {
                        string segments_ = "HKIDN:3:2+280:" + BLZ + "+" + UserID + "+0+1'" +
                            "HKVVB:4:2+0+0+0+" + Program.Buildname + "+" + Program.Version + "'" +
                            "HKSYN:5:2+0'";

                        segments = segments_;
                    }
                    else if (HBCIVersion == 300)
                    {
                        string segments_ = "HKIDN:3:2+280:" + BLZ + "+" + UserID + "+0+1'" +
                            "HKVVB:4:3+0+0+0+" + Program.Buildname + "+" + Program.Version + "'" +
                            "HKSYN:5:3+0'";

                        segments = segments_;
                    }
                    else
                    {
                        UserID = string.Empty;
                        PIN = null;

                        Log.Write("HBCI version not supported");

                        throw new Exception("HBCI version not supported");
                    }

                    if (Helper.Parse_Segment(UserID, BLZ, HBCIVersion, FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, "1", "0", BLZ, UserID, PIN, "0", segments, null, 5))))
                    {
                        // Sync OK
                        Log.Write("Synchronisation ok");

                        /// <summary>
                        /// INI
                        /// </summary>
                        if (HBCIVersion == 220)
                        {
                            string segments_ = "HKIDN:3:2+280:" + BLZ + "+" + UserID + "+" + Segment.HISYN + "+1'" +
                                "HKVVB:4:2+0+0+0+" + Program.Buildname + "+" + Program.Version + "'";

                            segments = segments_;
                        }
                        else if (HBCIVersion == 300)
                        {
                            string segments_ = "HKIDN:3:2+280:" + BLZ + "+" + UserID + "+" + Segment.HISYN + "+1'" +
                                "HKVVB:4:3+0+0+0+" + Program.Buildname + "+" + Program.Version + "'";

                            segments = segments_;
                        }
                        else
                        {
                            UserID = string.Empty;
                            PIN = null;

                            Log.Write("HBCI version not supported");

                            throw new Exception("HBCI version not supported");
                        }

                        if (Helper.Parse_Segment(UserID, BLZ, HBCIVersion, FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, "1", "0", BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS, 4))))
                            return true;
                        else
                        {
                            UserID = string.Empty;
                            PIN = null;

                            Log.Write("Initialisation failed");

                            throw new Exception("Initialisation failed");
                        }
                    }
                    else
                    {
                        UserID = string.Empty;
                        PIN = null;

                        Log.Write("Sync failed");

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    UserID = string.Empty;
                    PIN = null;

                    Log.Write(ex.ToString());

                    throw new Exception("Software error");
                }
            }
            else
            {
                /// <summary>
                /// Sync
                /// </summary>
                try
                {
                    Log.Write("Starting Synchronisation anonymous");

                    string segments;

                    if (HBCIVersion == 300)
                    {
                        string segments_ = "HKIDN:2:2+280:" + BLZ + "+" + "9999999999" + "+0+0'" +
                                    "HKVVB:3:3+0+0+1+" + Program.Buildname + "+" + Program.Version + "'";

                        segments = segments_;
                    }
                    else
                    {
                        UserID = string.Empty;
                        PIN = null;

                        Log.Write("HBCI version not supported");

                        throw new Exception("HBCI version not supported");
                    }

                    if (Helper.Parse_Segment(UserID, BLZ, HBCIVersion, FinTSMessage.Send(URL, FinTSMessageAnonymous.Create(HBCIVersion, "1", "0", BLZ, UserID, PIN, "0", segments, null, 4))))
                    {
                        // Sync OK
                        Log.Write("Synchronisation anonymous ok");

                        /// <summary>
                        /// INI
                        /// </summary>
                        if (HBCIVersion == 300)
                        {
                            string segments__ = "HKIDN:3:2+280:" + BLZ + "+" + UserID + "+" + Segment.HISYN + "+1'" +
                                "HKVVB:4:3+0+0+0+" + Program.Buildname + "+" + Program.Version + "'" +
                                "HKSYN:5:3+0'";

                            segments = segments__;
                        }
                        else
                        {
                            UserID = string.Empty;
                            PIN = null;

                            Log.Write("HBCI version not supported");

                            throw new Exception("HBCI version not supported");
                        }

                        if (Helper.Parse_Segment(UserID, BLZ, HBCIVersion, FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, "1", "0", BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS, 5))))
                        {
                            return true;
                        }
                        else
                        {
                            UserID = string.Empty;
                            PIN = null;

                            Log.Write("Initialisation failed");

                            throw new Exception("Initialisation failed");
                        }
                    }
                    else
                    {
                        UserID = string.Empty;
                        PIN = null;

                        Console.WriteLine("FALSE");

                        Log.Write("Sync failed");

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    UserID = string.Empty;
                    PIN = null;

                    Log.Write(ex.ToString());

                    throw new Exception("Software error");
                }
            }
        }

        /// <summary>
        /// Balance
        /// </summary>
        public static string HKSAL(int Konto, int BLZ, string IBAN, string BIC, string URL, int HBCIVersion, string UserID, string PIN)
        {
            Log.Write("Starting job HKSAL: Request balance");

            string segments = string.Empty;

            if (Convert.ToInt16(Segment.HISALS) >= 7)
                segments = "HKSAL:3:" + Segment.HISALS + "+" + IBAN + ":" + BIC + "+N'";
            else
            {
                segments = "HKSAL:3:" + Segment.HISALS + "+" + Konto + "::280:" + BLZ + "+N'";
            }

            return FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS, 3));
        }

        /// <summary>
        /// Transactions
        /// </summary>
        public static string HKKAZ(int Konto, int BLZ, string IBAN, string BIC, string URL, int HBCIVersion, string UserID, string PIN, string FromDate, string Startpoint)
        {
            Log.Write("Starting job HKKAZ: Request transactions");

            string segments = string.Empty;

            if (String.IsNullOrEmpty(FromDate))
            {
                if (String.IsNullOrEmpty(Startpoint))
                {
                    if (Convert.ToInt16(Segment.HKKAZ) < 7)
                        segments = "HKKAZ:3:" + Segment.HKKAZ + "+" + Konto + "::280:" + BLZ + "+N'";
                    else
                        segments = "HKKAZ:3:" + Segment.HKKAZ + "+" + IBAN + ":" + BIC + ":" + Konto + "::280:" + BLZ + "+N'";
                }
                else
                {
                    if (Convert.ToInt16(Segment.HKKAZ) < 7)
                        segments = "HKKAZ:3:" + Segment.HKKAZ + "+" + Konto + "::280:" + BLZ + "+N++++" + Startpoint + "'";
                    else
                        segments = "HKKAZ:3:" + Segment.HKKAZ + "+" + IBAN + ":" + BIC + ":" + Konto + "::280:" + BLZ + "+N++++" + Startpoint + "'";
                }
            }
            else
            {
                if (String.IsNullOrEmpty(Startpoint))
                {
                    if (Convert.ToInt16(Segment.HKKAZ) < 7)
                        segments = "HKKAZ:3:" + Segment.HKKAZ + "+" + Konto + "::280:" + BLZ + "+N+" + FromDate + "'";
                    else
                        segments = "HKKAZ:3:" + Segment.HKKAZ +"+" + IBAN + ":" + BIC + ":" + Konto + "::280:" + BLZ + "+N+" + FromDate + "'";
                }
                else
                {
                    if (Convert.ToInt16(Segment.HKKAZ) < 7)
                        segments = "HKKAZ:3:" + Segment.HKKAZ + "+" + Konto + "::280:" + BLZ + "+N+" + FromDate + "+++" + Startpoint + "'";
                    else
                        segments = "HKKAZ:3:" + Segment.HKKAZ + "+" + IBAN + ":" + BIC + ":" + Konto + "::280:" + BLZ + "+N+" + FromDate + "+++" + Startpoint + "'";
                }
            }

            return FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS, 3));
        }

        /// <summary>
        /// Transfer
        /// </summary>
        public static string HKCCS(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, string URL, int HBCIVersion, string UserID, string PIN)
        {
            Log.Write("Starting job HKCCS: Transfer money");

            string segments = "HKCCS:3:1+" + AccountholderIBAN + ":" + AccountholderBIC + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.002.03+@@";

            var message = pain00100203.Create(Accountholder, AccountholderIBAN, AccountholderBIC, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, "1999-01-01");

            segments = segments.Replace("@@", "@" + (message.Length - 1) + "@") + message;

            segments = segments + "HKTAN:4:" + Segment.HITANS + "'";

            var TAN = FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS, 4));
            
            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(TAN, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(TAN);

            return TAN;
        }

        /// <summary>
        /// Transfer terminated
        /// </summary>
        public static string HKCCSt(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, string ExecutionDay, string URL, int HBCIVersion, string UserID, string PIN)
        {
            Log.Write("Starting job HKCCS: Transfer money terminated");

            string segments = "HKCCS:3:1+" + AccountholderIBAN + ":" + AccountholderBIC + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.002.03+@@";

            var message = pain00100203.Create(Accountholder, AccountholderIBAN, AccountholderBIC, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, ExecutionDay);

            segments = segments.Replace("@@", "@" + (message.Length - 1) + "@") + message;

            segments = segments + "HKTAN:4:" + Segment.HITANS + "'";

            var TAN = FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS, 4));

            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(TAN, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(TAN);

            return TAN;
        }

        /// <summary>
        /// Collective transfer
        /// </summary>
        public static string HKCCM(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC, List<pain00100203_ct_data> PainData, string NumberofTransactions, decimal TotalAmount, string URL, int HBCIVersion, string UserID, string PIN)
        {
            Log.Write("Starting job HKCCM: Collective transfer money");

            var TotalAmount_ = TotalAmount.ToString().Replace(",", ".");

            string segments = "HKCCM:3:1+" + AccountholderIBAN + ":" + AccountholderBIC + TotalAmount_ + ":EUR++" + " + urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.002.03+@@";

            var message = pain00100203.Create(Accountholder, AccountholderIBAN, AccountholderBIC, PainData, NumberofTransactions, TotalAmount, "1999-01-01");

            segments = segments.Replace("@@", "@" + (message.Length - 1) + "@") + message;

            segments = segments + "HKTAN:4:" + Segment.HITANS + "'";

            var TAN = FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS, 4));

            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(TAN, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(TAN);

            return TAN;
        }

        /// <summary>
        /// Collective transfer terminated
        /// </summary>
        public static string HKCME(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC, List<pain00100203_ct_data> PainData, string NumberofTransactions, decimal TotalAmount, string ExecutionDay, string URL, int HBCIVersion, string UserID, string PIN)
        {
            Log.Write("Starting job HKCME: Collective transfer money terminated");

            var TotalAmount_ = TotalAmount.ToString().Replace(",", ".");

            string segments = "HKCME:3:1+" + AccountholderIBAN + ":" + AccountholderBIC + TotalAmount_ + ":EUR++" + " + urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.002.03+@@";

            var message = pain00100203.Create(Accountholder, AccountholderIBAN, AccountholderBIC, PainData, NumberofTransactions, TotalAmount, ExecutionDay);

            segments = segments.Replace("@@", "@" + (message.Length - 1) + "@") + message;

            segments = segments + "HKTAN:4:" + Segment.HITANS + "'";

            var TAN = FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS, 4));

            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(TAN, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(TAN);

            return TAN;
        }

        /// <summary>
        /// Rebooking
        /// </summary>
        public static string HKCUM(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, string URL, int HBCIVersion, string UserID, string PIN)
        {
            Log.Write("Starting job HKCUM: Rebooking money");

            string segments = "HKCUM:3:1+" + AccountholderIBAN + ":" + AccountholderBIC + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.002.03+@@";

            var message = pain00100203.Create(Accountholder, AccountholderIBAN, AccountholderBIC, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, "1999-01-01");

            segments = segments.Replace("@@", "@" + (message.Length - 1) + "@") + message;

            segments = segments + "HKTAN:4:" + Segment.HITANS + "'";

            var TAN = FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS, 4));

            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(TAN, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(TAN);

            return TAN;
        }

        /// <summary>
        /// Collect
        /// </summary>
        public static string HKDSE(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC, string Payer, string PayerIBAN, string PayerBIC, decimal Amount, string Usage, string SettlementDate, string MandateNumber, string MandateDate, string CeditorIDNumber, string URL, int HBCIVersion, string UserID, string PIN)
        {
            Log.Write("Starting job HKDSE: Collect money");

            string segments = "HKDSE:3:1+" + AccountholderIBAN + ":" + AccountholderBIC + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.002.02+@@";

            var message = pain00800202.Create(Accountholder, AccountholderIBAN, AccountholderBIC, Payer, PayerIBAN, PayerBIC, Amount, Usage, SettlementDate, MandateNumber, MandateDate, CeditorIDNumber);

            segments = segments.Replace("@@", "@" + (message.Length - 1) + "@") + message;

            segments = segments + "HKTAN:4:" + Segment.HITANS + "'";

            var TAN = FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS, 4));

            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(TAN, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(TAN);

            return TAN;
        }

        /// <summary>
        /// Collective collect
        /// </summary>
        public static string HKDME(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC, string SettlementDate, List<pain00800202_cc_data> PainData, string NumberofTransactions, decimal TotalAmount, string URL, int HBCIVersion, string UserID, string PIN)
        {
            Log.Write("Starting job HKDME: Collective collect money");

            var TotalAmount_ = TotalAmount.ToString().Replace(",", ".");

            string segments = "HKDME:3:2+" + AccountholderIBAN + ":" + AccountholderBIC + TotalAmount_ + ":EUR++" + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.002.02+@@";

            var message = pain00800202.Create(Accountholder, AccountholderIBAN, AccountholderBIC, SettlementDate, PainData, NumberofTransactions, TotalAmount);

            segments = segments.Replace("@@", "@" + (message.Length - 1) + "@") + message;

            segments = segments + "HKTAN:4:" + Segment.HITANS + "'";

            var TAN = FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS, 4));

            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(TAN, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(TAN);

            return TAN;
        }

        /// <summary>
        /// Load prepaid
        /// </summary>
        public static string HKPPD(int BLZ, string IBAN, string BIC, int MobileServiceProvider, string PhoneNumber, int Amount, string URL, int HBCIVersion, string UserID, string PIN)
        {
            Log.Write("Starting job HKPPD: Load prepaid");

            string segments = "HKPPD:3:2+" + IBAN + ":" +BIC + "+" + MobileServiceProvider + "+" + PhoneNumber + "+" + Amount + ",:EUR'";

            segments = segments + "HKTAN:4:" + Segment.HITANS + "'";

            var TAN = FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS, 4));

            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(TAN, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(TAN);

            return TAN;
        }

        /// <summary>
        /// Submit bankers order
        /// </summary>
        public static string HKCDE(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, string FirstTimeExecutionDay, string TimeUnit, string Rota, string ExecutionDay, string URL, int HBCIVersion, string UserID, string PIN)
        {
            Log.Write("Starting job HKCDE: Submit bankers order");

            string segments = "HKCDE:3:1+" + AccountholderIBAN + ":" + AccountholderBIC + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.002.03+@@";

            var message = pain00100203.Create(Accountholder, AccountholderIBAN, AccountholderBIC, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, "1999-01-01");

            message = message.Replace("'", "") + "+" + FirstTimeExecutionDay + ":" + TimeUnit + ":" + Rota + ":" + ExecutionDay + "'";

            segments = segments.Replace("@@", "@" + (message.Length - 1) + "@") + message;

            segments = segments + "HKTAN:4:" + Segment.HITANS + "'";

            var TAN = FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS, 4));

            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(TAN, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(TAN);

            return TAN;
        }

        /// <summary>
        /// Get bankers orders
        /// </summary>
        public static string HKCSB(int BLZ, string IBAN, string BIC, string URL, int HBCIVersion, string UserID, string PIN)
        {
            Log.Write("Starting job HKCSB: Get bankers order");

            string segments = "HKCSB:3:1+" + IBAN + ":" + BIC + "+sepade?:xsd?:pain.001.001.03.xsd'";

            return FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS, 3));
        }

        /// <summary>
        /// TAN
        /// </summary>
        public static string TAN(string TAN, string URL, int HBCIVersion, int BLZ, string UserID, string PIN)
        {
            Log.Write("Starting TAN process");

            string segments = string.Empty;

            // Version 2, Process 2
            if (Segment.HITANS.Substring(0, 1).Equals("2+2"))
                segments = "HKTAN:3:" + Segment.HITANS.Substring(0, 1) + "+2++" + Segment.HITAN + "++N'";
            // Version 3, Process 2
            if (Segment.HITANS.Substring(0, 1).Equals("3+2"))
                segments = "HKTAN:3:" + Segment.HITANS.Substring(0, 1) + "+2++" + Segment.HITAN + "++N'";
            // Version 4, Process 2
            if (Segment.HITANS.Substring(0, 1).Equals("4+2"))
                segments = "HKTAN:3:" + Segment.HITANS.Substring(0, 1) + "+2++" + Segment.HITAN + "++N'";
            // Version 5, Process 2
            if (Segment.HITANS.Substring(0, 1).Equals("5+2"))
                segments = "HKTAN:3:" + Segment.HITANS.Substring(0, 1) + "+2++++" + Segment.HITAN + "++N'";
            else
                segments = "HKTAN:3:" + Segment.HITANS.Substring(0, 1) + "+2++++" + Segment.HITAN + "++N'";

            return FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS + ":" + TAN, 3));
        }

        /// <summary>
        /// TAN process 4
        /// </summary>
        public static string TAN4(string TAN, string URL, int HBCIVersion, int BLZ, string UserID, string PIN, string MediumName)
        {
            Log.Write("Starting job TAN process 4");

            string segments = string.Empty;

            // Version 3, Process 4
            if (Segment.HITANS.Substring(0, 1).Equals("3+4"))
                segments = "HKTAN:3:" + Segment.HITANS.Substring(0, 1) + "+4+++++++" + MediumName + "'";
            // Version 4, Process 4
            if (Segment.HITANS.Substring(0, 1).Equals("4+4"))
                segments = "HKTAN:3:" + Segment.HITANS.Substring(0, 1) + "+4++++++++" + MediumName + "'";
            // Version 5, Process 4
            if (Segment.HITANS.Substring(0, 1).Equals("5+4"))
                segments = "HKTAN:3:" + Segment.HITANS.Substring(0, 1) + "+4++++++++++" + MediumName + "'";

            return FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS + ":" + TAN, 3));
        }
    }
}
