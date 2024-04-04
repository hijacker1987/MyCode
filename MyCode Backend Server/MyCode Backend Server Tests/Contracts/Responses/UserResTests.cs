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
            var expectedId = Guid.NewGuid().ToString();
            var expectedEmail = "testuser@example.com";
            var expectedUserName = "testuser";
            var expectedDisplayName = "Test User";
            var expectedPhoneNumber = "123";
            var expectedRole = "Role";

            // Act
            var userRegResponse = new UserRegResponse(expectedId, expectedEmail, expectedUserName, expectedDisplayName, expectedPhoneNumber, expectedRole);

            // Assert
            Assert.Equal(expectedId, userRegResponse.Id);
            Assert.Equal(expectedEmail, userRegResponse.Email);
            Assert.Equal(expectedUserName, userRegResponse.UserName);
            Assert.Equal(expectedDisplayName, userRegResponse.DisplayName);
            Assert.Equal(expectedPhoneNumber, userRegResponse.PhoneNumber);
            Assert.Equal(expectedRole, userRegResponse.Role);
        }

        [Fact]
        public void UserRegResponse_ShouldHaveCorrectProperties()
        {
            // Arrange
            var response = new UserRegResponse("t3St-iD", "test@example.com", "TestUser", "Display User", "321", "User");

            // Act & Assert
            Assert.Equal("t3St-iD", response.Id);
            Assert.Equal("test@example.com", response.Email);
            Assert.Equal("TestUser", response.UserName);
            Assert.Equal("Display User", response.DisplayName);
            Assert.Equal("321", response.PhoneNumber);
        }
    }
}
