using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Contracts.Services;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Contracts.ResponseTests
{
    public class ResponseTests
    {
        [Fact]
        public void CodeRegResponse_PropertiesMatch_ExpectedValues()
        {
            // Arrange
            var expectedId = new Guid("12345678-1234-5678-1234-567812345678");
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
        public void UserRegResponse_PropertiesMatch_ExpectedValues()
        {
            // Arrange
            var expectedEmail = "testuser@example.com";
            var expectedUserName = "testuser";

            // Act
            var userRegResponse = new UserRegResponse(expectedEmail, expectedUserName);

            // Assert
            Assert.Equal(expectedEmail, userRegResponse.Email);
            Assert.Equal(expectedUserName, userRegResponse.UserName);
        }

        [Fact]
        public void AuthResponse_PropertiesMatch_ExpectedValues()
        {
            // Arrange
            var expectedEmail = "testuser@example.com";
            var expectedUsername = "testuser";
            var expectedToken = "myAccessToken";
            var expectedRole = "UserRole";

            // Act
            var authResponse = new AuthResponse(expectedEmail, expectedUsername, expectedToken, expectedRole);

            // Assert
            Assert.Equal(expectedEmail, authResponse.Email);
            Assert.Equal(expectedUsername, authResponse.Username);
            Assert.Equal(expectedToken, authResponse.Token);
            Assert.Equal(expectedRole, authResponse.Role);
        }

        [Fact]
        public void ChangePassResponse_PropertiesMatch_ExpectedValues()
        {
            // Arrange
            var expectedEmail = "testuser@example.com";

            // Act
            var changePassResponse = new ChangePassResponse(expectedEmail);

            // Assert
            Assert.Equal(expectedEmail, changePassResponse.Email);
        }
    }
}

