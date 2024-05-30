using MyCode_Backend_Server.Models.MyCode_Backend_Server.Models;
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

            // Act
            var userProfile = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testUserName",
                Email = "testEmail@test.com",
                DisplayName = "testDisplayName",
                LastTimeLogin = time,
                PhoneNumber = "123456789"
            };

            // Assert
            Assert.NotEqual(default, userProfile.Id);
            Assert.Equal("testUserName", userProfile.UserName);
            Assert.Equal("testEmail@test.com", userProfile.Email);
            Assert.Equal("testDisplayName", userProfile.DisplayName);
            Assert.Equal(time, userProfile.LastTimeLogin);
            Assert.Equal("123456789", userProfile.PhoneNumber);
        }

        [TestMethod]
        [Fact]
        public void UserWithRoleConstructor_SetsPropertiesCorrectly()
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

            var userWithRole = new UserWithRole(user, "User");

            // Assert
            Assert.Equal(default, userWithRole.User.Id);
            Assert.Equal("Test User", userWithRole.User.DisplayName);
            Assert.Equal(time, userWithRole.User.LastTimeLogin);
            Assert.Equal("Test", userWithRole.User.UserName);
            Assert.Equal("123", userWithRole.User.PhoneNumber);
            Assert.Equal("testmail@test.ts", userWithRole.User.Email);
            Assert.Equal("User", userWithRole.Role);
        }

        [TestMethod]
        [Fact]
        public void UserWithRoleDefaultConstructor_SetsPropertiesWithDefaults()
        {
            // Act
            UserWithRole user = new();

            // Assert
            Assert.Equal(default, user.User.Id);
            Assert.Null(user.User.DisplayName);
            Assert.NotEqual(default, user.User.LastTimeLogin);
            Assert.Null(user.User.UserName);
            Assert.Null(user.User.PhoneNumber);
            Assert.Null(user.User.Email);
            Assert.Equal(string.Empty, user.Role);
        }
    }
}
