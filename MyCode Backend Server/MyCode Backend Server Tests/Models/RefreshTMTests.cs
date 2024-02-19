using MyCode_Backend_Server.Models;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Models
{
    public class RefreshTMTests
    {
        [Fact]
        public void RefreshTM_Validation_Success()
        {
            // Arrange
            var refreshTM = new RefreshTM
            {
                AccessToken = "testAccessToken",
                RefreshToken = "testRefreshToken"
            };

            // Act & Assert
            Assert.Equal("testAccessToken", refreshTM.AccessToken);
            Assert.Equal("testRefreshToken", refreshTM.RefreshToken);
        }
    }
}
