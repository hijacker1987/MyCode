using MyCode_Backend_Server.Contracts.Registers;
using System.ComponentModel.DataAnnotations;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server.Models.Tests
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
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
    }
}
