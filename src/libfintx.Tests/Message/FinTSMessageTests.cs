using System.Threading.Tasks;
using libfintx.FinTS;
using libfintx.FinTS.Message;
using Xunit;

namespace libfintx.Tests.Message
{
    public class FinTSMessageTests
    {
        [Fact(Skip = "You have to set the Arrange variables for this test")]
        public void CreateSync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            FinTsClient client = null;
            string Segments = null;

            // Act
            var result = FinTSMessage.CreateSync(
                client,
                Segments);

            // Assert
            Assert.True(false);
        }

        [Fact(Skip = "You have to set the Arrange variables for this test")]
        public void Create_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            FinTsClient client = null;
            int MsgNum = 0;
            string DialogID = null;
            string Segments = null;
            string HIRMS_TAN = null;
            string SystemID = null;

            // Act
            var result = FinTSMessage.Create(
                client,
                MsgNum,
                DialogID,
                Segments,
                HIRMS_TAN,
                SystemID);

            // Assert
            Assert.True(false);
        }

        [Fact(Skip = "You have to set the Arrange variables for this test")]
        public async Task Send_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            FinTsClient client = null;
            string Message = null;

            // Act
            var result = await FinTSMessage.Send(
                client,
                Message);

            // Assert
            Assert.True(false);
        }
    }
}
