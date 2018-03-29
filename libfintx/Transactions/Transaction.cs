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
using libfintx.Data;
using System;

namespace libfintx
{
    public static class Transaction
    {
        public static bool INI(ConnectionDetails connectionDetails, bool Anonymous)
        {
            return Init_INI(connectionDetails, Anonymous);
        }

        public static string HKSAL(ConnectionDetails connectionDetails)
        {
            return Init_HKSAL(connectionDetails);
        }

        public static string HKKAZ(ConnectionDetails connectionDetails, string FromDate, string ToDate, string Startpoint)
        {
            return Init_HKKAZ(connectionDetails, FromDate, ToDate, Startpoint);
        }

        public static string HKCCS(ConnectionDetails connectionDetails, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage)
        {
            return Init_HKCCS(connectionDetails, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage);
        }

        public static string HKCCST(ConnectionDetails connectionDetails, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime ExecutionDay)
        {
            return Init_HKCCST(connectionDetails, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, ExecutionDay);
        }

        public static string HKCCM(ConnectionDetails connectionDetails, List<pain00100203_ct_data> PainData, string NumberofTransactions, decimal TotalAmount)
        {
            return Init_HKCCM(connectionDetails, PainData, NumberofTransactions, TotalAmount);
        }

        public static string HKCME(ConnectionDetails connectionDetails, List<pain00100203_ct_data> PainData, string NumberofTransactions, decimal TotalAmount, DateTime ExecutionDay)
        {
            return Init_HKCME(connectionDetails, PainData, NumberofTransactions, TotalAmount, ExecutionDay);
        }

        public static string HKCUM(ConnectionDetails connectionDetails, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage)
        {
            return Init_HKCUM(connectionDetails, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage);
        }

        public static string HKDSE(ConnectionDetails connectionDetails, string Payer, string PayerIBAN, string PayerBIC, decimal Amount, string Usage, 
            DateTime SettlementDate, string MandateNumber, DateTime MandateDate, string CeditorIDNumber)
        {
            return Init_HKDSE(connectionDetails, Payer, PayerIBAN, PayerBIC, Amount, Usage, SettlementDate, MandateNumber, MandateDate, CeditorIDNumber);
        }

        public static string HKDME(ConnectionDetails connectionDetails, DateTime SettlementDate, List<pain00800202_cc_data> PainData, string NumberofTransactions, decimal TotalAmount)
        {
            return Init_HKDME(connectionDetails, SettlementDate, PainData, NumberofTransactions, TotalAmount);
        }

        public static string HKPPD(ConnectionDetails connectionDetails, int MobileServiceProvider, string PhoneNumber, int Amount)
        {
            return Init_HKPPD(connectionDetails, MobileServiceProvider, PhoneNumber, Amount);
        }

        public static string HKCDE(ConnectionDetails connectionDetails, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime FirstTimeExecutionDay, TimeUnit TimeUnit, string Rota,
            int ExecutionDay)
        {
            return Init_HKCDE(connectionDetails, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, FirstTimeExecutionDay, TimeUnit, Rota, ExecutionDay);
        }

        public static string HKCSB(ConnectionDetails connectionDetails)
        {
            return Init_HKCSB(connectionDetails);
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
