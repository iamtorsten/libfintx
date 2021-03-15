using System;
using System.Collections.Generic;
using System.Text;

namespace libfintx.Data.Segment
{
    public abstract class SegmentBase : Segment
    {
        public SegmentBase(Segment segment) : base(segment.Value)
        {
            this.Name = segment.Name;
            this.Number = segment.Number;
            this.Version = segment.Version;
            this.Ref = segment.Ref;
            this.Payload = segment.Payload;
        }
    }
}
