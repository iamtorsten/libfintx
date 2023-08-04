/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2023 Torsten Klement
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
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace libfintx.EBICS.Commands
{
    internal class C53Command : GenericCommand<C53Response>
    {
        private static readonly ILogger s_logger = EbicsLogging.CreateLogger<C53Command>();
        private string _transactionId;
        private int _numSegments;
        private int _initSegment;
        private bool _initLastSegment;
        private string[] _orderData;

        internal C53Params Params { private get; set; }
        internal override string OrderType => "C53";
        internal override string OrderAttribute => "DZHNN";
        internal override TransactionType TransactionType => TransactionType.Download;
        internal override IList<XmlDocument> Requests => CreateRequests();
        internal override XmlDocument InitRequest => CreateInitRequest();
        internal override XmlDocument ReceiptRequest => CreateReceiptRequest();

        internal override DeserializeResponse Deserialize(string payload)
        {
            using (new MethodLogger(s_logger))
            {
                try
                {
                    var dr = base.Deserialize(payload);
                    var doc = XDocument.Parse(payload);
                    var xph = new XPathHelper(doc, Namespaces);
                    
                    if (dr.HasError || dr.IsRecoverySync)
                    {
                        return dr;
                    }

                    // do signature validation here

                    switch (dr.Phase)
                    {
                        case TransactionPhase.Initialisation:
                            _transactionId = dr.TransactionId;
                            _numSegments = dr.NumSegments;
                            _initSegment = dr.SegmentNumber;
                            _initLastSegment = dr.LastSegment;
                            _orderData = new string[_numSegments];
                            _orderData[dr.SegmentNumber - 1] =
                                Encoding.UTF8.GetString(Decompress(DecryptOrderData(xph)));
                            Response.Data = string.Join("", _orderData);
                            break;
                        case TransactionPhase.Transfer:
                            _orderData[dr.SegmentNumber - 1] =
                                Encoding.UTF8.GetString(Decompress(DecryptOrderData(xph)));
                            // Zip File ausgeben
                            Guid myuuid = Guid.NewGuid();
                            string myuuidAsString = myuuid.ToString();
                            byte[] data = _orderData.Select(byte.Parse).ToArray();
                            string distPath = "CAMT053_" + myuuidAsString + ".zip";
                            using (FileStream dstZip = new FileStream(distPath, FileMode.Create))
                            {
                                dstZip.Write(data, 0, data.Length);
                            }
                            Response.Data = string.Join("", distPath);
                            break;
                    }

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

        private IList<XmlDocument> CreateRequests()
        {
            using (new MethodLogger(s_logger))
            {
                try
                {
                    if (_initLastSegment)
                    {
                        s_logger.LogDebug("lastSegment is {lastSegment}. Not creating any transfer requests",
                            _initLastSegment);
                        return null;
                    }

                    var reqs = new List<XmlDocument>();

                    for (var i = 1; i < _numSegments; i++)
                    {
                        s_logger.LogDebug("Creating transfer request {no}", i);
                        var req = new EbicsRequest
                        {
                            Namespaces = Namespaces,
                            Version = Config.Version,
                            Revision = Config.Revision,
                            StaticHeader = new StaticHeader
                            {
                                HostId = Config.User.HostId,
                                TransactionId = _transactionId,
                            },
                            MutableHeader = new MutableHeader
                            {
                                Namespaces = Namespaces,
                                TransactionPhase = "Transfer",
                                SegmentNumber = i + _initSegment,
                                LastSegment = i + _initSegment == _numSegments
                            },
                            Body = new Body
                            {
                                Namespaces = Namespaces
                            }
                        };

                        reqs.Add(AuthenticateXml(req.Serialize().ToXmlDocument(), null, null));
                    }

                    return reqs;
                }
                catch (EbicsException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new CreateRequestException($"can't create {OrderType} requests", ex);
                }
            }
        }

        private XmlDocument CreateReceiptRequest()
        {
            using (new MethodLogger(s_logger))
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
                    throw new CreateRequestException($"can't create {OrderType} receipt request", ex);
                }
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
                    throw new CreateRequestException($"can't create {OrderType} init request", ex);
                }
            }
        }
    }
}
