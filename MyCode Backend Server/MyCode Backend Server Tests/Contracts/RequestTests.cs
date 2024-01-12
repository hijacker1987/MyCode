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
            var codeRegRequest = new
            {
                CodeTitle = "Test Title",
                MyCode = "console.log('Hello, World!');",
                WhatKindOfCode = "Test",
                IsBackend = true,
                IsVisible = true
            };

            // Act
            var validationContext = new ValidationContext(codeRegRequest, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(codeRegRequest, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void UserRegRequest_Validation_Success()
        {
            // Arrange
            var userRegRequest = new
            {
                Email = "Test@Title.Mail",
                Username = "Hello World!",
                Password = "Test",
                DisplayName = "Test User",
                PhoneNumber = "123"
            };

            // Act
            var validationContext = new ValidationContext(userRegRequest, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(userRegRequest, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void AuthRegRequest_Validation_Success()
        {
            // Arrange
            var authRegRequest = new
            {
                Email = "Test@Title.Mail",
                Password = "Test"
            };

            // Act
            var validationContext = new ValidationContext(authRegRequest, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(authRegRequest, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ChangePassRequest_Validation_Success()
        {
            // Arrange
            var changePassRegRequest = new
            {
                Email = "Test@Title.Mail",
                Password = "Test",
                NewPassword = "Tester"
            };

            // Act
            var validationContext = new ValidationContext(changePassRegRequest, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(changePassRegRequest, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
        }
    }
}
