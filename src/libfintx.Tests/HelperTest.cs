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
        public void TestSplitSegments_1()
        {
            var message =
@"HNHBK:1:3+000000000443+300+kQhXXcecBq4120210429134136079%+2+kQhXXcecBq4120210429134136079%:2'
HNVSK:998:3+PIN:1+998+1+2::37971492958550005K551VUX2BCW3U+1:20210429:134136+2:2:13:@8@        :5:1+280:10070124:XXXXXX:V:0:0+0'
HNVSD:999:1+@187@
HIRMG:2:2+9050::Teilweise fehlerhaft.'
HIRMS:3:2:3+9210::Wert widerspricht Bankvorgaben.'
HIRMS:4:2:4+9210::Auftrag abgelehnt - Zwei-Schritt-TAN inkonsistent. Eingereichter Auftrag gelösch''
HNHBS:5:1+2'".Replace(Environment.NewLine, string.Empty);

            var segments = Helper.SplitSegments(message);
            Assert.Equal(5, segments.Count);

            Assert.StartsWith("HNHBK", segments[0]);
            Assert.StartsWith("HIRMG", segments[1]);
            Assert.StartsWith("HIRMS", segments[2]);
            Assert.StartsWith("HIRMS", segments[3]);
            Assert.StartsWith("HNHBS", segments[4]);
        }
    }
}
