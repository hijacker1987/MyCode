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

        [Fact]
        public void CodeRegRequest_Validation_Fail_When_CodeTitle_Exceeds_MaxLength()
        {
            // Arrange
            var codeRegRequest = new CodeRegRequest(
                new string('A', CodeRegRequest.MaxCodeTitleLength + 1),
                "console.log('Hello, World!');",
                "Test",
                true,
                true
            );

            // Act
            var isValid = IsValid(codeRegRequest, nameof(CodeRegRequest.CodeTitle));

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void CodeRegRequest_Validation_Fail_When_WhatKindOfCode_Contains_SpecialCharacters()
        {
            // Arrange
            var codeRegRequest = new CodeRegRequest(
                "Test Title",
                "console.log('Hello, World!');",
                "!@#$%^&*", 
                true,
                true
            );

            // Act
            var isValid = IsValid(codeRegRequest, nameof(CodeRegRequest.WhatKindOfCode));

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void CodeRegRequest_Validation_Fail_When_Multiple_Properties_Invalid()
        {
            // Arrange
            var codeRegRequest = new CodeRegRequest(
                null!,
                string.Empty,
                null!,
                false,
                true
            );

            // Act
            var isValid = IsValid(codeRegRequest);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void CodeRegRequest_Validation_Fail_When_MyCode_Is_Null()
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
        public void CodeRegRequest_Validation_Fail_When_WhatKindOfCode_Is_Null()
        {
            // Arrange
            var codeRegRequest = new CodeRegRequest
            (
                "Test Title",
                "console.log('Hello, World!');",
                null!,
                true,
                true
            );

            // Act
            var isValid = IsValid(codeRegRequest);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void CodeRegRequest_Validation_Fail_When_WhatKindOfCode_Is_Too_Long()
        {
            // Arrange
            var codeRegRequest = new CodeRegRequest
            (
                "Test Title",
                "console.log('Hello, World!');",
                new string('A', CodeRegRequest.MaxKindOfCodeLength + 1),
                true,
                true
            );

            // Act
            var isValid = IsValid(codeRegRequest);

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
