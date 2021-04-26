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

using System;
using System.Runtime.Serialization;

namespace libfintx.EBICS.Exceptions
{
    public class RecoverySyncException: EbicsException
    {
        public RecoverySyncException()
        {
        }

        public RecoverySyncException(string message) : base(message)
        {
        }

        public RecoverySyncException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RecoverySyncException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
