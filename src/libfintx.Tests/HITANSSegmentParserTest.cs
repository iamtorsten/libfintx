using System;
using System.Collections.Generic;
using System.Text;
using libfintx.FinTS;
using libfintx.FinTS.Data.Segment;
using Xunit;

namespace libfintx.Tests
{
    public class HITANSSegmentParserTest
    {
        [Fact]
        public void Test_HITANS_6()
        {
            var rawSegment = @"HITANS:174:6:4+1+1+1+J:N:0:910:2:HHD1.3.0:::chipTAN manuell:6:1:TAN-Nummer:3:J:2:N:0:0:N:N:00:0:N:1:911:2:HHD1.3.2OPT:HHDOPT1:1.3.2:chipTAN optisch:6:1:TAN-Nummer:3:J:2:N:0:0:N:N:00:0:N:1:912:2:HHD1.3.2USB:HHDUSB1:1.3.2:chipTAN-USB:6:1:TAN-Nummer:3:J:2:N:0:0:N:N:00:0:N:1:913:2:Q1S:Secoder_UC:1.2.0:chipTAN-QR:6:1:TAN-Nummer:3:J:2:N:0:0:N:N:00:0:N:1:920:2:smsTAN:::smsTAN:6:1:TAN-Nummer:3:J:2:N:0:0:N:N:00:2:N:5:921:2:pushTAN:::pushTAN:6:1:TAN-Nummer:3:J:2:N:0:0:N:N:00:2:N:2:900:2:iTAN:::iTAN:6:1:TAN-Nummer:3:J:2:N:0:0:N:N:00:0:N:0'";
            var segment = new Segment(rawSegment);
            segment = new GenericSegmentParser().ParseSegment(segment);

            var parser = new HITANSSegmentParser();
            var hitans = (HITANS) parser.ParseSegment(segment);

            Assert.Equal("HITANS", hitans.Name);
            Assert.Equal(7, hitans.TanProcesses.Count);
            Assert.Equal(910, hitans.TanProcesses[0].TanCode);
            Assert.Equal("chipTAN manuell", hitans.TanProcesses[0].Name);
            Assert.Equal(911, hitans.TanProcesses[1].TanCode);
            Assert.Equal("chipTAN optisch", hitans.TanProcesses[1].Name);
            Assert.Equal(912, hitans.TanProcesses[2].TanCode);
            Assert.Equal("chipTAN-USB", hitans.TanProcesses[2].Name);
            Assert.Equal(913, hitans.TanProcesses[3].TanCode);
            Assert.Equal("chipTAN-QR", hitans.TanProcesses[3].Name);
            Assert.Equal(920, hitans.TanProcesses[4].TanCode);
            Assert.Equal("smsTAN", hitans.TanProcesses[4].Name);
            Assert.Equal(921, hitans.TanProcesses[5].TanCode);
            Assert.Equal("pushTAN", hitans.TanProcesses[5].Name);
            Assert.Equal(900, hitans.TanProcesses[6].TanCode);
            Assert.Equal("iTAN", hitans.TanProcesses[6].Name);
        }

        [Fact]
        public void Test_HITANS_7()
        {
            var rawSegment = @"HITANS:175:7:4+1+1+1+N:N:0:922:2:pushTAN-dec:Decoupled::pushTAN 2.0:::Aufforderung:2048:J:2:N:0:0:N:N:00:2:N:2:180:1:1:J:J'";
            var segment = new Segment(rawSegment);
            segment = new GenericSegmentParser().ParseSegment(segment);

            var parser = new HITANSSegmentParser();
            var hitans = (HITANS) parser.ParseSegment(segment);

            Assert.Equal("HITANS", hitans.Name);
            Assert.Single(hitans.TanProcesses);
            Assert.Equal(922, hitans.TanProcesses[0].TanCode);
            Assert.Equal("pushTAN 2.0", hitans.TanProcesses[0].Name);
        }
    }
}
