using System;
using System.Collections.Generic;
using System.Text;
using libfintx.FinTS.Data.Segment;

namespace libfintx.FinTS
{
    public class HITANS : SegmentBase
    {
        public List<HITANS_TanProcess> TanProcesses { get; set; }

        public HITANS(Segment segment) : base(segment)
        {
            TanProcesses = new List<HITANS_TanProcess>();
        }
    }

    public class HITANS_TanProcess
    {
        public int TanCode { get; set; }

        public string Name { get; set; }
    }
}
