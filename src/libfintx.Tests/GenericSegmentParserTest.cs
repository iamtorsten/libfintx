using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using libfintx.FinTS.Data.Segment;
using Xunit;

namespace libfintx.Tests
{
    public class GenericSegmentParserTest
    {
        [Fact]
        public void TestHikazs()
        {
            var segmentCode = "HIKAZS:23:6:4+20+1+1+90:N:N";
            var segment = new Segment(segmentCode);
            segment = new GenericSegmentParser().ParseSegment(segment);

            Assert.Equal("HIKAZS", segment.Name);
            Assert.Equal(23, segment.Number);
            Assert.Equal(6, segment.Version);
            Assert.Equal(4, segment.Ref);
            Assert.False(string.IsNullOrWhiteSpace(segment.Payload));
        }

        [Fact]
        public void TestHitan()
        {
            var segmentCode = "HITAN:5:7:3+S++eNmo9/2dEocBAACRO?+gjhW?+owAQA";
            var segment = new Segment(segmentCode);
            segment = new GenericSegmentParser().ParseSegment(segment);

            Assert.Equal("HITAN", segment.Name);
            Assert.Equal(5, segment.Number);
            Assert.Equal(7, segment.Version);
            Assert.Equal(3, segment.Ref);
            Assert.Equal(3, segment.DataElements.Count);
            Assert.Equal("eNmo9/2dEocBAACRO?+gjhW?+owAQA", segment.DataElements[2]);
        }
    }
}
