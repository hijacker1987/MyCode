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
            var expectedRole = "TestRole";
            var expectedId = "test123";

            // Act
            var authResponse = new AuthResponse(expectedRole, expectedId);

            // Assert
            Assert.Equal(expectedRole, authResponse.Role);
            Assert.Equal(expectedId, authResponse.Id);
        }

        [Fact]
        public void AuthResponse_ShouldHaveCorrectProperties()
        {
            // Arrange
            var response = new AuthResponse("TestRole", "test123");

            // Act & Assert
            Assert.Equal("TestRole", response.Role);
            Assert.Equal("test123", response.Id);
        }
    }
}
