using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using libfintx.FinTS;
using libfintx.FinTS.Data.Segment;
using Xunit;

namespace libfintx.Tests
{
    public class HelperTest
    {
        [Fact]
        public void TestSplitMessages_1()
        {
            var message =
@"HNHBK:1:3+000000000604+300+KI1031610095894+1+KI1031610095894:1'
HNVSK:998:3+PIN:2+998+1+2::gsYsKzQ0YHYBAAAdyJv1hGuowAQA+1:20210316:100958+2:2:13:@8@00000000:5:1+280:10090000:XXXXXX:V:0:0+0'
HNVSD:999:1+@381@HNSHK:2:4+PIN:2+944+330097059902611+1+1+2::gsYsKzQ0YHYBAAAdyJv1hGuowAQA+1+1:20210316:100958+1:999:1+6:10:16+280:10090000:XXXXXX:S:0:0'
HIRMG:3:2+0010::Nachricht entgegengenommen.'
HIRMS:4:2:5+0030::Auftrag empfangen - Sicherheitsfreigabe erforderlich'
HITAN:5:6:5+4++BgBjqwg2OngBAAAqEcp9hGuowAQA+Ihre TAN wurde an Ihre App ?'Apple iPhone8,1?' gesendet.'
HNSHA:6:2+330097059902611''
HNHBS:7:1+1'";

            message = message.Replace(Environment.NewLine, string.Empty);

            var segments = Helper.SplitSegments(message);

            Assert.Equal(8, segments.Count);
        }

        [Fact]
        public void Test_PhotoTAN()
        {
            var message = File.ReadAllText("Resources/Photo_TAN.txt");
            var rawSegments = Helper.SplitSegments(message);

            // Helper.SplitSegments() does not handle binary data correctly.
            // After this has been fixed, this test can be modified so that it does not expect an exception any more
            Assert.Throws<ArgumentException>(() =>
            {
                foreach (var rawSegment in rawSegments)
                {
                    var segment = new Segment(rawSegment);
                    new GenericSegmentParser().ParseSegment(segment);
                }
            });
        }
    }
}
