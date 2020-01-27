/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2020 Torsten Klinger
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

using libfintx.Camt;
using System;
using System.Collections.Generic;
using static libfintx.HKCAZ;
using static libfintx.HKCCM;
using static libfintx.HKCCS;
using static libfintx.HKCDB;
using static libfintx.HKCDE;
using static libfintx.HKCDL;
using static libfintx.HKCDN;
using static libfintx.HKCME;
using static libfintx.HKCSB;
using static libfintx.HKCSE;
using static libfintx.HKCUM;
using static libfintx.HKDME;
using static libfintx.HKDSE;
using static libfintx.HKEND;
using static libfintx.HKKAZ;
using static libfintx.HKPPD;
using static libfintx.HKSAL;
using static libfintx.HKSYN;
using static libfintx.HKTAB;
using static libfintx.INI;
/* RDH */
using static libfintx.INI_RDH;
using static libfintx.Tan;
using static libfintx.Tan4;

namespace libfintx
{
    public static class Transaction
    {
        public static string INI(FinTsClient client)
        {
            return Init_INI(client);
        }

        public static string HKEND(FinTsClient client, string dialogId)
        {
            return Init_HKEND(client, dialogId);
        }

        public static string HKSYN(FinTsClient client)
        {
            return Init_HKSYN(client);
        }

        public static string HKSAL(FinTsClient client)
        {
            return Init_HKSAL(client);
        }

        public static string HKKAZ(FinTsClient client, string FromDate, string ToDate, string Startpoint)
        {
            return Init_HKKAZ(client, FromDate, ToDate, Startpoint);
        }

        public static string HKCCS(FinTsClient client, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage)
        {
            return Init_HKCCS(client, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage);
        }

        public static string HKCSE(FinTsClient client, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime ExecutionDay)
        {
            return Init_HKCSE(client, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, ExecutionDay);
        }

        public static string HKCCM(FinTsClient client, List<Pain00100203CtData> PainData, string NumberofTransactions, decimal TotalAmount)
        {
            return Init_HKCCM(client, PainData, NumberofTransactions, TotalAmount);
        }

        public static string HKCME(FinTsClient client, List<Pain00100203CtData> PainData, string NumberofTransactions, decimal TotalAmount, DateTime ExecutionDay)
        {
            return Init_HKCME(client, PainData, NumberofTransactions, TotalAmount, ExecutionDay);
        }

        public static string HKCUM(FinTsClient client, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage)
        {
            return Init_HKCUM(client, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage);
        }

        public static string HKDSE(FinTsClient client, string Payer, string PayerIBAN, string PayerBIC, decimal Amount, string Usage,
            DateTime SettlementDate, string MandateNumber, DateTime MandateDate, string CeditorIDNumber)
        {
            return Init_HKDSE(client, Payer, PayerIBAN, PayerBIC, Amount, Usage, SettlementDate, MandateNumber, MandateDate, CeditorIDNumber);
        }

        public static string HKDME(FinTsClient client, DateTime SettlementDate, List<Pain00800202CcData> PainData, string NumberofTransactions, decimal TotalAmount)
        {
            return Init_HKDME(client, SettlementDate, PainData, NumberofTransactions, TotalAmount);
        }

        public static string HKPPD(FinTsClient client, int MobileServiceProvider, string PhoneNumber, int Amount)
        {
            return Init_HKPPD(client, MobileServiceProvider, PhoneNumber, Amount);
        }

        public static string HKCDE(FinTsClient client, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime FirstTimeExecutionDay, TimeUnit TimeUnit, string Rota, int ExecutionDay, DateTime? LastExecutionDay)
        {
            return Init_HKCDE(client, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, FirstTimeExecutionDay, TimeUnit, Rota, ExecutionDay, LastExecutionDay);
        }

        public static string HKCDN(FinTsClient client, string OrderId, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime FirstTimeExecutionDay, TimeUnit TimeUnit, string Rota, int ExecutionDay, DateTime? LastExecutionDay)
        {
            return Init_HKCDN(client, OrderId, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, FirstTimeExecutionDay, TimeUnit, Rota, ExecutionDay, LastExecutionDay);
        }

        public static string HKCDL(FinTsClient client, string OrderId, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime FirstTimeExecutionDay, TimeUnit TimeUnit, string Rota, int ExecutionDay, DateTime? LastExecutionDay)
        {
            return Init_HKCDL(client, OrderId, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, FirstTimeExecutionDay, TimeUnit, Rota, ExecutionDay, LastExecutionDay);
        }

        public static string HKCSB(FinTsClient client)
        {
            return Init_HKCSB(client);
        }

        public static string HKCDB(FinTsClient client)
        {
            return Init_HKCDB(client);
        }

        public static string TAN(FinTsClient client, string TAN)
        {
            return Send_TAN(client, TAN);
        }

        public static string TAN4(FinTsClient client, string TAN, string MediumName)
        {
            return Send_TAN4(client, TAN, MediumName);
        }

        public static string HKTAB(FinTsClient client)
        {
            return Init_HKTAB(client);
        }

        public static string HKCAZ(FinTsClient client, string FromDate, string ToDate, string Startpoint, CamtVersion camtVers)
        {
            return Init_HKCAZ(client, FromDate, ToDate, Startpoint, camtVers);
        }

        /* RDH */

        public static bool INI_RDH(FinTsClient client, int BLZ, string URL, int Port, int HBCIVersion, string UserID, string FilePath, string Password)
        {
            return Init_INI_RDH(client, BLZ, URL, Port, HBCIVersion, UserID, FilePath, Password);
        }
    }
}
