using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MyCode_Backend_Server.Models.Tests
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void UserConstructor_SetsPropertiesCorrectly()
        {
            // Arrange
            var codes = new List<Code> { new Code("code1"), new Code("code2") };

            // Act
            var user = new User(codes);

            // Assert
            Assert.IsNotNull(user.Code);
            Assert.AreEqual(2, user.Code.Count);
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
            string myCode = "testCode";
            bool isVisible = true;

            // Act
            Code code = new Code(myCode, isVisible);

            // Assert
            Assert.AreEqual(myCode, code.MyCode);
            Assert.AreEqual(isVisible, code.IsVisible);
        }

        [TestMethod]
        public void CodeDefaultConstructor_SetsPropertiesWithDefaults()
        {
            // Act
            Code code = new Code();

            // Assert
            Assert.AreEqual(default(Guid), code.Id);
            Assert.IsNull(code.UserId);
            Assert.IsNull(code.User);
            Assert.IsNull(code.MyCode);
            Assert.IsFalse(code.IsVisible);
        }
    }
}
