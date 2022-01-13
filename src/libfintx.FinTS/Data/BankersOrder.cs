/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2021 - 2022 Abid Hussain
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
using libfintx.Sepa;
using static libfintx.FinTS.HKCDE;

namespace libfintx.FinTS.Data
{
    public class BankersOrder
    {
        public string OrderId { get; set; }
        public Pain00100103CtData SepaData { get; set; }
        public DateTime FirstExecutionDate { get; set; }
        public TimeUnit TimeUnit { get; set; }
        public string Rota { get; set; }
        public int ExecutionDay { get; set; }
        public DateTime? LastExecutionDate { get; set; }

        public BankersOrder(string orderId, Pain00100103CtData sepaData, DateTime firstExecutionDate, TimeUnit timeUnit, string rota, int executionDay, DateTime? lastExecutionDate)
        {
            OrderId = orderId;
            SepaData = sepaData;
            FirstExecutionDate = firstExecutionDate;
            TimeUnit = timeUnit;
            Rota = rota;
            ExecutionDay = executionDay;
            LastExecutionDate = lastExecutionDate;
        }

        public BankersOrder(Pain00100103CtData sepaData, DateTime firstExecutionDate, TimeUnit timeUnit, string rota, int executionDay, DateTime? lastExecutionDate)
        {
            SepaData = sepaData;
            FirstExecutionDate = firstExecutionDate;
            TimeUnit = timeUnit;
            Rota = rota;
            ExecutionDay = executionDay;
            LastExecutionDate = lastExecutionDate;
        }

        public override bool Equals(object obj)
        {
            return obj is BankersOrder order &&
                   OrderId == order.OrderId;
        }

        public override int GetHashCode()
        {
            return 755918762 + EqualityComparer<string>.Default.GetHashCode(OrderId);
        }
    }
}
