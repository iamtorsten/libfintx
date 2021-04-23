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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using libfintx.Camt;
using libfintx.Data;
using libfintx.Swift;

namespace libfintx
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

        Task<HBCIDialogResult<List<AccountInformation>>> Accounts(TANDialog tanDialog);
        Task<HBCIDialogResult<AccountBalance>> Balance(TANDialog tanDialog);
        Task<HBCIDialogResult> Collect(TANDialog tanDialog, string payerName, string payerIBAN, string payerBIC, decimal amount, string purpose, DateTime settlementDate, string mandateNumber, DateTime mandateDate, string creditorIdNumber, string hirms);
        Task<HBCIDialogResult> CollectiveCollect(TANDialog tanDialog, DateTime settlementDate, List<Pain00800202CcData> painData, string numberOfTransactions, decimal totalAmount, string hirms);
        Task<HBCIDialogResult> CollectiveTransfer(TANDialog tanDialog, List<Pain00100203CtData> painData, string numberOfTransactions, decimal totalAmount, string hirms);
        Task<HBCIDialogResult> CollectiveTransfer_Terminated(TANDialog tanDialog, List<Pain00100203CtData> painData, string numberOfTransactions, decimal totalAmount, DateTime executionDay, string hirms);
        Task<HBCIDialogResult> DeleteBankersOrder(TANDialog tanDialog, string orderId, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota, int executionDay, DateTime? lastExecutionDay, string hirms);
        Task<HBCIDialogResult> DeleteTerminatedTransfer(TANDialog tanDialog, string orderId, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string usage, DateTime executionDay, string hirms);
        Task<HBCIDialogResult<List<BankersOrder>>> GetBankersOrders(TANDialog tanDialog);
        Task<HBCIDialogResult<List<TerminatedTransfer>>> GetTerminatedTransfers(TANDialog tanDialog);
        Task<HBCIDialogResult> ModifyBankersOrder(TANDialog tanDialog, string OrderId, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota, int executionDay, DateTime? lastExecutionDay, string hirms);
        Task<HBCIDialogResult> ModifyTerminatedTransfer(TANDialog tanDialog, string orderId, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string usage, DateTime executionDay, string hirms);
        Task<HBCIDialogResult> Prepaid(TANDialog tanDialog, int mobileServiceProvider, string phoneNumber, int amount, string hirms);
        Task<HBCIDialogResult> Rebooking(TANDialog tanDialog, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose, string hirms);
        Task<HBCIDialogResult<List<string>>> RequestTANMediumName();
        Task<HBCIDialogResult> SubmitBankersOrder(TANDialog tanDialog, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota, int executionDay, DateTime? lastExecutionDay, string hirms);
        Task<HBCIDialogResult<string>> Synchronization();
        Task<HBCIDialogResult> TAN(string TAN);
        Task<HBCIDialogResult> TAN4(string TAN, string MediumName);
        Task<HBCIDialogResult<List<SwiftStatement>>> Transactions(TANDialog tanDialog, DateTime? startDate = null, DateTime? endDate = null, bool saveMt940File = false);
        Task<HBCIDialogResult<List<AccountTransaction>>> TransactionsSimple(TANDialog tanDialog, DateTime? startDate = null, DateTime? endDate = null);
        Task<HBCIDialogResult<List<CamtStatement>>> Transactions_camt(TANDialog tanDialog, CamtVersion camtVers, DateTime? startDate = null, DateTime? endDate = null, bool saveCamtFile = false);
        Task<HBCIDialogResult> Transfer(TANDialog tanDialog, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose, string hirms);
        Task<HBCIDialogResult> Transfer_Terminated(TANDialog tanDialog, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose, DateTime executionDay, string hirms);
    }
}