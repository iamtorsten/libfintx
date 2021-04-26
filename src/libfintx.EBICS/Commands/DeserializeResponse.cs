/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2018 Bjoern Kuensting
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

using libfintx.EBICS.Handler;
using Newtonsoft.Json;

namespace libfintx.EBICS.Commands
{
    internal class DeserializeResponse
    {       
        public int NumSegments { get; set; }
        public int SegmentNumber { get; set; }
        public bool LastSegment { get; set; }
        public string TransactionId { get; set; }
        public TransactionPhase Phase { get; set; }
        public int TechnicalReturnCode { get; set; }
        public int BusinessReturnCode { get; set; }
        public string ReportText { get; set; }

        public bool HasError =>
            ((TechnicalReturnCode != ReturnCodes.TechnicalCodeOK &&
              TechnicalReturnCode != ReturnCodes.TechnicalCodePostProcessDone &&
              TechnicalReturnCode != ReturnCodes.TechnicalCodePostProcessSkipped &&
              TechnicalReturnCode != ReturnCodes.TechnicalCodeRecoverySync) ||
             BusinessReturnCode != ReturnCodes.BusinessCodeOK);

        public bool IsRecoverySync =>
            (TechnicalReturnCode == ReturnCodes.TechnicalCodeRecoverySync);
        
        
        static DeserializeResponse()
        {

        }

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
    
    
}
