using MyCode_Backend_Server.Contracts.Registers;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Contracts.Requests
{
    public class UserReqTests
    {

        [Fact]
        public void UserRegRequest_Validation_Success()
        {
            // Arrange
            var userRegRequest = new UserRegRequest
            (
                "Test@Title.Mail",
                "Hello World!",
                "Tester",
                "Test User",
                "123"
            );

            // Act
            var isValid = IsValid(userRegRequest);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void UserRegRequest_Validation_Fail_When_Email_InvalidFormat()
        {
            // Arrange
            var userRegRequest = new UserRegRequest
            (
                "InvalidEmail",
                "Hello World!",
                "Tester",
                "Test User",
                "123"
            );

            // Act
            var isValid = IsValid(userRegRequest);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void UserRegRequest_Validation_Fail_When_Required_Fields_Null_Or_Empty()
        {
            // Arrange
            var userRegRequest1 = new UserRegRequest
            (
                null!,
                "HelloWorld",
                "testPassword",
                "Test User",
                "123456789"
            );

            var userRegRequest2 = new UserRegRequest
            (
                "test@example.com",
                string.Empty,
                "StrongPassword123",
                "Test User",
                "123456789"
            );

            var userRegRequest3 = new UserRegRequest
            (
                "test@example.com",
                "HelloWorld",
                null!,
                "Test User",
                "123456789"
            );

            var userRegRequest4 = new UserRegRequest
            (
                "test@example.com",
                "HelloWorld",
                "testPassword",
                string.Empty,
                "123456789"
            );

            var userRegRequest5 = new UserRegRequest
            (
                "test@example.com",
                "HelloWorld",
                "testPassword",
                "Test User",
                null!
            );

            // Act & Assert
            Assert.False(IsValid(userRegRequest1, nameof(UserRegRequest.Email)));
            Assert.False(IsValid(userRegRequest2, nameof(UserRegRequest.Username)));
            Assert.False(IsValid(userRegRequest3, nameof(UserRegRequest.Password)));
            Assert.False(IsValid(userRegRequest4, nameof(UserRegRequest.DisplayName)));
            Assert.False(IsValid(userRegRequest5, nameof(UserRegRequest.PhoneNumber)));
        }

        [Fact]
        public void UserRegRequest_Validation_Fail_When_Password_TooShort()
        {
            // Arrange
            var userRegRequest = new UserRegRequest
            (
                "test@example.com",
                "HelloWorld",
                "Short",
                "Test User",
                "123456789"
            );

            // Act
            var isValid = IsValid(userRegRequest, nameof(UserRegRequest.Password));

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void UserRegRequest_Validation_Fail_When_Password_TooLong()
        {
            // Arrange
            var userRegRequest = new UserRegRequest
            (
                "test@example.com",
                "HelloWorld",
                new string('A', UserRegRequest.MaxPasswordLength + 1),
                "Test User",
                "123456789"
            );

            // Act
            var isValid = IsValid(userRegRequest, nameof(UserRegRequest.Password));

            // Assert
            Assert.False(isValid);
        }

        private static bool IsValid(object instance, string propertyName = null!)
        {
            var validationContext = new ValidationContext(instance, null, null);
            var validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(instance, validationContext, validationResults, true);
        }
    }
}
