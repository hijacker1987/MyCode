using System.ComponentModel.DataAnnotations;
using Xunit;
using MyCode_Backend_Server.Contracts.Registers;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Models
{
    public class CodeRegs
    {
        private static IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void CodeRegRequest_ValidModel_DoesNotReturnValidationErrors()
        {
            // Arrange
            var model = new CodeRegRequest("Valid Title", "console.log('Hello, World!');", "JavaScript", true, true);

            // Act
            var results = ValidateModel(model);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void CodeRegRequest_MissingRequiredFields_ReturnsValidationErrors()
        {
            // Arrange
            var model = new CodeRegRequest("", "", "", false, true);

            // Act
            var results = ValidateModel(model);

            // Assert
            Assert.Equal(3, results.Count);
            Assert.Contains(results, v => v.MemberNames.Contains("CodeTitle"));
            Assert.Contains(results, v => v.MemberNames.Contains("MyCode"));
            Assert.Contains(results, v => v.MemberNames.Contains("WhatKindOfCode"));
        }

        [Fact]
        public void CodeRegRequest_CodeTitleTooLong_ReturnsValidationError()
        {
            // Arrange
            var longTitle = new string('a', 256);
            var model = new CodeRegRequest(longTitle, "console.log('Hello, World!');", "JavaScript", true, true);

            // Act
            var results = ValidateModel(model);

            // Assert
            Assert.Single(results);
            Assert.Contains(results, v => v.MemberNames.Contains("CodeTitle"));
        }

        [Fact]
        public void CodeRegRequest_WhatKindOfCodeTooLong_ReturnsValidationError()
        {
            // Arrange
            var longKindOfCode = new string('a', 51);
            var model = new CodeRegRequest("Valid Title", "console.log('Hello, World!');", longKindOfCode, true, true);

            // Act
            var results = ValidateModel(model);

            // Assert
            Assert.Single(results);
            Assert.Contains(results, v => v.MemberNames.Contains("WhatKindOfCode"));
        }

        [Fact]
        public void CodeRegResponse_CanBeCreated_WithValidData()
        {
            // Arrange
            var id = "123";
            var codeTitle = "Valid Title";
            var myCode = "console.log('Hello, World!');";
            var whatKindOfCode = "JavaScript";
            var isBackend = true;
            var isVisible = true;

            // Act
            var response = new CodeRegResponse(id, codeTitle, myCode, whatKindOfCode, isBackend, isVisible);

            // Assert
            Assert.Equal(id, response.Id);
            Assert.Equal(codeTitle, response.CodeTitle);
            Assert.Equal(myCode, response.MyCode);
            Assert.Equal(whatKindOfCode, response.WhatKindOfCode);
            Assert.Equal(isBackend, response.IsBackend);
            Assert.Equal(isVisible, response.IsVisible);
        }

        [Fact]
        public void CodeRegResponse_EqualityComparison_WorksCorrectly()
        {
            // Arrange
            var response1 = new CodeRegResponse("123", "Valid Title", "console.log('Hello, World!');", "JavaScript", true, true);
            var response2 = new CodeRegResponse("123", "Valid Title", "console.log('Hello, World!');", "JavaScript", true, true);

            // Act & Assert
            Assert.Equal(response1, response2);
            Assert.True(response1 == response2);
            Assert.False(response1 != response2);
        }

        [Fact]
        public void CodeRegResponse_DifferentInstances_NotEqual()
        {
            // Arrange
            var response1 = new CodeRegResponse("123", "Valid Title", "console.log('Hello, World!');", "JavaScript", true, true);
            var response2 = new CodeRegResponse("124", "Another Title", "print('Hello, World!')", "Python", false, false);

            // Act & Assert
            Assert.NotEqual(response1, response2);
            Assert.False(response1 == response2);
            Assert.True(response1 != response2);
        }
    }
}
