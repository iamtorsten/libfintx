/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2016 - 2021 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 *  
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *  
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU Affero General Public License for more details.
 *  
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program. If not, see <http://www.gnu.org/licenses/>.
 * 	
 */

using libfintx.Camt;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static libfintx.HKCAZ;
using static libfintx.HKCCM;
using static libfintx.HKCCS;
using static libfintx.HKCDB;
using static libfintx.HKCDE;
using static libfintx.HKCDL;
using static libfintx.HKCDN;
using static libfintx.HKCME;
using static libfintx.HKCSL;
using static libfintx.HKCSA;
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
using static libfintx.Tan;
using static libfintx.Tan4;

namespace libfintx
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
