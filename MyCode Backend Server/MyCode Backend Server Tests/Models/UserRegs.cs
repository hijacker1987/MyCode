using System.ComponentModel.DataAnnotations;
using Xunit;
using MyCode_Backend_Server.Contracts.Registers;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Models
{
    public class UserRegs
    {
        private List<ValidationResult> ValidateModel(UserRegRequest model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void UserRegRequest_CanBeCreated_WithValidData()
        {
            // Arrange
            var email = "valid@example.com";
            var username = "validUsername";
            var password = "ValidPassword1";
            var displayName = "Valid Display Name";
            var phoneNumber = "123-456-7890";

            // Act
            var request = new UserRegRequest(email, username, password, displayName, phoneNumber);
            var validationResults = ValidateModel(request);

            // Assert
            Assert.Equal(email, request.Email);
            Assert.Equal(username, request.Username);
            Assert.Equal(password, request.Password);
            Assert.Equal(displayName, request.DisplayName);
            Assert.Equal(phoneNumber, request.PhoneNumber);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void UserRegRequest_InvalidEmail_ReturnsValidationError()
        {
            // Arrange
            var email = "invalidEmail";
            var username = "validUsername";
            var password = "ValidPassword1";
            var displayName = "Valid Display Name";
            var phoneNumber = "123-456-7890";

            // Act
            var request = new UserRegRequest(email, username, password, displayName, phoneNumber);
            var validationResults = ValidateModel(request);

            // Assert
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(UserRegRequest.Email)));
        }

        [Fact]
        public void UserRegRequest_ShortPassword_ReturnsValidationError()
        {
            // Arrange
            var email = "valid@example.com";
            var username = "validUsername";
            var password = "short";
            var displayName = "Valid Display Name";
            var phoneNumber = "123-456-7890";

            // Act
            var request = new UserRegRequest(email, username, password, displayName, phoneNumber);
            var validationResults = ValidateModel(request);

            // Assert
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(UserRegRequest.Password)));
        }

        [Fact]
        public void UserRegRequest_LongPassword_ReturnsValidationError()
        {
            // Arrange
            var email = "valid@example.com";
            var username = "validUsername";
            var password = new string('a', 21);
            var displayName = "Valid Display Name";
            var phoneNumber = "123-456-7890";

            // Act
            var request = new UserRegRequest(email, username, password, displayName, phoneNumber);
            var validationResults = ValidateModel(request);

            // Assert
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(UserRegRequest.Password)));
        }

        [Fact]
        public void UserRegRequest_MissingRequiredFields_ReturnsValidationErrors()
        {
            // Arrange
            var request = new UserRegRequest(null!, null!, null!, null!, null!);
            var validationResults = ValidateModel(request);

            // Assert
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(UserRegRequest.Email)));
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(UserRegRequest.Username)));
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(UserRegRequest.Password)));
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(UserRegRequest.DisplayName)));
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(UserRegRequest.PhoneNumber)));
        }

        [Fact]
        public void UserRegResponse_CanBeCreated_WithNullValues()
        {
            // Arrange
            string? id = null;
            string? email = null;
            string? userName = null;
            string? displayName = null;
            string? phoneNumber = null;
            string? role = null;

            // Act
            var response = new UserRegResponse(id!, email!, userName!, displayName!, phoneNumber!, role!);

            // Assert
            Assert.Null(response.Id);
            Assert.Null(response.Email);
            Assert.Null(response.UserName);
            Assert.Null(response.DisplayName);
            Assert.Null(response.PhoneNumber);
            Assert.Null(response.Role);
        }

        [Fact]
        public void UserRegResponse_CanBeCreated_WithEmptyStrings()
        {
            // Arrange
            var id = string.Empty;
            var email = string.Empty;
            var userName = string.Empty;
            var displayName = string.Empty;
            var phoneNumber = string.Empty;
            var role = string.Empty;

            // Act
            var response = new UserRegResponse(id, email, userName, displayName, phoneNumber, role);

            // Assert
            Assert.Equal(string.Empty, response.Id);
            Assert.Equal(string.Empty, response.Email);
            Assert.Equal(string.Empty, response.UserName);
            Assert.Equal(string.Empty, response.DisplayName);
            Assert.Equal(string.Empty, response.PhoneNumber);
            Assert.Equal(string.Empty, response.Role);
        }

        [Fact]
        public void UserRegResponse_CanBeCreated_WithValidData()
        {
            // Arrange
            var id = "123";
            var email = "valid@example.com";
            var userName = "validUsername";
            var displayName = "Valid Display Name";
            var phoneNumber = "123-456-7890";
            var role = "User";

            // Act
            var response = new UserRegResponse(id, email, userName, displayName, phoneNumber, role);

            // Assert
            Assert.Equal(id, response.Id);
            Assert.Equal(email, response.Email);
            Assert.Equal(userName, response.UserName);
            Assert.Equal(displayName, response.DisplayName);
            Assert.Equal(phoneNumber, response.PhoneNumber);
            Assert.Equal(role, response.Role);
        }

        [Fact]
        public void UserRegResponse_Equality_Test()
        {
            // Arrange
            var id = "123";
            var email = "valid@example.com";
            var userName = "validUsername";
            var displayName = "Valid Display Name";
            var phoneNumber = "123-456-7890";
            var role = "User";

            // Act
            var response1 = new UserRegResponse(id, email, userName, displayName, phoneNumber, role);
            var response2 = new UserRegResponse(id, email, userName, displayName, phoneNumber, role);

            // Assert
            Assert.Equal(response1, response2);
            Assert.True(response1 == response2);
        }

        [Fact]
        public void UserRegResponse_Inequality_Test()
        {
            // Arrange
            var id = "123";
            var email = "valid@example.com";
            var userName = "validUsername";
            var displayName = "Valid Display Name";
            var phoneNumber = "123-456-7890";
            var role = "User";

            // Act
            var response1 = new UserRegResponse(id, email, userName, displayName, phoneNumber, role);
            var response2 = new UserRegResponse(id, email, userName, displayName, phoneNumber, "Admin");

            // Assert
            Assert.NotEqual(response1, response2);
            Assert.False(response1 == response2);
        }
    }
}
