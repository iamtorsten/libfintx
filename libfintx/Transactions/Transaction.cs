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

using System.Collections.Generic;

using static libfintx.INI;
using static libfintx.HKSAL;
using static libfintx.HKKAZ;
using static libfintx.HKCCS;
using static libfintx.HKCCST;
using static libfintx.HKCCM;
using static libfintx.HKCME;
using static libfintx.HKCUM;
using static libfintx.HKDSE;
using static libfintx.HKDME;
using static libfintx.HKPPD;
using static libfintx.HKCDE;
using static libfintx.HKCSB;
using static libfintx.TAN;
using static libfintx.TAN4;

using static libfintx.INI_RDH;

namespace libfintx
{
    public static class Transaction
    {
        public static bool INI(int BLZ, string URL, int HBCIVersion, string UserID, string PIN, bool Anonymous)
        {
            return Init_INI(BLZ, URL, HBCIVersion, UserID, PIN, Anonymous);
        }

        public static string HKSAL(int Konto, int BLZ, string IBAN, string BIC, string URL, int HBCIVersion, string UserID, string PIN)
        {
            return Init_HKSAL(Konto, BLZ, IBAN, BIC, URL, HBCIVersion, UserID, PIN);
        }

        public static string HKKAZ(int Konto, int BLZ, string IBAN, string BIC, string URL, int HBCIVersion, string UserID,
            string PIN, string FromDate, string Startpoint)
        {
            return Init_HKKAZ(Konto, BLZ, IBAN, BIC, URL, HBCIVersion, UserID, PIN, FromDate, Startpoint);
        }

        public static string HKCCS(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC, string Receiver,
            string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, string URL, int HBCIVersion, string UserID, string PIN)
        {
            return Init_HKCCS(BLZ, Accountholder, AccountholderIBAN, AccountholderBIC, Receiver,
                ReceiverIBAN, ReceiverBIC, Amount, Usage, URL, HBCIVersion, UserID, PIN);
        }

        public static string HKCCST(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC,
            string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, string ExecutionDay,
            string URL, int HBCIVersion, string UserID, string PIN)
        {
            return Init_HKCCST(BLZ, Accountholder, AccountholderIBAN, AccountholderBIC, Receiver, ReceiverIBAN, ReceiverBIC, Amount,
                Usage, ExecutionDay, URL, HBCIVersion, UserID, PIN);
        }

        public static string HKCCM(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC,
            List<pain00100203_ct_data> PainData, string NumberofTransactions, decimal TotalAmount, string URL, int HBCIVersion,
            string UserID, string PIN)
        {
            return Init_HKCCM(BLZ, Accountholder, AccountholderIBAN, AccountholderBIC, PainData, NumberofTransactions, TotalAmount, URL, HBCIVersion,
                UserID, PIN);
        }

        public static string HKCME(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC,
            List<pain00100203_ct_data> PainData, string NumberofTransactions, decimal TotalAmount, string ExecutionDay,
            string URL, int HBCIVersion, string UserID, string PIN)
        {
            return Init_HKCME(BLZ, Accountholder, AccountholderIBAN, AccountholderBIC, PainData, NumberofTransactions, TotalAmount, ExecutionDay,
                URL, HBCIVersion, UserID, PIN);
        }

        public static string HKCUM(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC, string Receiver,
            string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, string URL, int HBCIVersion, string UserID, string PIN)
        {
            return Init_HKCUM(BLZ, Accountholder, AccountholderIBAN, AccountholderBIC, Receiver, ReceiverIBAN, ReceiverBIC,
                Amount, Usage, URL, HBCIVersion, UserID, PIN);
        }

        public static string HKDSE(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC,
            string Payer, string PayerIBAN, string PayerBIC, decimal Amount, string Usage, string SettlementDate, string MandateNumber,
            string MandateDate, string CeditorIDNumber, string URL, int HBCIVersion, string UserID, string PIN)
        {
            return Init_HKDSE(BLZ, Accountholder, AccountholderIBAN, AccountholderBIC, Payer, PayerIBAN, PayerBIC, Amount, Usage,
                SettlementDate, MandateNumber, MandateDate, CeditorIDNumber, URL, HBCIVersion, UserID, PIN);
        }

        public static string HKDME(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC,
           string SettlementDate, List<pain00800202_cc_data> PainData, string NumberofTransactions, decimal TotalAmount,
           string URL, int HBCIVersion, string UserID, string PIN)
        {
            return Init_HKDME(BLZ, Accountholder, AccountholderIBAN, AccountholderBIC, SettlementDate, PainData, NumberofTransactions, TotalAmount, 
                URL, HBCIVersion, UserID, PIN);
        }

        public static string HKPPD(int BLZ, string IBAN, string BIC, int MobileServiceProvider, string PhoneNumber, int Amount,
             string URL, int HBCIVersion, string UserID, string PIN)
        {
            return Init_HKPPD(BLZ, IBAN, BIC, MobileServiceProvider, PhoneNumber, Amount, URL, HBCIVersion, UserID, PIN);
        }

        public static string HKCDE(int BLZ, string Accountholder, string AccountholderIBAN, string AccountholderBIC, string Receiver,
            string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, string FirstTimeExecutionDay, string TimeUnit, string Rota,
            string ExecutionDay, string URL, int HBCIVersion, string UserID, string PIN)
        {
            return Init_HKCDE(BLZ, Accountholder, AccountholderIBAN, AccountholderBIC, Receiver, ReceiverIBAN, ReceiverBIC, Amount,
                Usage, FirstTimeExecutionDay, TimeUnit, Rota, ExecutionDay, URL, HBCIVersion, UserID, PIN);
        }

        public static string HKCSB(int BLZ, string IBAN, string BIC, string URL, int HBCIVersion, string UserID, string PIN)
        {
            return Init_HKCSB(BLZ, IBAN, BIC, URL, HBCIVersion, UserID, PIN);
        }

        public static string TAN(string TAN, string URL, int HBCIVersion, int BLZ, string UserID, string PIN)
        {
            return Send_TAN(TAN, URL, HBCIVersion, BLZ, UserID, PIN);
        }

        public static string TAN4(string TAN, string URL, int HBCIVersion, int BLZ, string UserID, string PIN, string MediumName)
        {
            return Send_TAN4(TAN, URL, HBCIVersion, BLZ, UserID, PIN, MediumName);
        }

        // RDH
        public static bool INI_RDH(int BLZ, string URL, int Port, int HBCIVersion, string UserID, string FilePath, string Password)
        {
            return Init_INI_RDH(BLZ, URL, Port, HBCIVersion, UserID, FilePath, Password);
        }
    }
}
