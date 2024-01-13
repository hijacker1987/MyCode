using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Contracts.Services;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Contracts
{
    public class RequestTests
    {
        [Fact]
        public void CodeRegRequest_Validation_Success()
        {
            // Arrange
            var codeRegRequest = new CodeRegRequest
            (
                "Test Title",
                "console.log('Hello, World!');",
                "Test",
                true,
                true
            );

            // Act
            var isValid = IsValid(codeRegRequest);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void UserRegRequest_Validation_Success()
        {
            // Arrange
            var userRegRequest = new UserRegRequest
            (
                "Test@Title.Mail",
                "Hello World!",
                "Test",
                "Test User",
                "123"
            );

            // Act
            var isValid = IsValid(userRegRequest);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void AuthRegRequest_Validation_Success()
        {
            // Arrange
            var authRegRequest = new AuthRequest("Test@Title.Mail", "Test");

            // Act
            var isValid = IsValid(authRegRequest);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ChangePassRequest_Validation_Success()
        {
            // Arrange
            var changePassRegRequest = new ChangePassRequest("Test@Title.Mail", "Test", "Tester");

            // Act
            var isValid = IsValid(changePassRegRequest);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void CodeRegRequest_Validation_Fail_When_CodeTitle_Is_Null()
        {
            // Arrange
            var codeRegRequest = new CodeRegRequest
            (
                null!,
                "console.log('Hello, World!');",
                "Test",
                true,
                true
            );

            // Act
            var isValid = IsValid(codeRegRequest);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void UserRegRequest_Validation_Fail_When_Email_InvalidFormat()
        {
            // Arrange
            var userRegRequest = new UserRegRequest
            (
                "InvalidEmail",
                "Hello World!",
                "Test",
                "Test User",
                "123"
            );

            // Act
            var isValid = IsValid(userRegRequest);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void AuthRegRequest_Validation_Fail_When_Email_Null_Or_Empty()
        {
            // Arrange
            var authRegRequest1 = new AuthRequest("invalid-email", "Test");
            var authRegRequest2 = new AuthRequest(string.Empty, "Test");
            var authRegRequest3 = new AuthRequest(null!, "Test");

            // Act & Assert
            Assert.False(IsValid(authRegRequest1, nameof(AuthRequest.Email)));
            Assert.False(IsValid(authRegRequest2, nameof(AuthRequest.Email)));
            Assert.False(IsValid(authRegRequest3, nameof(AuthRequest.Email)));
        }

        [Fact]
        public void ChangePassRequest_Validation_Fail_When_NewPassword_TooShort()
        {
            // Arrange
            var changePassRegRequest = new ChangePassRequest("test@example.com", "Test", "Ab1");

            // Act & Assert
            Assert.False(IsValid(changePassRegRequest, nameof(ChangePassRequest.NewPassword)));
        }

        [Fact]
        public void ChangePassRequest_Validation_Success_When_NewPassword_Valid()
        {
            // Arrange
            var changePassRegRequest = new ChangePassRequest("test@example.com", "Test", "StrongPassword123");

            // Act & Assert
            Assert.True(IsValid(changePassRegRequest));
        }

        private static bool IsValid(object instance, string propertyName = null!)
        {
            var validationContext = new ValidationContext(instance, null, null);
            var validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(instance, validationContext, validationResults, true);
        }
    }
}
