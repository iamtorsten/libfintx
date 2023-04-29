using System.Threading.Tasks;
using Xunit;
using libfintx.FinTS;

namespace libfintx.Tests.Transactions
{
    public class INITests
    {
        [Fact(Skip = "You have to set the Arrange variables for this test")]
        public async Task Init_INI_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            FinTsClient client = null;
            string hkTanSegmentId = null;

            // Act
            var result = await INI.Init_INI(
                client,
                hkTanSegmentId);

            // Assert
            Assert.True(false);
        }
    }
}
