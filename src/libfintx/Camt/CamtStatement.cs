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

/*
 *
 *	Based on Timotheus Pokorra's C# implementation of OpenPetraPlugin_BankimportCAMT
 *	available at https://github.com/SolidCharity/OpenPetraPlugin_BankimportCAMT/blob/master/Client/ParseCAMT.cs
 *
 */

using libfintx.Util;
using System;
using System.Collections.Generic;

namespace libfintx.Camt
{
    public class CamtStatement
    {
        public string Id { get; set; }

        public string ElctrncSeqNb { get; set; }

        public string BankCode { get; set; }

        public string AccountCode { get; set; }

        public string Currency { get; set; }

        public decimal StartBalance { get; set; }

        public decimal EndBalance { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        /// <summary>
        /// Across several years
        /// </summary>
        public bool SeveralYears { get; set; }

        public List<CamtTransaction> Transactions { get; set; }

        public CamtStatement()
        {
            Transactions = new List<CamtTransaction>();
        }

        public override string ToString()
        {
            return ReflectionUtil.ToString(this);
        }
    }
}
