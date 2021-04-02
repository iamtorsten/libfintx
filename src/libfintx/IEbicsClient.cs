/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2018 Bjoern Kuensting
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

using libfintx.Config;
using libfintx.Parameters;
using libfintx.Responses;

namespace libfintx
{
    public interface IEbicsClient
    {
        EbicsConfig Config { get; set; }
        HpbResponse HPB(HpbParams p);
        PtkResponse PTK(PtkParams p);
        StaResponse STA(StaParams p);
        CctResponse CCT(CctParams p);
        IniResponse INI(IniParams p);
        HiaResponse HIA(HiaParams p);
        SprResponse SPR(SprParams p);
        CddResponse CDD(CddParams p);
    }
}
