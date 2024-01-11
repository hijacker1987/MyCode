
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
            Assert.IsNotNull(user.Code);
            Assert.AreEqual(2, user.Code.Count);
            Assert.AreEqual(default, user.Id);
            Assert.AreEqual("Test User", user.DisplayName);
            Assert.AreEqual(time, user.LastTimeLogin);
            Assert.AreEqual("Test", user.UserName);
            Assert.AreEqual("123", user.PhoneNumber);
            Assert.AreEqual ("testmail@test.ts", user.Email);
        }

        [TestMethod]
        public void UserDefaultConstructor_CreatesUserWithEmptyCodeList()
        {
            // Act
            var user = new User();

            // Assert
            Assert.IsNotNull(user.Code);
            Assert.AreEqual(0, user.Code.Count);
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
            Assert.AreEqual(codeTitle, code.CodeTitle);
            Assert.AreEqual(myCode, code.MyCode);
            Assert.AreEqual(whatKindOfCode, code.WhatKindOfCode);
            Assert.AreEqual(isBackend, code.IsBackend);
            Assert.AreEqual(isVisible, code.IsVisible);
        }

        [TestMethod]
        public void CodeDefaultConstructor_SetsPropertiesWithDefaults()
        {
            // Act
            Code code = new ();

            // Assert
            Assert.AreEqual(default, code.Id);
            Assert.IsNotNull(code.UserId);
            Assert.IsNull(code.User);
            Assert.IsNull(code.CodeTitle);
            Assert.IsNull(code.MyCode);
            Assert.IsNull(code.WhatKindOfCode);
            Assert.IsFalse(code.IsBackend);
            Assert.IsFalse(code.IsVisible);
        }
    }
}
