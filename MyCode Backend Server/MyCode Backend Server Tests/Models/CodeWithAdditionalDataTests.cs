using MyCode_Backend_Server.Models;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Models
{
    public class CodeWithAdditionalDataTests
    {
        [Fact]
        public void CodeWithAdditionalData_SetDisplayName()
        {
            // Arrange
            var codeWithAdditionalData = new CodeWithAdditionalData();

            // Act
            codeWithAdditionalData.DisplayName = "TestDisplayName";

            // Assert
            Assert.Equal("TestDisplayName", codeWithAdditionalData.DisplayName);
        }
    }
}
