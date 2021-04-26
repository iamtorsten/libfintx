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
using libfintx.EBICS.Handler;
using libfintx.EBICS.Parameters;
using libfintx.EBICS.Responses;
using libfintx.Xml;

namespace libfintx.EBICS.Commands
{
    internal class PtkCommand : GenericCommand<PtkResponse>
    {
        private static readonly ILogger s_logger = EbicsLogging.CreateLogger<PtkCommand>();
        private string _transactionId;

        internal PtkParams Params { private get; set; }
        internal override TransactionType TransactionType => TransactionType.Download;
        internal override IList<XmlDocument> Requests => null;
        internal override XmlDocument InitRequest => CreateInitRequest();
        internal override XmlDocument ReceiptRequest => CreateReceiptRequest();
        internal override string OrderType => "PTK";
        internal override string OrderAttribute => "DZHNN";

        internal override DeserializeResponse Deserialize(string payload)
        {
            using (new MethodLogger(s_logger))
            {
                try
                {
                    var dr = base.Deserialize(payload);
                    var doc = XDocument.Parse(payload);
                    var xph = new XPathHelper(doc, Namespaces);
                    var sb = new StringBuilder();
                    
                    if (dr.HasError || dr.IsRecoverySync)
                    {
                        return dr;
                    }

                    // do signature validation here

                    if (dr.Phase != TransactionPhase.Initialisation)
                    {
                        return dr;
                    }

                    sb.Append(Response.Data ?? "").Append(Encoding.UTF8.GetString(Decompress(DecryptOrderData(xph))));
                    Response.Data = sb.ToString();
                    _transactionId = dr.TransactionId;

                    return dr;
                }
                catch (EbicsException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new DeserializationException($"can't deserialize {OrderType} command", ex, payload);
                }
            }
        }

        private XmlDocument CreateReceiptRequest()
        {
            try
            {
                var receiptReq = new EbicsRequest
                {
                    Version = Config.Version,
                    Revision = Config.Revision,
                    Namespaces = Namespaces,
                    StaticHeader = new StaticHeader
                    {
                        Namespaces = Namespaces,
                        HostId = Config.User.HostId,
                        TransactionId = _transactionId
                    },
                    MutableHeader = new MutableHeader
                    {
                        Namespaces = Namespaces,
                        TransactionPhase = "Receipt"
                    },
                    Body = new Body
                    {
                        Namespaces = Namespaces,
                        TransferReceipt = new TransferReceipt
                        {
                            Namespaces = Namespaces,
                            ReceiptCode = "0"
                        }
                    }
                };

                return AuthenticateXml(receiptReq.Serialize().ToXmlDocument(), null, null);
            }
            catch (EbicsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CreateRequestException($"can't create receipt request for {OrderType}", ex);
            }
        }

        private XmlDocument CreateInitRequest()
        {
            using (new MethodLogger(s_logger))
            {
                try
                {
                    var initReq = new EbicsRequest
                    {
                        StaticHeader = new StaticHeader
                        {
                            Namespaces = Namespaces,
                            HostId = Config.User.HostId,
                            PartnerId = Config.User.PartnerId,
                            UserId = Config.User.UserId,
                            SecurityMedium = Params.SecurityMedium,
                            Nonce = CryptoUtils.GetNonce(),
                            Timestamp = CryptoUtils.GetUtcTimeNow(),
                            BankPubKeyDigests = new BankPubKeyDigests
                            {
                                Namespaces = Namespaces,
                                Bank = Config.Bank,
                                DigestAlgorithm = s_digestAlg
                            },
                            OrderDetails = new OrderDetails
                            {
                                Namespaces = Namespaces,
                                OrderAttribute = OrderAttribute,
                                OrderType = OrderType,
                                StandardOrderParams = new StartEndDateOrderParams
                                {
                                    Namespaces = Namespaces,
                                    StartDate = Params.StartDate,
                                    EndDate = Params.EndDate
                                }
                            }
                        },
                        MutableHeader = new MutableHeader
                        {
                            Namespaces = Namespaces,
                            TransactionPhase = "Initialisation"
                        },
                        Body = new Body
                        {
                            Namespaces = Namespaces
                        },
                        Namespaces = Namespaces,
                        Version = Config.Version,
                        Revision = Config.Revision,
                    };

                    return AuthenticateXml(initReq.Serialize().ToXmlDocument(), null, null);
                }
                catch (EbicsException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new CreateRequestException($"can't create init request for {OrderType}", ex);
                }
            }
        }
    }
}
