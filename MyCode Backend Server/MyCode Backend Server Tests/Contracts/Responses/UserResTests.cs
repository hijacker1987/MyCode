using Microsoft.AspNetCore.Routing;
using MyCode_Backend_Server.Contracts.Registers;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Contracts.Responses
{
    public class UserResTests
    {
        [Fact]
        public void UserRegResponse_PropertiesMatch_ExpectedValues()
        {
            // Arrange
            var expectedEmail = "testuser@example.com";
            var expectedUserName = "testuser";

            // Act
            var userRegResponse = new UserRegResponse(expectedEmail, expectedUserName);

            // Assert
            Assert.Equal(expectedEmail, userRegResponse.Email);
            Assert.Equal(expectedUserName, userRegResponse.UserName);
        }

        [Fact]
        public void UserRegResponse_ShouldHaveCorrectProperties()
        {
            // Arrange
            var response = new UserRegResponse("test@example.com", "TestUser");

            // Act & Assert
            Assert.Equal("test@example.com", response.Email);
            Assert.Equal("TestUser", response.UserName);
        }
    }
}
