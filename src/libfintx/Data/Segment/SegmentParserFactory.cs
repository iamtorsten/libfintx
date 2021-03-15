using System;
using System.Collections.Generic;
using System.Text;

namespace libfintx.Data.Segment
{
    internal class SegmentParserFactory
    {
        public static Segment ParseSegment(string segmentCode)
        {
            var segment = new Segment(segmentCode);

            var genericParser = new GenericSegmentParser();
            genericParser.ParseSegment(segment);

            Type parserType = Type.GetType($"libfintx.Data.Segment.{segment.Name}SegmentParser");
            if (parserType == null)
                return segment;

            return ((ISegmentParser) Activator.CreateInstance(parserType)).ParseSegment(segment);
        }
    }
}
