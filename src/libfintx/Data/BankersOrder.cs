/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2021 Abid Hussain
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
using static libfintx.HKCDE;

namespace libfintx.Data
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
