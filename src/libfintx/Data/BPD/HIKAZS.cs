using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using libfintx.Data.Segment;

namespace libfintx
{
    public class HIKAZS : SegmentBase
    {
        public int Zeitraum { get; set; }

        public HIKAZS(Segment segment) : base(segment)
        {
        }
    }
}
