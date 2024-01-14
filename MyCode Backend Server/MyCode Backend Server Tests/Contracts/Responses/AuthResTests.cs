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
            var expectedRole = "UserRole";

            // Act
            var authResponse = new AuthResponse(expectedEmail, expectedUsername, expectedToken, expectedRole);

            // Assert
            Assert.Equal(expectedEmail, authResponse.Email);
            Assert.Equal(expectedUsername, authResponse.Username);
            Assert.Equal(expectedToken, authResponse.Token);
            Assert.Equal(expectedRole, authResponse.Role);
        }

        [Fact]
        public void AuthResponse_ShouldHaveCorrectProperties()
        {
            // Arrange
            var response = new AuthResponse("test@example.com", "TestUser", "sampleToken", "UserRole");

            // Act & Assert
            Assert.Equal("test@example.com", response.Email);
            Assert.Equal("TestUser", response.Username);
            Assert.Equal("sampleToken", response.Token);
            Assert.Equal("UserRole", response.Role);
        }
    }
}
