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
            var changePassRegRequest = new ChangePassRequest("Test@Title.Mail", "Tester", "Tester");

            // Act
            var isValid = IsValid(changePassRegRequest);

            // Assert
            Assert.True(isValid);
        }


        [Fact]
        public void ChangePassRequest_Validation_Fail_When_NewPassword_TooShort()
        {
            // Arrange
            var changePassRegRequest = new ChangePassRequest("test@example.com", "Tester", "Ab1");

            // Act & Assert
            Assert.False(IsValid(changePassRegRequest, nameof(ChangePassRequest.NewPassword)));
        }

        [Fact]
        public void ChangePassRequest_Validation_Success_When_NewPassword_Valid()
        {
            // Arrange
            var changePassRegRequest = new ChangePassRequest("test@example.com", "Tester", "StrongPassword123");

            // Act & Assert
            Assert.True(IsValid(changePassRegRequest));
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

        [Fact]
        public void ChangePassRequest_Validation_Fail_When_CurrentPassword_TooShort()
        {
            // Arrange
            var changePassRegRequest = new ChangePassRequest("test@example.com", "Ab1", "StrongPassword123");

            // Act & Assert
            Assert.False(IsValid(changePassRegRequest, nameof(ChangePassRequest.CurrentPassword)));
        }

        [Fact]
        public void ChangePassRequest_Validation_Fail_When_InvalidEmail()
        {
            // Arrange
            var changePassRegRequest = new ChangePassRequest("testexample.com", "Ab1234", "StrongPassword123");

            // Act & Assert
            Assert.False(IsValid(changePassRegRequest, nameof(ChangePassRequest.CurrentPassword)));
        }


        private static bool IsValid(object instance, string propertyName = null!)
        {
            var validationContext = new ValidationContext(instance, null, null);
            var validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(instance, validationContext, validationResults, true);
        }
    }
}
