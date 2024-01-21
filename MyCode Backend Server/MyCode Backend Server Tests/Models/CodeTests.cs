using MyCode_Backend_Server.Models;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Models
{
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
            Code code = new(codeTitle, myCode, whatKindOfCode, isBackend, isVisible);

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
            Code code = new();

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
