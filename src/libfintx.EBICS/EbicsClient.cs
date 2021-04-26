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
using System.Net.Http;
using Microsoft.Extensions.Logging;
using libfintx.EBICS.Handler;
using libfintx.EBICS.Parameters;
using libfintx.EBICS.Responses;
using libfintx.EBICSConfig;

namespace libfintx.EBICS
{
    public class EbicsClientFactory
    {
        private Func<Config, IEbicsClient> _ctor;

        public EbicsClientFactory(Func<Config, IEbicsClient> ctor)
        {
            _ctor = ctor;
        }

        public IEbicsClient Create(Config cfg)
        {
            return _ctor(cfg);
        }
    }

    public class EbicsClient : IEbicsClient
    {
        private static readonly ILogger Logger = EbicsLogging.CreateLogger<EbicsClient>();
        private Config _config;
        private HttpClient _httpClient;
        private readonly ProtocolHandler _protocolHandler;
        private readonly CommandHandler _commandHandler;

        public Config Config
        {
            get => _config;
            set
            {
                _config = value ?? throw new ArgumentNullException(nameof(Config));
                _httpClient = new HttpClient {BaseAddress = new Uri(_config.Address)};
                _commandHandler.Config = value;
                _protocolHandler.Client = _httpClient;
                _commandHandler.ProtocolHandler = _protocolHandler;
                var nsCfg = new NamespaceConfig();
                switch (_config.Version)
                {
                    case EbicsVersion.H004:
                        nsCfg.Ebics = $"urn:org:ebics:{EbicsVersion.H004.ToString()}";
                        break;
                    case EbicsVersion.H005:
                        nsCfg.Ebics = $"urn:org:ebics:{EbicsVersion.H005.ToString()}";
                        break;
                    default:
                        throw new ArgumentException(nameof(Config.Version));
                }

                nsCfg.XmlDsig = "http://www.w3.org/2000/09/xmldsig#";
                nsCfg.Cct = "urn:iso:std:iso:20022:tech:xsd:pain.001.001.03";
                nsCfg.Cdd = "urn:iso:std:iso:20022:tech:xsd:pain.008.001.02";
                nsCfg.SignatureData = "http://www.ebics.org/S001";

                _commandHandler.Namespaces = nsCfg;
            }
        }

        public static EbicsClientFactory Factory()
        {
            return new EbicsClientFactory(x => new EbicsClient {Config = x});
        }

        private EbicsClient()
        {
            _protocolHandler = new ProtocolHandler();
            _commandHandler = new CommandHandler();            
        }

        public HpbResponse HPB(HpbParams p)
        {
            using (new MethodLogger(Logger))
            {
                var resp = _commandHandler.Send<HpbResponse>(p);
                return resp;
            }
        }

        public PtkResponse PTK(PtkParams p)
        {
            using (new MethodLogger(Logger))
            {
                var resp = _commandHandler.Send<PtkResponse>(p);
                return resp;
            }
        }

        public StaResponse STA(StaParams p)
        {
            using (new MethodLogger(Logger))
            {
                var resp = _commandHandler.Send<StaResponse>(p);
                return resp;
            }
        }

        public CctResponse CCT(CctParams p)
        {
            using (new MethodLogger(Logger))
            {
                var resp = _commandHandler.Send<CctResponse>(p);
                return resp;
            }
        }

        public IniResponse INI(IniParams p)
        {
            using (new MethodLogger(Logger))
            {
                var resp = _commandHandler.Send<IniResponse>(p);
                return resp;
            }
        }

        public HiaResponse HIA(HiaParams p)
        {
            using (new MethodLogger(Logger))
            {
                var resp = _commandHandler.Send<HiaResponse>(p);
                return resp;
            }
        }

        public SprResponse SPR(SprParams p)
        {
            using (new MethodLogger(Logger))
            {
                var resp = _commandHandler.Send<SprResponse>(p);
                return resp;
            }
        }

        public CddResponse CDD(CddParams p)
        {
            using (new MethodLogger(Logger))
            {
                var resp = _commandHandler.Send<CddResponse>(p);
                return resp;
            }
        }
    }
}
