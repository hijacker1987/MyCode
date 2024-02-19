using MyCode_Backend_Server.Contracts.Services;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Contracts.Requests
{
    public class AuthReqTests
    {
        [Fact]
        public void AuthRegRequest_Validation_Success()
        {
            // Arrange
            var authRegRequest = new AuthRequest("Test@Title.Mail", "Tester", "Tester");

            // Act
            var isValid = IsValid(authRegRequest);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void AuthRegRequest_Validation_Fail_When_Email_Null_Or_Empty()
        {
            // Arrange
            var authRegRequest1 = new AuthRequest(string.Empty, "Tester", "Tester");
            var authRegRequest2 = new AuthRequest(null!, "Tester", "Tester");

            // Act & Assert
            Assert.False(IsValid(authRegRequest1, nameof(AuthRequest.Email)));
            Assert.False(IsValid(authRegRequest2, nameof(AuthRequest.Email)));
        }

        [Fact]
        public void AuthRegRequest_Validation_Fail_When_Password_Null_Or_Empty()
        {
            // Arrange
            var authRegRequest1 = new AuthRequest("test@example.com", null!, "Tester");
            var authRegRequest2 = new AuthRequest("test@example.com", string.Empty, "Tester");
            var authRegRequest3 = new AuthRequest("test@example.com", "Tester", null!);
            var authRegRequest4 = new AuthRequest("test@example.com", "Tester", string.Empty);

            // Act & Assert
            Assert.False(IsValid(authRegRequest1, nameof(AuthRequest.Password)));
            Assert.False(IsValid(authRegRequest2, nameof(AuthRequest.Password)));
            Assert.False(IsValid(authRegRequest3, nameof(AuthRequest.ConfirmPassword)));
            Assert.False(IsValid(authRegRequest4, nameof(AuthRequest.ConfirmPassword)));
        }

        [Fact]
        public void AuthRequest_Validation_Fail_When_Email_Invalid_Format()
        {
            // Arrange
            var authRegRequest = new AuthRequest("InvalidEmail", "Tester", "Tester");

            // Act
            var isValid = IsValid(authRegRequest, nameof(AuthRequest.Email));

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void AuthRegRequest_Validation_Fail_When_Password_TooShort()
        {
            // Arrange
            var authRegRequest = new AuthRequest("test@example.com", "Short", "Short");

            // Act
            var isValid = IsValid(authRegRequest, nameof(AuthRequest.Password));

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void AuthRegRequest_Validation_Fail_When_Password_TooLong()
        {
            // Arrange
            var authRegRequest = new AuthRequest("test@example.com", new string('A', AuthRequest.MaxPasswordLength + 1), new string('A', AuthRequest.MaxPasswordLength + 1));

            // Act
            var isValid = IsValid(authRegRequest, nameof(AuthRequest.Password));

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void AuthRegRequest_Validation_Fail_When_Email_And_Password_Invalid()
        {
            // Arrange
            var authRegRequest = new AuthRequest("InvalidEmail", "Short", "Short");

            // Act
            var isValid = IsValid(authRegRequest);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void AuthRegRequest_Validation_Fail_When_Email_Without_AtSymbol()
        {
            // Arrange
            var authRegRequest = new AuthRequest("testexample.com", "Tester", "Tester");

            // Act
            var isValid = IsValid(authRegRequest, nameof(AuthRequest.Email));

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void AuthRegRequest_Validation_Fail_When_Password_Null_Or_Whitespace()
        {
            // Arrange
            var authRegRequest1 = new AuthRequest("test@example.com", null!, "Tester");
            var authRegRequest2 = new AuthRequest("test@example.com", "   ", "Tester");
            var authRegRequest3 = new AuthRequest("test@example.com", "Tester", null!);
            var authRegRequest4 = new AuthRequest("test@example.com", "Tester", "   ");

            // Act & Assert
            Assert.False(IsValid(authRegRequest1, nameof(AuthRequest.Password)));
            Assert.False(IsValid(authRegRequest2, nameof(AuthRequest.Password)));
            Assert.False(IsValid(authRegRequest3, nameof(AuthRequest.ConfirmPassword)));
            Assert.False(IsValid(authRegRequest4, nameof(AuthRequest.ConfirmPassword)));
        }

        [Fact]
        public void AuthRegRequest_Validation_Fail_When_Email_TooLong()
        {
            // Arrange
            var authRegRequest = new AuthRequest(new string('a', 320) + "@example.com", "Password123", "Password123");

            // Act
            var isValid = IsValid(authRegRequest, nameof(AuthRequest.Email));

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
