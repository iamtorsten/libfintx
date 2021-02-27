using Xunit;

namespace libfintx.Tests
{
    public class Test_Parse_Balance
    {
        [Fact]
        public void Test_1()
        {
            var message = "HNHBK:1:3+000000000468+300+n4kfRJCpJK4D20210227110433521%+2+n4kfRJCpJK4D20210227110433521%:2'HNVSK:998:3+PIN:1+998+1+2::3791439560898000HYC8K5V5P7UPX9+1:20210227:110435+2:2:13:@8@        :5:1+280:70070010:XXXXXX:V:0:0+0'HNVSD:999:1+@212@HIRMG:2:2+0010::Nachricht entgegengenommen.'HIRMS:3:2:3+0020::Auftrag ausgeführt.'HISAL:4:7:3+DE02100701240123456789:DEUTDEDB101:123456789:EUR:280:10070124+SparCard+EUR+C:E-9,:EUR:20210227:110435++0,:EUR++0,:EUR''HNHBS:5:1+2'";
            var result = Helper.Parse_Balance(message);

            Assert.Equal(0, result.Balance);
        }
    }
}
