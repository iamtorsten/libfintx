using System;
using System.Collections.Generic;
using System.Text;

namespace libfintx.Data.Segment
{
    public interface ISegmentParser
    {
        Segment ParseSegment(Segment segment);
    }
}
