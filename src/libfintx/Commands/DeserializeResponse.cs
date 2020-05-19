/*
 * NetEbics -- .NET Core EBICS Client Library
 * (c) Copyright 2018 Bjoern Kuensting
 *
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using libfintx.Handler;
using StatePrinting;
using StatePrinting.OutputFormatters;

namespace libfintx.Commands
{
    internal class DeserializeResponse
    {
        private static readonly Stateprinter _printer;
        
        public int NumSegments { get; set; }
        public int SegmentNumber { get; set; }
        public bool LastSegment { get; set; }
        public string TransactionId { get; set; }
        public TransactionPhase Phase { get; set; }
        public int TechnicalReturnCode { get; set; }
        public int BusinessReturnCode { get; set; }
        public string ReportText { get; set; }

        public bool HasError =>
            ((TechnicalReturnCode != ReturnCodes.TechnicalCodeOK &&
              TechnicalReturnCode != ReturnCodes.TechnicalCodePostProcessDone &&
              TechnicalReturnCode != ReturnCodes.TechnicalCodePostProcessSkipped &&
              TechnicalReturnCode != ReturnCodes.TechnicalCodeRecoverySync) ||
             BusinessReturnCode != ReturnCodes.BusinessCodeOK);

        public bool IsRecoverySync =>
            (TechnicalReturnCode == ReturnCodes.TechnicalCodeRecoverySync);
        
        
        static DeserializeResponse()
        {
            _printer = new Stateprinter();
            _printer.Configuration.SetNewlineDefinition("");
            _printer.Configuration.SetIndentIncrement(" ");
            _printer.Configuration.SetOutputFormatter(new JsonStyle(_printer.Configuration));
        }

        public override string ToString() => _printer.PrintObject(this);
    }
    
    
}