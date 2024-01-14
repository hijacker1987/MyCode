using MyCode_Backend_Server.Contracts.Services;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Contracts.Responses
{
    public class ChangePassResTests
    {
        [Fact]
        public void ChangePassResponse_PropertiesMatch_ExpectedValues()
        {
            // Arrange
            var expectedEmail = "testuser@example.com";

            // Act
            var changePassResponse = new ChangePassResponse(expectedEmail);

            // Assert
            Assert.Equal(expectedEmail, changePassResponse.Email);
        }

        [Fact]
        public void ChangePassResponse_ShouldHaveCorrectProperties()
        {
            // Arrange
            var response = new ChangePassResponse("test@example.com");

            // Act & Assert
            Assert.Equal("test@example.com", response.Email);
        }
    }
}
