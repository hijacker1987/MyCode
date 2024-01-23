using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication;
using MyCode_Backend_Server.Service.Authentication.Token;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Service.Auth
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task RegisterAsync_SuccessfulRegistration_ReturnsAuthResult()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);

            var tokenServiceMock = new Mock<ITokenService>();

            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                                                         .ReturnsAsync(IdentityResult.Success);

            var authService = new AuthService(userManagerMock.Object, tokenServiceMock.Object, new Mock<ILogger<AuthService>>().Object);

            // Act
            var result = await authService.RegisterAsync("test@example.com", "username", "password", "displayname", "123456789");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("test@example.com", result.Email);
            Assert.Empty(result.ErrorMessages);
        }

        [Fact]
        public async Task RegisterAsync_FailedRegistration_ReturnsAuthResultWithErrors()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);

            var tokenServiceMock = new Mock<ITokenService>();

            var errors = new List<IdentityError> { new() { Code = "ErrorCode", Description = "ErrorDescription" } };

            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                                                         .ReturnsAsync(IdentityResult.Failed([.. errors]));

            var authService = new AuthService(userManagerMock.Object, tokenServiceMock.Object, new Mock<ILogger<AuthService>>().Object);

            // Act
            var result = await authService.RegisterAsync("test@example.com", "username", "password", "displayname", "123456789");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("test@example.com", result.Email);
            Assert.Single(result.ErrorMessages);
            Assert.Contains("ErrorCode", result.ErrorMessages.Keys);
            Assert.Equal("ErrorDescription", result.ErrorMessages["ErrorCode"]);
        }

        [Fact]
        public async Task LoginAsync_InvalidEmail_ReturnsAuthResultWithErrorMessage()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);

            var authService = new AuthService(userManagerMock.Object, Mock.Of<ITokenService>(), new Mock<ILogger<AuthService>>().Object);

            // Act
            var result = await authService.LoginAsync("nonexistent@example.com", "password");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Bad credentials", result.ErrorMessages.First().Key);
            Assert.Equal("Invalid email", result.ErrorMessages.First().Value);
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ReturnsAuthResultWithErrorMessage()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);

            var authService = new AuthService(userManagerMock.Object, Mock.Of<ITokenService>(), new Mock<ILogger<AuthService>>().Object);

            // Act
            var result = await authService.LoginAsync("testexample.com", "password");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Bad credentials", result.ErrorMessages.First().Key);
            Assert.Equal("Invalid email", result.ErrorMessages.First().Value);
        }
    }
}
