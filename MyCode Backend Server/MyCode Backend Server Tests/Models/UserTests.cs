using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server.Models.Tests
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        [Fact]
        public void UserConstructor_SetsPropertiesCorrectly()
        {
            // Arrange
            var time = DateTime.Now;

            // Act
            var user = new User()
            {
                Id = default,
                DisplayName = "Test User",
                LastTimeLogin = time,
                UserName = "Test",
                PhoneNumber = "123",
                Email = "testmail@test.ts"
            };

            // Assert
            Assert.Equal(default, user.Id);
            Assert.Equal("Test User", user.DisplayName);
            Assert.Equal(time, user.LastTimeLogin);
            Assert.Equal("Test", user.UserName);
            Assert.Equal("123", user.PhoneNumber);
            Assert.Equal ("testmail@test.ts", user.Email);
        }

        [TestMethod]
        [Fact]
        public void UserDefaultConstructor_SetsPropertiesWithDefaults()
        {
            // Act
            User user = new();

            // Assert
            Assert.Equal(default, user.Id);
            Assert.Null(user.DisplayName);
            Assert.NotEqual(default, user.LastTimeLogin);
            Assert.Null(user.UserName);
            Assert.Null(user.PhoneNumber);
            Assert.Null(user.Email);
        }

        [Fact]
        public void UserProfile_SetterMethods_ModifyPropertiesCorrectly()
        {
            // Arrange
            var time = DateTime.Now;
            var userProfile = new User();

            // Act
            userProfile.Id = Guid.NewGuid();
            userProfile.UserName = "testUserName";
            userProfile.Email = "testEmail@test.com";
            userProfile.DisplayName = "testDisplayName";
            userProfile.LastTimeLogin = time;
            userProfile.PhoneNumber = "123456789";

            // Assert
            Assert.NotEqual(default(Guid), userProfile.Id);
            Assert.Equal("testUserName", userProfile.UserName);
            Assert.Equal("testEmail@test.com", userProfile.Email);
            Assert.Equal("testDisplayName", userProfile.DisplayName);
            Assert.Equal(time, userProfile.LastTimeLogin);
            Assert.Equal("123456789", userProfile.PhoneNumber);
        }
    }
}
