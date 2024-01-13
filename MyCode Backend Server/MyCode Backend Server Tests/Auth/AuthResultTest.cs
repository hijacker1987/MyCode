using MyCode_Backend_Server.Service.Authentication;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Auth
{
    public class AuthResultTest
    {
        [Fact]
        public void AuthResult_Validation_Success()
        {
            // Arrange
            var codeRegRequest = new AuthResult
            (
                Id: "123",
                Success: true,
                Email: "Test@example.hu",
                UserName: "TestUser",
                Token: "TestToken"
            );

            // Act
            var validationContext = new ValidationContext(codeRegRequest, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(codeRegRequest, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
        }
    }
}
