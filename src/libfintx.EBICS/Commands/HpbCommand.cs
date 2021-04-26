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
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using libfintx.EBICS.Exceptions;
using libfintx.EBICS.Parameters;
using libfintx.EBICS.Responses;
using libfintx.EBICSConfig;
using libfintx.Xml;

namespace libfintx.EBICS.Commands
{
    internal class HpbCommand : GenericCommand<HpbResponse>
    {
        private static readonly ILogger s_logger = EbicsLogging.CreateLogger<HpbCommand>();
        private IList<XmlDocument> _requests;

        internal HpbParams Params { private get; set; }
        internal override TransactionType TransactionType => TransactionType.Upload;
        internal override IList<XmlDocument> Requests => _requests ?? (_requests = CreateRequests());
        internal override XmlDocument InitRequest => null;
        internal override XmlDocument ReceiptRequest => null;
        internal override string OrderType => "HPB";
        internal override string OrderAttribute => "DZHNN";

        internal override DeserializeResponse Deserialize(string payload)
        {
            try
            {
                using (new MethodLogger(s_logger))
                {
                    var dr = base.Deserialize(payload);

                    var doc = XDocument.Parse(payload);
                    var xph = new XPathHelper(doc, Namespaces);

                    Response.Bank = new BankParams();

                    Response.OrderId = xph.GetOrderID()?.Value;
                    Response.TimestampBankParameter = ParseTimestamp(xph.GetTimestampBankParameter()?.Value);

                    if (dr.HasError)
                    {
                        return dr;
                    }

                    var decryptedOd = DecryptOrderData(xph);
                    var deflatedOd = Decompress(decryptedOd);
                    var strResp = Encoding.UTF8.GetString(deflatedOd);
                    var hpbrod = XDocument.Parse(strResp);

                    s_logger.LogDebug("Order data:\n{orderData}", hpbrod.ToString());

                    var r = new XPathHelper(hpbrod, Namespaces);

                    if (r.GetAuthenticationPubKeyInfoX509Data() != null || r.GetEncryptionPubKeyInfoX509Data() != null)
                    {
                        throw new DeserializationException("X509 not supported yet", payload);
                    }

                    if (r.GetAuthenticationPubKeyInfoPubKeyValue() != null)
                    {
                        if (!Enum.TryParse<AuthVersion>(r.GetAuthenticationPubKeyInfoAuthenticationVersion()?.Value,
                            out var authVersion))
                        {
                            throw new DeserializationException(
                                "unknown authentication version for bank's authentication key");
                        }

                        var modulus =
                            Convert.FromBase64String(r.GetAuthenticationPubKeyInfoModulus()?.Value);
                        var exponent =
                            Convert.FromBase64String(r.GetAuthenticationPubKeyInfoExponent()?.Value);

                        var authPubKeyParams = new RSAParameters {Exponent = exponent, Modulus = modulus};
                        var authPubKey = RSA.Create();
                        authPubKey.ImportParameters(authPubKeyParams);
                        Response.Bank.AuthKeys = new AuthKeyPair
                        {
                            PublicKey = authPubKey,
                            Version = authVersion
                        };
                    }
                    else
                    {
                        throw new DeserializationException($"{XmlNames.AuthenticationPubKeyInfo} missing", payload);
                    }

                    if (r.GetEncryptionPubKeyInfoPubKeyValue() != null)
                    {
                        if (!Enum.TryParse<CryptVersion>(r.GetEncryptionPubKeyInfoEncryptionVersion()?.Value,
                            out var encryptionVersion))
                        {
                            throw new DeserializationException("unknown encryption version for bank's encryption key");
                        }

                        var modulus =
                            Convert.FromBase64String(r.GetEncryptionPubKeyInfoModulus()?.Value);
                        var exponent =
                            Convert.FromBase64String(r.GetEncryptionPubKeyInfoExponent()?.Value);

                        var cryptPubKeyParams = new RSAParameters {Exponent = exponent, Modulus = modulus};
                        var cryptPubKey = RSA.Create();
                        cryptPubKey.ImportParameters(cryptPubKeyParams);
                        Response.Bank.CryptKeys = new CryptKeyPair
                        {
                            PublicKey = cryptPubKey,
                            Version = encryptionVersion
                        };
                    }
                    else
                    {
                        throw new DeserializationException($"{XmlNames.EncryptionPubKeyInfo} missing", payload);
                    }

                    s_logger.LogDebug("Bank authentication key digest: {digest}",
                        CryptoUtils.Print(Response.Bank?.AuthKeys?.Digest));
                    s_logger.LogDebug("Bank encryption key digest: {digest}",
                        CryptoUtils.Print(Response.Bank?.CryptKeys?.Digest));

                    return dr;
                }
            }
            catch (EbicsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DeserializationException($"can't deserialize {OrderType} response", ex, payload);
            }
        }

        private List<XmlDocument> CreateRequests()
        {
            using (new MethodLogger(s_logger))
            {
                try
                {
                    var reqs = new List<XmlDocument>();

                    var req = new EbicsNoPubKeyDigestsRequest
                    {
                        StaticHeader = new StaticHeader
                        {
                            HostId = Config.User.HostId,
                            Nonce = CryptoUtils.GetNonce(),
                            Timestamp = CryptoUtils.GetUtcTimeNow(),
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
                            Namespaces = Namespaces
                        },
                        Version = Config.Version,
                        Revision = Config.Revision,
                        Namespaces = Namespaces
                    };

                    reqs.Add(AuthenticateXml(req.Serialize().ToXmlDocument(), null, null));
                    return reqs;
                }
                catch (Exception e)
                {
                    throw new CreateRequestException($"can't create requests for {OrderType}", e);
                }
            }
        }
    }
}
