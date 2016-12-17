/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 * 	
 * 	libfintx is free software; you can redistribute it and/or
 *	modify it under the terms of the GNU Lesser General Public
 * 	License as published by the Free Software Foundation; either
 * 	version 2.1 of the License, or (at your option) any later version.
 *	
 * 	libfintx is distributed in the hope that it will be useful,
 * 	but WITHOUT ANY WARRANTY; without even the implied warranty of
 * 	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * 	Lesser General Public License for more details.
 *	
 * 	You should have received a copy of the GNU Lesser General Public
 * 	License along with libfintx; if not, write to the Free Software
 * 	Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 * 	
 */

using System;

namespace libfintx
{
    public static class Transaction
    {
        /// <summary>
        /// INI
        /// </summary>
        public static bool INI(int BLZ, string URL, int HBCIVersion, int UserID, string PIN)
        {
            /// <summary>
            /// Sync
            /// </summary>
            try
            {
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
                    UserID = 0;
                    PIN = null;

                    throw new Exception("HBCI version not supported");
                }

                if (Helper.Parse_Segment(UserID, BLZ, HBCIVersion, FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, "1", "0", BLZ, UserID, PIN, "0", segments, null))))
                {
                    // Sync OK

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
                        UserID = 0;
                        PIN = null;

                        throw new Exception ("HBCI version not supported");
                    }

                    if (Helper.Parse_Segment(UserID, BLZ, HBCIVersion, FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, "1", "0", BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS))))
                        return true;
                    else
                    {
                        UserID = 0;
                        PIN = null;

                        throw new Exception ("Initialisation failed");
                    }
                }
                else
                {
                    UserID = 0;
                    PIN = null;

                    return false;
                }
            }
            catch
            {
                UserID = 0;
                PIN = null;

                throw new Exception("Software error");
            }
        }

        /// <summary>
        /// Balance
        /// </summary>
        public static string HKSAL(int Konto, int BLZ, string IBAN, string BIC, string URL, int HBCIVersion, int UserID, string PIN)
        {
            string segments = string.Empty;

            if (Convert.ToInt16(Segment.HISALS) >= 7)
                segments = "HKSAL:3:" + Segment.HISALS + "+" + IBAN + ":" + BIC + "+N'";
            else
            {
                segments = "HKSAL:3:" + Segment.HISALS + "+" + Konto + "::280:" + BLZ + "+N'";
            }

            return FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS));
        }

        /// <summary>
        /// Transactions
        /// </summary>
        public static string HKKAZ(int Konto, int BLZ, string IBAN, string BIC, string URL, int HBCIVersion, int UserID, string PIN, string FromDate, string Startpoint)
        {
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

            return FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS));
        }

        /// <summary>
        /// Transfer
        /// </summary>
        public static string HKCCS(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, string URL, int HBCIVersion, int UserID, string PIN)
        {
            string segments = "HKCCS:3:1+" + AccountholderIBAN + ":" + AccountholderBIC + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.002.03+@@";

            var message = pain00100203.Create(Accountholder, AccountholderIBAN, AccountholderBIC, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, "1999-01-01");

            segments = segments.Replace("@@", "@" + (message.Length - 1) + "@") + message;

            segments = segments + "HKTAN:4:" + Segment.HITANS + "'";

            var TAN = FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS));
            
            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(TAN, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(TAN);

            return TAN;
        }

        /// <summary>
        /// Transfer terminated
        /// </summary>
        public static string HKCCSt(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, string ExecutionDay, string URL, int HBCIVersion, int UserID, string PIN)
        {
            string segments = "HKCCS:3:1+" + AccountholderIBAN + ":" + AccountholderBIC + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.002.03+@@";

            var message = pain00100203.Create(Accountholder, AccountholderIBAN, AccountholderBIC, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, ExecutionDay);

            segments = segments.Replace("@@", "@" + (message.Length - 1) + "@") + message;

            segments = segments + "HKTAN:4:" + Segment.HITANS + "'";

            var TAN = FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS));

            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(TAN, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(TAN);

            return TAN;
        }

        /// <summary>
        /// Collect
        /// </summary>
        public static string HKDSE(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC, string Payer, string PayerIBAN, string PayerBIC, decimal Amount, string Usage, string SettlementDate, string MandateNumber, string MandateDate, string CeditorIDNumber, string URL, int HBCIVersion, int UserID, string PIN)
        {
            string segments = "HKDSE:3:1+" + AccountholderIBAN + ":" + AccountholderBIC + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.002.02+@@";

            var message = pain00800202.Create(Accountholder, AccountholderIBAN, AccountholderBIC, Payer, PayerIBAN, PayerBIC, Amount, Usage, SettlementDate, MandateNumber, MandateDate, CeditorIDNumber);

            segments = segments.Replace("@@", "@" + (message.Length - 1) + "@") + message;

            segments = segments + "HKTAN:4:" + Segment.HITANS + "'";

            var TAN = FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS));

            Segment.HITAN = Helper.Parse_String(Helper.Parse_String(TAN, "HITAN", "'").Replace("?+", "??"), "++", "+").Replace("??", "?+");

            Helper.Parse_Message(TAN);

            return TAN;
        }

        /// <summary>
        /// TAN
        /// </summary>
        public static string TAN(string TAN, string URL, int HBCIVersion, int BLZ, int UserID, string PIN)
        {
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

            return FinTSMessage.Send(URL, FinTSMessage.Create(HBCIVersion, Segment.HNHBS, Segment.HNHBK, BLZ, UserID, PIN, Segment.HISYN, segments, Segment.HIRMS + ":" + TAN));
        }
    }
}
