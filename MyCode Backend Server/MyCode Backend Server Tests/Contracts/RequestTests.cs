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

        [Fact]
        public void UserRegRequest_Validation_Fail_When_Required_Fields_Null_Or_Empty()
        {
            // Arrange
            var userRegRequest1 = new UserRegRequest
            (
                "test@example.com",
                null!,
                "StrongPassword123",
                "Test User",
                "123456789"
            );

            var userRegRequest2 = new UserRegRequest
            (
                "test@example.com",
                "HelloWorld",
                null!,
                "Test User",
                "123456789"
            );

            // Act & Assert
            Assert.False(IsValid(userRegRequest1, nameof(UserRegRequest.Username)));
            Assert.False(IsValid(userRegRequest2, nameof(UserRegRequest.Password)));
        }

        [Fact]
        public void CodeRegRequest_Validation_Fail_When_CodeContent_Is_Null()
        {
            // Arrange
            var codeRegRequest = new CodeRegRequest
            (
                "Test Title",
                null!,
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
        public void AuthRegRequest_Validation_Fail_When_Password_Null_Or_Empty()
        {
            // Arrange
            var authRegRequest1 = new AuthRequest("test@example.com", null!);
            var authRegRequest2 = new AuthRequest("test@example.com", string.Empty);
            var authRegRequest3 = new AuthRequest("test@example.com", null!);

            // Act & Assert
            Assert.False(IsValid(authRegRequest1, nameof(AuthRequest.Password)));
            Assert.False(IsValid(authRegRequest2, nameof(AuthRequest.Password)));
            Assert.False(IsValid(authRegRequest3, nameof(AuthRequest.Password)));
        }

        [Fact]
        public void ChangePassRequest_Validation_Fail_When_CurrentPassword_Null_Or_Empty()
        {
            // Arrange
            var changePassRegRequest1 = new ChangePassRequest("test@example.com", null!, "NewPassword");
            var changePassRegRequest2 = new ChangePassRequest("test@example.com", string.Empty, "NewPassword");
            var changePassRegRequest3 = new ChangePassRequest("test@example.com", null!, "NewPassword");

            // Act & Assert
            Assert.False(IsValid(changePassRegRequest1, nameof(ChangePassRequest.CurrentPassword)));
            Assert.False(IsValid(changePassRegRequest2, nameof(ChangePassRequest.CurrentPassword)));
            Assert.False(IsValid(changePassRegRequest3, nameof(ChangePassRequest.CurrentPassword)));
        }

        private static bool IsValid(object instance, string propertyName = null!)
        {
            var validationContext = new ValidationContext(instance, null, null);
            var validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(instance, validationContext, validationResults, true);
        }
    }
}
