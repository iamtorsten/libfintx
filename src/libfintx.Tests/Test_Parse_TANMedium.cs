using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace libfintx.Tests
{
    public class Test_Parse_TANMedium
    {
        [Fact]
        public void Test_1()
        {
            var result = Helper.Parse_TANMedium("HITAB:5:4:3+0+A:1:::::::::::Handy::::::::+A:2:::::::::::iPhone Abid::::::::");

            Assert.Equal(2, result?.Count);
            Assert.Equal("Handy", result[0]);
            Assert.Equal("iPhone Abid", result[1]);
        }

        [Fact]
        public void Test_2()
        {
            var result = Helper.Parse_TANMedium("HITAB:4:4:3+0+M:1:::::::::::mT?:MFN1:********0340'");

            Assert.Equal(1, result?.Count);
            Assert.Equal("mT?:MFN1", result[0]);
        }

        [Fact]
        public void Test_3()
        {
            var result = Helper.Parse_TANMedium("HITAB:5:4:3+0+M:2:::::::::::Unregistriert 1::01514/654321::::::+M:1:::::::::::Handy:*********4321:::::::");

            Assert.Equal(2, result?.Count);
            Assert.Equal("Unregistriert 1", result[0]);
            Assert.Equal("Handy", result[1]);
        }

        [Fact]
        public void Test_4()
        {
            var result = Helper.Parse_TANMedium("HITAB:4:4:3+0+M:1:::::::::::mT?:MFN1:********0340+G:1:SO?:iPhone:00:::::::::SO?:iPhone");

            Assert.Equal(2, result?.Count);
            Assert.Equal("mT?:MFN1", result[0]);
            Assert.Equal("SO?:iPhone", result[1]);
        }
    }
}
