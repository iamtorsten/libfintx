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

            var segments = Helper.SplitEncryptedSegments(message);
            Assert.Equal(5, segments.Count);

            Assert.StartsWith("HNHBK", segments[0]);
            Assert.StartsWith("HIRMG", segments[1]);
            Assert.StartsWith("HIRMS", segments[2]);
            Assert.StartsWith("HIRMS", segments[3]);
            Assert.StartsWith("HNHBS", segments[4]);
        }

        [Fact]
        public void TestSplitSegments_2()
        {
            var message = "HNHBK:1:3+000000000605+300+IL1061013552403+1+IL1061013552403:1'HNVSK:998:3+PIN:2+998+1+2::bTZX3AOq9XkBAABqEKiFhWuowAQA+1:20210610:135525+2:2:13:@8@00000000:5:1+280:10090000:XXXXXXXXXXX:V:0:0+0'HNVSD:999:1+@382@HNSHK:2:4+PIN:2+944+967729067269966+1+1+2::bTZX3AOq9XkBAABqEKiFhWuowAQA+1+1:20210610:135525+1:999:1+6:10:16+280:10090000:XXXXXXXXXXX:S:0:0'HIRMG:3:2+0010::Nachricht entgegengenommen.'HIRMS:4:2:5+0030::Auftrag empfangen - Sicherheitsfreigabe erforderlich'HITAN:5:6:5+4++g9thgHmp9XkBAACc9QLMh2uowAQA+Ihre TAN wurde an Ihre App ?'Apple iPhone10,4?' gesendet.'HNSHA:6:2+967729067269966''HNHBS:7:1+1'";

            var segments = Helper.SplitEncryptedSegments(message);
            Assert.Equal(7, segments.Count);

            Assert.StartsWith("HNHBK", segments[0]);
            Assert.StartsWith("HNSHK", segments[1]);
            Assert.StartsWith("HIRMG", segments[2]);
            Assert.StartsWith("HIRMS", segments[3]);
            Assert.StartsWith("HITAN", segments[4]);
            Assert.StartsWith("HNSHA", segments[5]);
            Assert.StartsWith("HNHBS", segments[6]);
        }
    }
}
