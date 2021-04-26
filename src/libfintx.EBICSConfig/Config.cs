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

using Newtonsoft.Json;

namespace libfintx.EBICSConfig
{
    public class Config
    {
        
        
        public string Address { get; set; }
        public bool TLS { get; set; }
        public bool Insecure { get; set; }
        public UserParams User { get; set; }
        public BankParams Bank { get; set; }
        public EbicsVersion Version { get; set; } = EbicsVersion.H004;
        public EbicsRevision Revision { get; set; } = EbicsRevision.Rev1;


        static Config()
        {

        }

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
