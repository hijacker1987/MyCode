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
            var codes = new List<Code> { new ("prime", "code1", "css"), new ("filter", "code2", "script") };
            var time = DateTime.Now;

            // Act
            var user = new User()
            {
                Id = default,
                DisplayName = "Test User",
                LastTimeLogin = time,
                UserName = "Test",
                PhoneNumber = "123",
                Email = "testmail@test.ts",
                Code = codes
            };

            // Assert
            Assert.NotNull(user.Code);
            Assert.Equal(2, user.Code!.Count);
            Assert.Equal(default, user.Id);
            Assert.Equal("Test User", user.DisplayName);
            Assert.Equal(time, user.LastTimeLogin);
            Assert.Equal("Test", user.UserName);
            Assert.Equal("123", user.PhoneNumber);
            Assert.Equal ("testmail@test.ts", user.Email);
        }

        [TestMethod]
        public void UserDefaultConstructor_CreatesUserWithEmptyCodeList()
        {
            // Act
            var user = new User();

            // Assert
            Assert.Empty(user.Code!);
        }
    }

    [TestClass]
    public class CodeTests
    {
        [TestMethod]
        public void CodeConstructor_SetsPropertiesCorrectly()
        {
            // Arrange
            string codeTitle = "testTitle";
            string myCode = "testCode";
            string whatKindOfCode = "testMethod";
            bool isBackend = true;
            bool isVisible = true;

            // Act
            Code code = new (codeTitle, myCode, whatKindOfCode, isBackend, isVisible);

            // Assert
            Assert.Equal(codeTitle, code.CodeTitle);
            Assert.Equal(myCode, code.MyCode);
            Assert.Equal(whatKindOfCode, code.WhatKindOfCode);
            Assert.Equal(isBackend, code.IsBackend);
            Assert.Equal(isVisible, code.IsVisible);
        }

        [TestMethod]
        public void CodeDefaultConstructor_SetsPropertiesWithDefaults()
        {
            // Act
            Code code = new ();

            // Assert
            Assert.Equal(default, code.Id);
            Assert.Null(code.User);
            Assert.Null(code.CodeTitle);
            Assert.Null(code.MyCode);
            Assert.Null(code.WhatKindOfCode);
            Assert.False(code.IsBackend);
            Assert.False(code.IsVisible);
        }
    }
}
