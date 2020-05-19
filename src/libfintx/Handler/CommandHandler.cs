/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using Microsoft.Extensions.Logging;
using libfintx.Commands;
using libfintx.Config;
using libfintx.Parameters;
using libfintx.Responses;

namespace libfintx.Handler
{
    internal class CommandHandler
    {
        private static readonly ILogger s_logger = EbicsLogging.CreateLogger<CommandHandler>();

        internal EbicsConfig Config { private get; set; }
        internal NamespaceConfig Namespaces { private get; set; }
        internal ProtocolHandler ProtocolHandler { private get; set; }

        private Command CreateCommand(Params cmdParams)
        {
            using (new MethodLogger(s_logger))
            {
                Command cmd = null;

                s_logger.LogDebug("Parameters: {params}", cmdParams.ToString());

                switch (cmdParams)
                {
                    case HpbParams hpb:
                        cmd = new HpbCommand {Params = hpb, Config = Config, Namespaces = Namespaces};
                        break;
                    case PtkParams ptk:
                        cmd = new PtkCommand {Params = ptk, Config = Config, Namespaces = Namespaces};
                        break;
                    case CctParams cct:
                        cmd = new CctCommand {Params = cct, Config = Config, Namespaces = Namespaces};
                        break;
                    case StaParams sta:
                        cmd = new StaCommand {Params = sta, Config = Config, Namespaces = Namespaces};
                        break;
                    case SprParams spr:
                        cmd = new SprCommand {Params = spr, Config = Config, Namespaces = Namespaces};
                        break;
                    case IniParams ini:
                        cmd = new IniCommand {Params = ini, Config = Config, Namespaces = Namespaces};
                        break;
                    case HiaParams hia:
                        cmd = new HiaCommand {Params = hia, Config = Config, Namespaces = Namespaces};
                        break;
                    case CddParams cdd:
                        cmd = new CddCommand {Params = cdd, Config = Config, Namespaces = Namespaces};
                        break;
                }

                s_logger.LogDebug("Command created: {cmd}", cmd?.ToString());

                return cmd;
            }
        }

        internal T Send<T>(Params cmdParams) where T : Response
        {
            var cmd = CreateCommand(cmdParams);
            ProtocolHandler.Send(cmd);
            return ((GenericCommand<T>) cmd).Response;
        }
    }
}