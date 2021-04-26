/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2016 - 2021 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 *  
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 3 of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software Foundation,
 *  Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 * 	
 */

using libfintx.FinTS.Camt;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static libfintx.FinTS.HKCAZ;
using static libfintx.FinTS.HKCCM;
using static libfintx.FinTS.HKCCS;
using static libfintx.FinTS.HKCDB;
using static libfintx.FinTS.HKCDE;
using static libfintx.FinTS.HKCDL;
using static libfintx.FinTS.HKCDN;
using static libfintx.FinTS.HKCME;
using static libfintx.FinTS.HKCSL;
using static libfintx.FinTS.HKCSA;
using static libfintx.FinTS.HKCSB;
using static libfintx.FinTS.HKCSE;
using static libfintx.FinTS.HKCUM;
using static libfintx.FinTS.HKDME;
using static libfintx.FinTS.HKDSE;
using static libfintx.FinTS.HKEND;
using static libfintx.FinTS.HKKAZ;
using static libfintx.FinTS.HKPPD;
using static libfintx.FinTS.HKSAL;
using static libfintx.FinTS.HKSYN;
using static libfintx.FinTS.HKTAB;
using static libfintx.FinTS.INI;
using static libfintx.FinTS.Tan;
using static libfintx.FinTS.Tan4;
using libfintx.Sepa;

namespace libfintx.FinTS
{
    public static class Transaction
    {
        public static async Task<String> INI(FinTsClient client, string hkTanSegmentId = null)
        {
            return await Init_INI(client, hkTanSegmentId);
        }

        public static async Task<String> HKEND(FinTsClient client, string dialogId)
        {
            return await Init_HKEND(client, dialogId);
        }

        public static async Task<String> HKSYN(FinTsClient client)
        {
            return await Init_HKSYN(client);
        }

        public static async Task<String> HKSAL(FinTsClient client)
        {
            return await Init_HKSAL(client);
        }

        public static async Task<String> HKKAZ(FinTsClient client, string FromDate, string ToDate, string Startpoint)
        {
            return await Init_HKKAZ(client, FromDate, ToDate, Startpoint);
        }

        public static async Task<String> HKCCS(FinTsClient client, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage)
        {
            return await Init_HKCCS(client, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage);
        }

        public static async Task<String> HKCSE(FinTsClient client, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime ExecutionDay)
        {
            return await Init_HKCSE(client, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, ExecutionDay);
        }

        public static async Task<String> HKCCM(FinTsClient client, List<Pain00100203CtData> PainData, string NumberofTransactions, decimal TotalAmount)
        {
            return await Init_HKCCM(client, PainData, NumberofTransactions, TotalAmount);
        }

        public static async Task<String> HKCME(FinTsClient client, List<Pain00100203CtData> PainData, string NumberofTransactions, decimal TotalAmount, DateTime ExecutionDay)
        {
            return await Init_HKCME(client, PainData, NumberofTransactions, TotalAmount, ExecutionDay);
        }

        public static async Task<String> HKCUM(FinTsClient client, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage)
        {
            return await Init_HKCUM(client, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage);
        }

        public static async Task<String> HKDSE(FinTsClient client, string Payer, string PayerIBAN, string PayerBIC, decimal Amount, string Usage,
            DateTime SettlementDate, string MandateNumber, DateTime MandateDate, string CeditorIDNumber)
        {
            return await Init_HKDSE(client, Payer, PayerIBAN, PayerBIC, Amount, Usage, SettlementDate, MandateNumber, MandateDate, CeditorIDNumber);
        }

        public static async Task<String> HKDME(FinTsClient client, DateTime SettlementDate, List<Pain00800202CcData> PainData, string NumberofTransactions, decimal TotalAmount)
        {
            return await Init_HKDME(client, SettlementDate, PainData, NumberofTransactions, TotalAmount);
        }

        public static async Task<String> HKPPD(FinTsClient client, int MobileServiceProvider, string PhoneNumber, int Amount)
        {
            return await Init_HKPPD(client, MobileServiceProvider, PhoneNumber, Amount);
        }

        public static async Task<String> HKCDE(FinTsClient client, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime FirstTimeExecutionDay, TimeUnit TimeUnit, string Rota, int ExecutionDay, DateTime? LastExecutionDay)
        {
            return await Init_HKCDE(client, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, FirstTimeExecutionDay, TimeUnit, Rota, ExecutionDay, LastExecutionDay);
        }

        public static async Task<String> HKCDN(FinTsClient client, string OrderId, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime FirstTimeExecutionDay, TimeUnit TimeUnit, string Rota, int ExecutionDay, DateTime? LastExecutionDay)
        {
            return await Init_HKCDN(client, OrderId, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, FirstTimeExecutionDay, TimeUnit, Rota, ExecutionDay, LastExecutionDay);
        }

        public static async Task<String> HKCDL(FinTsClient client, string OrderId, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime FirstTimeExecutionDay, TimeUnit TimeUnit, string Rota, int ExecutionDay, DateTime? LastExecutionDay)
        {
            return await Init_HKCDL(client, OrderId, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, FirstTimeExecutionDay, TimeUnit, Rota, ExecutionDay, LastExecutionDay);
        }

        public static async Task<String> HKCSB(FinTsClient client)
        {
            return await Init_HKCSB(client);
        }

        public static async Task<String> HKCSL(FinTsClient client, string OrderId, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime ExecutionDay)
        {
            return await Init_HKCSL(client, OrderId, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, ExecutionDay);
        }

        public static async Task<String> HKCSA(FinTsClient client, string OrderId, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime ExecutionDay)
        {
            return await Init_HKCSA(client, OrderId, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, ExecutionDay);
        }

        public static async Task<String> HKCDB(FinTsClient client)
        {
            return await Init_HKCDB(client);
        }

        public static async Task<String> TAN(FinTsClient client, string TAN)
        {
            return await Send_TAN(client, TAN);
        }

        public static async Task<String> TAN4(FinTsClient client, string TAN, string MediumName)
        {
            return await Send_TAN4(client, TAN, MediumName);
        }

        public static async Task<String> HKTAB(FinTsClient client)
        {
            return await Init_HKTAB(client);
        }

        public static async Task<String> HKCAZ(FinTsClient client, string FromDate, string ToDate, string Startpoint, CamtVersion camtVers)
        {
            return await Init_HKCAZ(client, FromDate, ToDate, Startpoint, camtVers);
        }
    }
}
