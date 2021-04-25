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

using Microsoft.Extensions.Logging;
using libfintx.EBICS.Commands;
using libfintx.EBICS.Parameters;
using libfintx.EBICS.Responses;
using libfintx.EBICSConfig;

namespace libfintx.EBICS.Handler
{
    internal class CommandHandler
    {
        private static readonly ILogger s_logger = EbicsLogging.CreateLogger<CommandHandler>();

        internal Config Config { private get; set; }
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
