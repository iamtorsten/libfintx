using System;
using Xunit;
using libfintx.FinTS;
using System.Collections.Generic;
using libfintx.Sepa;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using libfintx.Sepa.Helper;


namespace libfintx.Tests.Pain
{
    public class pain00100103Tests
    {
        [Fact]
        public void Test_Escape()
        {
            string str = SepaHelper.Escape(@"Hübner;;;\\\");
            Assert.Equal("Huebner", str);

            str = SepaHelper.Escape(@"Der Verwendungszweck der Überweisung ####ist die Mietzahlung.");
            Assert.Equal("Der Verwendungszweck der Ueberweisung ist die Mietzahlung.", str);
        }

        [Fact(Skip = "You have to set the Arrange variables for this test")]
        public void Create_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            string Accountholder = null;
            string AccountholderIBAN = null;
            string AccountholderBIC = null;
            string Receiver = null;
            string ReceiverIBAN = null;
            string ReceiverBIC = null;
            decimal Amount = 0;
            string Usage = null;
            DateTime ExecutionDay = default(global::System.DateTime);

            // Act
            var result = pain00100103.Create(
                Accountholder,
                AccountholderIBAN,
                AccountholderBIC,
                Receiver,
                ReceiverIBAN,
                ReceiverBIC,
                Amount,
                Usage,
                ExecutionDay);

            // Assert
            Assert.True(false);
        }

        [Fact(Skip = "You have to set the Arrange variables for this test")]
        public void Create_StateUnderTest_ExpectedBehavior1()
        {
            // Arrange
            string Accountholder = null;
            string AccountholderIBAN = null;
            string AccountholderBIC = null;
            List<Pain00100203CtData> PainData = null;
            string NumberofTransactions = null;
            decimal TotalAmount = 0;
            DateTime ExecutionDay = default(global::System.DateTime);

            // Act
            var result = pain00100103.Create(
                Accountholder,
                AccountholderIBAN,
                AccountholderBIC,
                PainData,
                NumberofTransactions,
                TotalAmount,
                ExecutionDay);

            // Assert
            Assert.True(false);
        }
    }
}
