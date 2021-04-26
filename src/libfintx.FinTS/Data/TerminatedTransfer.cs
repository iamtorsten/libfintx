/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2021 Abid Hussain
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
    public class TerminatedTransfer
    {
        public string OrderId { get; set; }
        public bool? Deleteable { get; set; }
        public bool? Modifiable { get; set; }
        public Pain00100103CtData SepaData { get; set; }

        public TerminatedTransfer(string orderId, bool? deleteable, bool? modifiable, Pain00100103CtData sepaData)
        {
            OrderId = orderId;
            Deleteable = deleteable;
            Modifiable = modifiable;
            SepaData = sepaData;
        }

        public override bool Equals(object obj)
        {
            return obj is TerminatedTransfer transfer &&
                   OrderId == transfer.OrderId;
        }

        public override int GetHashCode()
        {
            return 755918762 + EqualityComparer<string>.Default.GetHashCode(OrderId);
        }
    }
}
