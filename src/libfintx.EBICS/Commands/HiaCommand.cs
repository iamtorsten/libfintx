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
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using libfintx.EBICS.Exceptions;
using libfintx.EBICS.Parameters;
using libfintx.EBICS.Responses;
using libfintx.Xml;

namespace libfintx.EBICS.Commands
{
    internal class HiaCommand : GenericCommand<HiaResponse>
    {
        private static readonly ILogger s_logger = EbicsLogging.CreateLogger<HiaCommand>();
        
        internal HiaParams Params { private get; set; }
        internal override string OrderType => "HIA";
        internal override string OrderAttribute => "DZNNN";
        internal override TransactionType TransactionType => TransactionType.Upload;
        internal override IList<XmlDocument> Requests => CreateRequests();
        internal override XmlDocument InitRequest => null;
        internal override XmlDocument ReceiptRequest => null;

        private IList<XmlDocument> CreateRequests()
        {
            using (new MethodLogger(s_logger))
            {
                try
                {
                    var reqs = new List<XmlDocument>();

                    var hiaOrderData = new HiaRequestOrderData
                    {
                        Namespaces = Namespaces,
                        PartnerId = Config.User.PartnerId,
                        UserId = Config.User.UserId,
                        AuthInfo = new AuthenticationPubKeyInfo
                        {
                            Namespaces = Namespaces,
                            AuthKeys = Config.User.AuthKeys,
                            UseEbicsDefaultNamespace = true
                        },
                        EncInfo = new EncryptionPubKeyInfo
                        {
                            Namespaces = Namespaces,
                            CryptKeys = Config.User.CryptKeys,
                            UseEbicsDefaultNamespace = true
                        }
                    };

                    var compressed =
                        Compress(Encoding.UTF8.GetBytes(
                            hiaOrderData.Serialize().ToString(SaveOptions.DisableFormatting)));
                    var b64Encoded = Convert.ToBase64String(compressed);

                    var req = new EbicsUnsecuredRequest
                    {
                        StaticHeader = new StaticHeader
                        {
                            HostId = Config.User.HostId,
                            PartnerId = Config.User.PartnerId,
                            UserId = Config.User.UserId,
                            SecurityMedium = Params.SecurityMedium,
                            Namespaces = Namespaces,
                            OrderDetails = new OrderDetails
                            {
                                OrderType = OrderType,
                                OrderAttribute = OrderAttribute,
                                Namespaces = Namespaces
                            }
                        },
                        MutableHeader = new MutableHeader
                        {
                            Namespaces = Namespaces
                        },
                        Body = new Body
                        {
                            Namespaces = Namespaces,
                            DataTransfer = new DataTransfer
                            {
                                OrderData = b64Encoded,
                                Namespaces = Namespaces
                            }
                        },
                        Namespaces = Namespaces,
                        Version = Config.Version,
                        Revision = Config.Revision,
                    };

                    reqs.Add(req.Serialize().ToXmlDocument());
                    return reqs;
                }
                catch (EbicsException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new CreateRequestException($"can't create requests for {OrderType}", ex);
                }
            }
        }
    }
}
