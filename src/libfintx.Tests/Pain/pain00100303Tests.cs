using System;
using Xunit;
using libfintx;
using System.Collections.Generic;

namespace libfintx.Tests.Pain
{
    public class pain00100303Tests
    {
        [Fact]
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
            var result = pain00100303.Create(
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

        [Fact]
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
            var result = pain00100303.Create(
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
