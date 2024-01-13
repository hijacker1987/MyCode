using MyCode_Backend_Server.Contracts.Registers;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Contracts.Requests
{
    public class CodeReqTests
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
        public void CodeRegRequest_Validation_Fail_When_WhatKindOfCode_Is_Null_Or_Empty()
        {
            // Arrange
            var codeRegRequest1 = new CodeRegRequest
            (
                "Test Title",
                "console.log('Hello, World!');",
                null!,
                true,
                true
            );

            var codeRegRequest2 = new CodeRegRequest
            (
                "Test Title",
                "console.log('Hello, World!');",
                string.Empty,
                true,
                true
            );

            // Act & Assert
            Assert.False(IsValid(codeRegRequest1, nameof(CodeRegRequest.WhatKindOfCode)));
            Assert.False(IsValid(codeRegRequest2, nameof(CodeRegRequest.WhatKindOfCode)));
        }

        private static bool IsValid(object instance, string propertyName = null!)
        {
            var validationContext = new ValidationContext(instance, null, null);
            var validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(instance, validationContext, validationResults, true);
        }
    }
}
