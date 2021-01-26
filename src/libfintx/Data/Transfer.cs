using System;
using System.Collections.Generic;
using static libfintx.HKCDE;

namespace libfintx.Data
{
    public class Transfer
    {
        public string OrderId { get; set; }
        public bool? Deleteable { get; set; }
        public bool? Modifiable { get; set; }
        public Pain00100103CtData SepaData { get; set; }

        public Transfer(string orderId, bool? deleteable, bool? modifiable, Pain00100103CtData sepaData)
        {
            OrderId = orderId;
            Deleteable = deleteable;
            Modifiable = modifiable;
            SepaData = sepaData;
        }

        public override bool Equals(object obj)
        {
            return obj is Transfer transfer &&
                   OrderId == transfer.OrderId;
        }

        public override int GetHashCode()
        {
            return 755918762 + EqualityComparer<string>.Default.GetHashCode(OrderId);
        }
    }
}
