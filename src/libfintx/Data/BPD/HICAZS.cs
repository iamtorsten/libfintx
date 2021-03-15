using System;
using System.Text.RegularExpressions;
using libfintx.Data.Segment;

namespace libfintx
{
    public class HICAZS : SegmentBase
    {
        public int? Zeitraum { get; set; }

        public string Format { get; set; }

        public HICAZS (Segment segment) : base(segment)
        {
        }
    }
}
