using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static libfintx.HKCDE;

namespace libfintx.Data
{
    public class BankersOrder
    {
        public string OrderId { get; set; }
        public pain00100103_ct_data SepaData { get; set; }
        public DateTime FirstExecutionDate { get; set; }
        public TimeUnit TimeUnit { get; set; }
        public string Rota { get; set; }
        public int ExecutionDay { get; set; }
        public DateTime? LastExecutionDate { get; set; }

        public BankersOrder(string orderId, pain00100103_ct_data sepaData, DateTime firstExecutionDate, TimeUnit timeUnit, string rota, int executionDay, DateTime? lastExecutionDate)
        {
            OrderId = orderId;
            SepaData = sepaData;
            FirstExecutionDate = firstExecutionDate;
            TimeUnit = timeUnit;
            Rota = rota;
            ExecutionDay = executionDay;
            LastExecutionDate = lastExecutionDate;
        }
        
        public BankersOrder(pain00100103_ct_data sepaData, DateTime firstExecutionDate, TimeUnit timeUnit, string rota, int executionDay, DateTime? lastExecutionDate)
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
            var order = obj as BankersOrder;
            return order != null &&
                   OrderId == order.OrderId &&
                   EqualityComparer<pain00100103_ct_data>.Default.Equals(SepaData, order.SepaData) &&
                   FirstExecutionDate == order.FirstExecutionDate &&
                   TimeUnit == order.TimeUnit &&
                   Rota == order.Rota &&
                   ExecutionDay == order.ExecutionDay &&
                   LastExecutionDate == order.LastExecutionDate;
        }
        public override int GetHashCode()
        {
            var hashCode = 434083080;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(OrderId);
            hashCode = hashCode * -1521134295 + EqualityComparer<pain00100103_ct_data>.Default.GetHashCode(SepaData);
            hashCode = hashCode * -1521134295 + FirstExecutionDate.GetHashCode();
            hashCode = hashCode * -1521134295 + TimeUnit.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Rota);
            hashCode = hashCode * -1521134295 + ExecutionDay.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<DateTime?>.Default.GetHashCode(LastExecutionDate);
            return hashCode;
        }
    }
}