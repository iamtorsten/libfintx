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

using Newtonsoft.Json;

namespace libfintx.Config
{
    internal class NamespaceConfig
    {
        
        
        internal string Ebics { get; set; }

        internal string EbicsPrefix => "urn";

        internal string XmlDsig { get; set; }

        internal string XmlDsigPrefix => "ds";

        internal string Cct { get; set; }
        internal string Cdd { get; set; }

        internal string SignatureData { get; set; }
        
        static NamespaceConfig()
        {

        }

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
