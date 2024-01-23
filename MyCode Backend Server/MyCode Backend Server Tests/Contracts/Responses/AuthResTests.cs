using MyCode_Backend_Server.Contracts.Services;
using Xunit;
using Assert = Xunit.Assert;
namespace MyCode_Backend_Server_Tests.Contracts.Responses
{
    public class AuthResTests
    {
        [Fact]
        public void AuthResponse_PropertiesMatch_ExpectedValues()
        {
            // Arrange
            var expectedEmail = "testuser@example.com";
            var expectedUsername = "testuser";
            var expectedToken = "myAccessToken";

            // Act
            var authResponse = new AuthResponse(expectedEmail, expectedUsername, expectedToken);

            // Assert
            Assert.Equal(expectedEmail, authResponse.Email);
            Assert.Equal(expectedUsername, authResponse.Username);
            Assert.Equal(expectedToken, authResponse.Token);
        }

        [Fact]
        public void AuthResponse_ShouldHaveCorrectProperties()
        {
            // Arrange
            var response = new AuthResponse("test@example.com", "TestUser", "sampleToken");

            // Act & Assert
            Assert.Equal("test@example.com", response.Email);
            Assert.Equal("TestUser", response.Username);
            Assert.Equal("sampleToken", response.Token);
        }
    }
}
