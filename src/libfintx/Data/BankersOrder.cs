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
