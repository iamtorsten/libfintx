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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using libfintx.FinTS.Camt;
using libfintx.FinTS.Data;
using libfintx.FinTS.Swift;
using libfintx.Sepa;

namespace libfintx.FinTS
{
    public interface IFinTsClient
    {
        AccountInformation activeAccount { get; set; }
        bool Anonymous { get; }
        ConnectionDetails ConnectionDetails { get; }
        string HIRMS { get; set; }
        string HITAB { get; set; }
        int HITANS { get; set; }
        string SystemId { get; }

        Task<HBCIDialogResult<List<AccountInformation>>> Accounts(TanRequest tanDialog);
        Task<HBCIDialogResult<AccountBalance>> Balance(TanRequest tanDialog);
        Task<HBCIDialogResult> Collect(TanRequest tanDialog, string payerName, string payerIBAN, string payerBIC, decimal amount, string purpose, DateTime settlementDate, string mandateNumber, DateTime mandateDate, string creditorIdNumber, string hirms);
        Task<HBCIDialogResult> CollectiveCollect(TanRequest tanDialog, DateTime settlementDate, List<Pain00800202CcData> painData, string numberOfTransactions, decimal totalAmount, string hirms);
        Task<HBCIDialogResult> CollectiveTransfer(TanRequest tanDialog, List<Pain00100203CtData> painData, string numberOfTransactions, decimal totalAmount, string hirms);
        Task<HBCIDialogResult> CollectiveTransfer_Terminated(TanRequest tanDialog, List<Pain00100203CtData> painData, string numberOfTransactions, decimal totalAmount, DateTime executionDay, string hirms);
        Task<HBCIDialogResult> DeleteBankersOrder(TanRequest tanDialog, string orderId, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota, int executionDay, DateTime? lastExecutionDay, string hirms);
        Task<HBCIDialogResult> DeleteTerminatedTransfer(TanRequest tanDialog, string orderId, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string usage, DateTime executionDay, string hirms);
        Task<HBCIDialogResult<List<BankersOrder>>> GetBankersOrders(TanRequest tanDialog);
        Task<HBCIDialogResult<List<TerminatedTransfer>>> GetTerminatedTransfers(TanRequest tanDialog);
        Task<HBCIDialogResult> ModifyBankersOrder(TanRequest tanDialog, string OrderId, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota, int executionDay, DateTime? lastExecutionDay, string hirms);
        Task<HBCIDialogResult> ModifyTerminatedTransfer(TanRequest tanDialog, string orderId, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string usage, DateTime executionDay, string hirms);
        Task<HBCIDialogResult> Prepaid(TanRequest tanDialog, int mobileServiceProvider, string phoneNumber, int amount, string hirms);
        Task<HBCIDialogResult> Rebooking(TanRequest tanDialog, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose, string hirms);
        Task<HBCIDialogResult<List<string>>> RequestTANMediumName();
        Task<HBCIDialogResult> SubmitBankersOrder(TanRequest tanDialog, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota, int executionDay, DateTime? lastExecutionDay, string hirms);
        Task<HBCIDialogResult<string>> Synchronization();
        Task<HBCIDialogResult> TAN(string TAN);
        Task<HBCIDialogResult> TAN4(string TAN, string MediumName);
        Task<HBCIDialogResult<List<SwiftStatement>>> Transactions(TanRequest tanDialog, DateTime? startDate = null, DateTime? endDate = null, bool saveMt940File = false);
        Task<HBCIDialogResult<List<AccountTransaction>>> TransactionsSimple(TanRequest tanDialog, DateTime? startDate = null, DateTime? endDate = null);
        Task<HBCIDialogResult<List<CamtStatement>>> Transactions_camt(TanRequest tanDialog, CamtVersion camtVers, DateTime? startDate = null, DateTime? endDate = null, bool saveCamtFile = false);
        Task<HBCIDialogResult> Transfer(TanRequest tanDialog, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose, string hirms);
        Task<HBCIDialogResult> Transfer_Terminated(TanRequest tanDialog, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose, DateTime executionDay, string hirms);
    }
}
