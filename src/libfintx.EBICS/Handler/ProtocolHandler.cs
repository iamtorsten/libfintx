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
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using libfintx.EBICS.Commands;
using libfintx.EBICS.Exceptions;

namespace libfintx.EBICS.Handler
{
    internal class Context
    {
        internal State State { get; set; }
        internal Command Cmd { get; set; }
        internal HttpClient Client { get; set; }
        internal int NumSegments { get; set; }
        internal int SegmentIndex { get; set; }
        internal int SegmentNumber { get; set; }
        internal bool LastSegment { get; set; }
        internal string TransactionId { get; set; }
        internal TransactionPhase Phase { get; set; }
        internal int TechReturnCode { get; set; }
        internal int BusReturnCode { get; set; }

        internal void Handle()
        {
            State.Handle(this);
        }

        public override string ToString()
        {
            return $"{nameof(State)}: {State}, {nameof(NumSegments)}: {NumSegments}, {nameof(SegmentIndex)}: {SegmentIndex}, {nameof(SegmentNumber)}: {SegmentNumber}, {nameof(LastSegment)}: {LastSegment}, {nameof(TransactionId)}: {TransactionId}, {nameof(Phase)}: {Phase}, {nameof(TechReturnCode)}: {TechReturnCode}, {nameof(BusReturnCode)}: {BusReturnCode}";
        }
    }

    internal abstract class State
    {
        private static readonly ILogger s_logger = EbicsLogging.CreateLogger<State>();

        protected const int _maxRecoverySyncCount = 3;

        protected int RecoverySyncCount { get; set; }

        public virtual void Handle(Context ctx)
        {
            s_logger.LogDebug("Context: {context}", ctx);
        }

        protected string Send(Context ctx, string request)
        {
            using (new MethodLogger(s_logger))
            {
                if (s_logger.IsEnabled(LogLevel.Debug))
                {
                    var tmp = XDocument.Parse(request);
                    s_logger.LogDebug("Sending request:\n{request}", tmp.ToString());    
                }

                var content = new StringContent(request, Encoding.UTF8, "application/xml");
                HttpResponseMessage httpResp;
                try
                {
                    httpResp = ctx.Client.PostAsync(ctx.Client.BaseAddress, content).Result;
                    if (!httpResp.IsSuccessStatusCode)
                    {
                        s_logger.LogError("Got status code {code}", httpResp.StatusCode.ToString());
                        throw new ConnectionException($"Got http status code {httpResp.StatusCode} for response");
                    }
                }
                catch (ConnectionException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ConnectionException(ex.Message, ex);
                }

                var response = httpResp.Content.ReadAsStringAsync().Result;
                s_logger.LogDebug("Got response:\n{response}", response);
                return response;
            }
        }

        protected void UpdateCtx(Context ctx, DeserializeResponse dr)
        {
            ctx.BusReturnCode = dr.BusinessReturnCode;
            ctx.LastSegment = dr.LastSegment;
            ctx.NumSegments = dr.NumSegments;
            ctx.SegmentNumber = dr.SegmentNumber;
            ctx.TechReturnCode = dr.TechnicalReturnCode;
            ctx.TransactionId = dr.TransactionId;
            ctx.Phase = dr.Phase;
        }

        protected bool IsRecoverySync(Context ctx, DeserializeResponse dr)
        {
            if (!dr.IsRecoverySync)
            {
                return false;
            }

            if (RecoverySyncCount >= _maxRecoverySyncCount)
            {
                throw new RecoverySyncException($"Recovery sync failed for {ctx.Cmd.OrderType}");
            }

            return true;
        }
    }

    internal class InitState : State
    {
        private static readonly ILogger s_logger = EbicsLogging.CreateLogger<InitState>();

        public override void Handle(Context ctx)
        {
            using (new MethodLogger(s_logger))
            {
                base.Handle(ctx);

                var initReq = ctx.Cmd.InitRequest;

                if (initReq != null)
                {
                    var payload = Send(ctx, initReq.OuterXml);
                    var dr = ctx.Cmd.Deserialize(payload);
                    UpdateCtx(ctx, dr);

                    if (dr.HasError)
                    {
                        ctx.State = null;
                        return;
                    }

                    if (IsRecoverySync(ctx, dr))
                    {
                        RecoverySyncCount++;
                        return;
                    }
                }

                switch (ctx.Cmd.TransactionType)
                {
                    case TransactionType.Upload:
                        ctx.State = new TransferUploadState();
                        break;
                    case TransactionType.Download:
                        ctx.State = new TransferDownloadState();
                        break;
                    default:
                        ctx.State = null;
                        break;
                }
            }
        }
    }

    internal class TransferUploadState : State
    {
        private static readonly ILogger s_logger = EbicsLogging.CreateLogger<TransferUploadState>();
        private IList<XmlDocument> _requests;

        public override void Handle(Context ctx)
        {
            using (new MethodLogger(s_logger))
            {
                base.Handle(ctx);

                _requests = _requests ?? ctx.Cmd.Requests;

                if (_requests != null)
                {
                    var req = _requests[ctx.SegmentIndex].OuterXml;
                    var payload = Send(ctx, req);
                    var dr = ctx.Cmd.Deserialize(payload);
                    UpdateCtx(ctx, dr);

                    if (dr.HasError || dr.LastSegment)
                    {
                        ctx.State = null;
                        return;
                    }

                    if (IsRecoverySync(ctx, dr))
                    {
                        _requests = ctx.Cmd.Requests;
                        RecoverySyncCount++;
                        ctx.SegmentIndex = ctx.SegmentNumber + 1;
                    }
                    else if (ctx.SegmentIndex < _requests.Count - 1)
                    {
                        ctx.SegmentIndex++;
                    }
                    else
                    {
                        ctx.State = null;
                    }
                }
                else
                {
                    ctx.State = null;
                }
            }
        }
    }

    internal class TransferDownloadState : State
    {
        private static readonly ILogger s_logger = EbicsLogging.CreateLogger<TransferDownloadState>();
        private IList<XmlDocument> _requests;

        public override void Handle(Context ctx)
        {
            using (new MethodLogger(s_logger))
            {
                base.Handle(ctx);

                _requests = _requests ?? ctx.Cmd.Requests;

                if (_requests != null)
                {
                    var req = _requests[ctx.SegmentNumber].OuterXml;
                    var payload = Send(ctx, req);
                    var dr = ctx.Cmd.Deserialize(payload);
                    UpdateCtx(ctx, dr);

                    if (dr.HasError)
                    {
                        ctx.State = null;
                        return;
                    }

                    if (IsRecoverySync(ctx, dr))
                    {
                        _requests = ctx.Cmd.Requests;
                        RecoverySyncCount++;
                        ctx.SegmentIndex = ctx.SegmentNumber + 1;
                    }
                    else if (ctx.SegmentIndex < _requests.Count - 1 && !ctx.LastSegment)
                    {
                        ctx.SegmentIndex++;
                    }
                    else
                    {
                        ctx.State = new ReceiptState();
                    }
                }
                else
                {
                    ctx.State = new ReceiptState();
                }
            }
        }
    }

    internal class ReceiptState : State
    {
        private static readonly ILogger s_logger = EbicsLogging.CreateLogger<ReceiptState>();

        public override void Handle(Context ctx)
        {
            using (new MethodLogger(s_logger))
            {
                base.Handle(ctx);

                var receiptReq = ctx.Cmd.ReceiptRequest;

                if (receiptReq != null)
                {
                    var payload = Send(ctx, receiptReq.OuterXml);
                    var dr = ctx.Cmd.Deserialize(payload);
                    UpdateCtx(ctx, dr);

                    if (IsRecoverySync(ctx, dr))
                    {
                        RecoverySyncCount++;
                        return;
                    }
                }

                ctx.State = null;
            }
        }
    }

    internal class ProtocolHandler
    {
        private static readonly ILogger s_logger = EbicsLogging.CreateLogger<ProtocolHandler>();

        internal HttpClient Client { private get; set; }

        internal void Send(Command cmd)
        {
            using (new MethodLogger(s_logger))
            {
                var ctx = new Context {Cmd = cmd, State = new InitState(), Client = Client};

                for (; ctx.State != null;)
                {
                    ctx.Handle();
                }
            }
        }
    }
}
