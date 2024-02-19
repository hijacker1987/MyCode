using MyCode_Backend_Server.Contracts.Services;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Contracts.Requests
{
    public class ChangePassReqTests
    {

        [Fact]
        public void ChangePassRequest_Validation_Success()
        {
            // Arrange
            var changePassRegRequest = new ChangePassRequest("Test@Title.Mail", "Tester", "Tested", "Tested");

            // Act
            var isValid = IsValid(changePassRegRequest);

            // Assert
            Assert.True(isValid);
        }


        [Fact]
        public void ChangePassRequest_Validation_Fail_When_NewPassword_TooShort()
        {
            // Arrange
            var changePassRegRequest = new ChangePassRequest("test@example.com", "Tester", "Ab1", "Ab1");

            // Act & Assert
            Assert.False(IsValid(changePassRegRequest, nameof(ChangePassRequest.NewPassword)));
        }

        [Fact]
        public void ChangePassRequest_Validation_Success_When_NewPassword_Valid()
        {
            // Arrange
            var changePassRegRequest = new ChangePassRequest("test@example.com", "Tester", "StrongPassword123", "StrongPassword123");

            // Act & Assert
            Assert.True(IsValid(changePassRegRequest));
        }

        [Fact]
        public void ChangePassRequest_Validation_Fail_When_Current_Or_ConfirmPassword_Null_Or_Empty()
        {
            // Arrange
            var changePassRegRequest1 = new ChangePassRequest("test@example.com", null!, "NewPassword", "NewPassword");
            var changePassRegRequest2 = new ChangePassRequest("test@example.com", string.Empty, "NewPassword", "NewPassword");
            var changePassRegRequest3 = new ChangePassRequest("test@example.com", "Tester", "NewPassword", null!);
            var changePassRegRequest4 = new ChangePassRequest("test@example.com", "Tester", "NewPassword", string.Empty);

            // Act & Assert
            Assert.False(IsValid(changePassRegRequest1, nameof(ChangePassRequest.CurrentPassword)));
            Assert.False(IsValid(changePassRegRequest2, nameof(ChangePassRequest.CurrentPassword)));
            Assert.False(IsValid(changePassRegRequest3, nameof(ChangePassRequest.ConfirmPassword)));
            Assert.False(IsValid(changePassRegRequest3, nameof(ChangePassRequest.ConfirmPassword)));

        }

        [Fact]
        public void ChangePassRequest_Validation_Fail_When_Current_And_ConfirmPassword_TooShort()
        {
            // Arrange
            var changePassRegRequest = new ChangePassRequest("test@example.com", "Ab1", "StrongPassword123", "Ab1");

            // Act & Assert
            Assert.False(IsValid(changePassRegRequest, nameof(ChangePassRequest.CurrentPassword)));
        }

        [Fact]
        public void ChangePassRequest_Validation_Fail_When_InvalidEmail()
        {
            // Arrange
            var changePassRegRequest = new ChangePassRequest("testexample.com", "Ab1234", "StrongPassword123", "StrongPassword123");

            // Act & Assert
            Assert.False(IsValid(changePassRegRequest, nameof(ChangePassRequest.CurrentPassword)));
        }

        [Fact]
        public void ChangePassRequest_Validation_Fail_When_Passwords_Not_Matching()
        {
            // Arrange
            var changePassRegRequest = new ChangePassRequest("test@example.com", "Tester", "DifferentPassword", "Tested");

            // Act & Assert
            Assert.False(IsValid(changePassRegRequest));
        }

        [Fact]
        public void ChangePassRequest_Validation_Fail_When_NewPassword_TooLong()
        {
            // Arrange
            var changePassRegRequest = new ChangePassRequest("test@example.com", "Tester", new string('A', ChangePassRequest.MaxPasswordLength + 1), "Tester");

            // Act & Assert
            Assert.False(IsValid(changePassRegRequest, nameof(ChangePassRequest.NewPassword)));
        }

        private static bool IsValid(object instance, string propertyName = null!)
        {
            var validationContext = new ValidationContext(instance, null, null);
            var validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(instance, validationContext, validationResults, true);
        }
    }
}
