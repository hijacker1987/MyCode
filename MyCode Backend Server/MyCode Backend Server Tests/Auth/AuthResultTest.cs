using MyCode_Backend_Server.Service.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Net;
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
                Email: "Test@example.com",
                UserName: "TestUser",
                DisplayName: "Test User",
                PhoneNumber: "1234567890",
                Token: "TestToken"
            );

            // Act
            var isValid = IsValid(codeRegRequest);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void AuthResult_Handle_Unsuccessful_Result()
        {
            // Arrange
            var authResult = new AuthResult
            (
                Id: null,
                Success: false,
                Email: null!,
                UserName: null,
                DisplayName: null,
                PhoneNumber: null,
                Token: null
            );

            // Assert
            Assert.False(authResult.Success);
            Assert.Null(authResult.Id);
            Assert.Null(authResult.Email);
            Assert.Null(authResult.UserName);
            Assert.Null(authResult.DisplayName);
            Assert.Null(authResult.PhoneNumber);
            Assert.Null(authResult.Token);
        }

        [Fact]
        public void AuthResult_Immutable_After_Creation()
        {
            // Arrange
            var authResult = new AuthResult
            (
                Id: "123",
                Success: true,
                Email: "Test@example.com",
                UserName: "TestUser",
                DisplayName: "Test User",
                PhoneNumber: "1234567890",
                Token: "TestToken"
            );

            // Act
            var idBefore = authResult.Id;
            authResult = authResult with { Id = "456" };
            var idAfter = authResult.Id;

            // Assert
            Assert.NotEqual(idBefore, idAfter);
        }

        [Fact]
        public void AuthResult_Validation_Success_When_All_Optional_Fields_Null_And_Success_True()
        {
            // Arrange
            var authResult = new AuthResult
            (
                Id: "123",
                Success: true,
                Email: null,
                UserName: null,
                DisplayName: null,
                PhoneNumber: null,
                Token: null
            );

            // Act
            var isValid = IsValid(authResult);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void AuthResult_ErrorMessages_Test()
        {
            // Arrange
            var authResult = new AuthResult(
                Id: "123",
                Success: true,
                Email: null,
                UserName: null,
                DisplayName: null,
                PhoneNumber: null,
                Token: null
            );

            // Act

            // Assert
            Assert.NotNull(authResult.ErrorMessages);
            Assert.Empty(authResult.ErrorMessages);
        }

        [Fact]
        public void AuthResult_StatusCode_Test()
        {
            // Arrange
            var authResult = new AuthResult(
                Id: "123",
                Success: true,
                Email: "tester@statuscode.com",
                UserName: "statusOk",
                DisplayName: "Success",
                PhoneNumber: "0660",
                Token: "aFg"
            );

            // Act

            // Assert
            Assert.Equal(HttpStatusCode.OK, authResult.StatusCode);
        }

        private static bool IsValid(object instance, string propertyName = null!)
        {
            var validationContext = new ValidationContext(instance, null, null);
            var validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(instance, validationContext, validationResults, true);
        }
    }
}
