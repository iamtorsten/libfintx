/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (c) 2016 - 2020 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 * 	
 * 	libfintx is free software; you can redistribute it and/or
 *	modify it under the terms of the GNU Lesser General Public
 * 	License as published by the Free Software Foundation; either
 * 	version 2.1 of the License, or (at your option) any later version.
 *	
 * 	libfintx is distributed in the hope that it will be useful,
 * 	but WITHOUT ANY WARRANTY; without even the implied warranty of
 * 	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * 	Lesser General Public License for more details.
 *	
 * 	You should have received a copy of the GNU Lesser General Public
 * 	License along with libfintx; if not, write to the Free Software
 * 	Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 * 	
 */

using libfintx.Util;
using System.Collections.Generic;
using System.Linq;

namespace libfintx
{
    public class AccountInformation
    {
        public string AccountIban { get; set; }
        public string AccountNumber { get; set; }
        public string AccountBankCode { get; set; }
        public string AccountUserId { get; set; }
        public string AccountOwner { get; set; }
        public string AccountType { get; set; }
        public string AccountCurrency { get; set; }
        public string AccountBic { get; set; }

        public List<AccountPermission> AccountPermissions { get; set; }

        public bool IsSegmentPermitted(string segment)
        {
            return AccountPermissions.Any(a => a.Segment == segment);
        }

        public override string ToString()
        {
            return ReflectionUtil.ToString(this);
        }
    }
}
