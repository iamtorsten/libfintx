using System;
using Xunit;
using libfintx;
using System.Collections.Generic;

namespace libfintx.Tests.Pain
{
    public class pain00800202Tests
    {
        [Fact]
        public void Create_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            string Accountholder = null;
            string AccountholderIBAN = null;
            string AccountholderBIC = null;
            string Payer = null;
            string PayerIBAN = null;
            string PayerBIC = null;
            decimal Amount = 0;
            string Usage = null;
            DateTime SettlementDate = default(global::System.DateTime);
            string MandateNumber = null;
            DateTime MandateDate = default(global::System.DateTime);
            string CeditorIDNumber = null;

            // Act
            var result = pain00800202.Create(
                Accountholder,
                AccountholderIBAN,
                AccountholderBIC,
                Payer,
                PayerIBAN,
                PayerBIC,
                Amount,
                Usage,
                SettlementDate,
                MandateNumber,
                MandateDate,
                CeditorIDNumber);

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
            DateTime SettlementDate = default(global::System.DateTime);
            List<Pain00800202CcData> PainData = null;
            string NumberofTransactions = null;
            decimal TotalAmount = 0;

            // Act
            var result = pain00800202.Create(
                Accountholder,
                AccountholderIBAN,
                AccountholderBIC,
                SettlementDate,
                PainData,
                NumberofTransactions,
                TotalAmount);

            // Assert
            Assert.True(false);
        }
    }
}
