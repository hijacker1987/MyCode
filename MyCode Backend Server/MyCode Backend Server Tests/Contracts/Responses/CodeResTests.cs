using MyCode_Backend_Server.Contracts.Registers;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Contracts.Responses
{
    public class CodeResTests
    {
        [Fact]
        public void CodeRegResponse_PropertiesMatch_ExpectedValues()
        {
            // Arrange
            var expectedId = "12345678-1234-5678-1234-567812345678";
            var expectedCodeTitle = "Test Title";
            var expectedMyCode = "console.log('Hello, World!');";
            var expectedWhatKindOfCode = "Test";
            var expectedIsBackend = true;
            var expectedIsVisible = true;

            // Act
            var codeRegResponse = new CodeRegResponse(
                expectedId,
                expectedCodeTitle,
                expectedMyCode,
                expectedWhatKindOfCode,
                expectedIsBackend,
                expectedIsVisible
            );

            // Assert
            Assert.Equal(expectedId, codeRegResponse.Id);
            Assert.Equal(expectedCodeTitle, codeRegResponse.CodeTitle);
            Assert.Equal(expectedMyCode, codeRegResponse.MyCode);
            Assert.Equal(expectedWhatKindOfCode, codeRegResponse.WhatKindOfCode);
            Assert.Equal(expectedIsBackend, codeRegResponse.IsBackend);
            Assert.Equal(expectedIsVisible, codeRegResponse.IsVisible);
        }

        [Fact]
        public void CodeRegResponse_ShouldHaveCorrectProperties()
        {
            // Arrange
            var response = new CodeRegResponse(
                "Sample Id",
                "Sample Title",
                "Sample Code",
                "Sample Kind",
                true,
                true
            );

            // Act & Assert
            Assert.Equal("Sample Id", response.Id);
            Assert.Equal("Sample Title", response.CodeTitle);
            Assert.Equal("Sample Code", response.MyCode);
            Assert.Equal("Sample Kind", response.WhatKindOfCode);
            Assert.True(response.IsBackend);
            Assert.True(response.IsVisible);
        }
    }
}
