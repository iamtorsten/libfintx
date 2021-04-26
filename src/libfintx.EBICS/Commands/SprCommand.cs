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
using System.Linq;
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
    internal class SprCommand : GenericCommand<SprResponse>
    {
        private static readonly ILogger s_logger = EbicsLogging.CreateLogger<SprCommand>();
        private readonly byte[] _transactionKey;

        internal SprParams Params { private get; set; }
        internal override string OrderType => "SPR";
        internal override string OrderAttribute => "UZHNN";
        internal override TransactionType TransactionType => TransactionType.Upload;
        internal override IList<XmlDocument> Requests => null;
        internal override XmlDocument InitRequest => CreateInitRequest();
        internal override XmlDocument ReceiptRequest => null;

        public SprCommand()
        {
            _transactionKey = CryptoUtils.GetTransactionKey();
            s_logger.LogDebug("Transaction Key: {key}", CryptoUtils.Print(_transactionKey));
        }

        private XmlDocument CreateInitRequest()
        {
            using (new MethodLogger(s_logger))
            {
                try
                {
                    var signedData = SignData(Encoding.ASCII.GetBytes(" "), Config.User.SignKeys);

                    var userSigData = new UserSignatureData
                    {
                        Namespaces = Namespaces,
                        OrderSignatureData = new OrderSignatureData
                        {
                            Namespaces = Namespaces,
                            PartnerId = Config.User.PartnerId,
                            UserId = Config.User.UserId,
                            SignatureValue = signedData,
                            SignKeys = Config.User.SignKeys
                        }
                    };

                    s_logger.LogDebug("User signature data:\n{data}", userSigData.ToString());

                    var userSigDataComp =
                        Compress(
                            Encoding.UTF8.GetBytes(userSigData.Serialize().ToString(SaveOptions.DisableFormatting)));
                    var userSigDataEnc = EncryptAes(userSigDataComp, _transactionKey);

                    XNamespace nsEbics = Namespaces.Ebics;

                    var initReq = new EbicsRequest
                    {
                        Namespaces = Namespaces,
                        Version = Config.Version,
                        Revision = Config.Revision,
                        StaticHeader = new StaticHeader
                        {
                            Namespaces = Namespaces,
                            HostId = Config.User.HostId,
                            Nonce = CryptoUtils.GetNonce(),
                            Timestamp = CryptoUtils.GetUtcTimeNow(),
                            PartnerId = Config.User.PartnerId,
                            UserId = Config.User.UserId,
                            NumSegments = 0,
                            OrderDetails = new OrderDetails
                            {
                                Namespaces = Namespaces,
                                OrderType = OrderType,
                                OrderAttribute = OrderAttribute,
                                StandardOrderParams = new EmptyOrderParams(),
                            },
                            BankPubKeyDigests = new BankPubKeyDigests
                            {
                                Namespaces = Namespaces,
                                DigestAlgorithm = s_digestAlg,
                                Bank = Config.Bank
                            }
                        },
                        MutableHeader = new MutableHeader
                        {
                            Namespaces = Namespaces,
                            TransactionPhase = "Initialisation"
                        },
                        Body = new Body
                        {
                            Namespaces = Namespaces,
                            DataTransfer = new DataTransfer
                            {
                                Namespaces = Namespaces,
                                DataEncryptionInfo = new DataEncryptionInfo
                                {
                                    Namespaces = Namespaces,
                                    EncryptionPubKeyDigest = new EncryptionPubKeyDigest
                                    {
                                        Namespaces = Namespaces,
                                        Bank = Config.Bank,
                                        DigestAlgorithm = s_digestAlg
                                    },
                                    TransactionKey = Convert.ToBase64String(EncryptRsa(_transactionKey))
                                },
                                SignatureData = new SignatureData
                                {
                                    Namespaces = Namespaces
                                }
                            }
                        }
                    };

                    var xmlInitReq = initReq.Serialize();
                    xmlInitReq.Descendants(nsEbics + XmlNames.SignatureData).FirstOrDefault()
                        .Add(Convert.ToBase64String(userSigDataEnc));
                    return AuthenticateXml(xmlInitReq.ToXmlDocument(), null, null);
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
