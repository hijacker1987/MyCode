using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MyCode_Backend_Server.Contracts.Services;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication;
using MyCode_Backend_Server.Service.Authentication.Token;
using MyCode_Backend_Server.Service.Email_Sender;
using MyCode_Backend_Server_Tests.Services;
using System.Security.Claims;
using Xunit;
using static System.Net.WebRequestMethods;
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
                new Mock<ILogger<UserManager<User>>>().Object,
                new Mock<IEmailSender>().Object);

            var tokenServiceMock = new Mock<ITokenService>();
            var dataContextMock = new Mock<DataContext>();

            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                                                         .ReturnsAsync(IdentityResult.Success);

            var authService = new AuthService(Mock.Of<IConfiguration>(), userManagerMock.Object, tokenServiceMock.Object, dataContextMock.Object, (IEmailSender)userManagerMock.Object, new Mock<ILogger<AuthService>>().Object);

            // Act
            var result = await authService.RegisterAccAsync("test@example.com", "username", "password", "displayname", "123456789");

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

            var dataContextMock = new Mock<DataContext>();

            var errors = new List<IdentityError> { new() { Code = "ErrorCode", Description = "ErrorDescription" } };

            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed([.. errors]));

            var authService = new AuthService(Mock.Of<IConfiguration>(), userManagerMock.Object, tokenServiceMock.Object, dataContextMock.Object, (IEmailSender)userManagerMock.Object, new Mock<ILogger<AuthService>>().Object);

            // Act
            var result = await authService.RegisterAccAsync("test@example.com", "username", "password", "displayname", "123456789");

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

            var dataContextMock = new Mock<DataContext>();

            var authService = new AuthService(Mock.Of<IConfiguration>(), userManagerMock.Object, Mock.Of<ITokenService>(), dataContextMock.Object, (IEmailSender)userManagerMock.Object, new Mock<ILogger<AuthService>>().Object);

            var httpContext = new DefaultHttpContext();
            var httpRequest = httpContext.Request;
            var httpResponse = httpContext.Response;

            // Act
            var result = await authService.LoginAccAsync("nonexistent@example.com", "password", "confirmPassword", httpRequest, httpResponse);

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

            var dataContextMock = new Mock<DataContext>();

            var authService = new AuthService(Mock.Of<IConfiguration>(), userManagerMock.Object, Mock.Of<ITokenService>(), dataContextMock.Object, (IEmailSender)userManagerMock.Object, new Mock<ILogger<AuthService>>().Object);

            var httpContext = new DefaultHttpContext();
            var httpRequest = httpContext.Request;
            var httpResponse = httpContext.Response;

            // Act
            var result = await authService.LoginAccAsync("testexample.com", "password", "confirmPassword", httpRequest, httpResponse);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Bad credentials", result.ErrorMessages.First().Key);
            Assert.Equal("Invalid email", result.ErrorMessages.First().Value);
        }

        [Fact]
        public async Task RegisterAsync_UserAlreadyExists_ReturnsAuthResultWithErrorMessage()
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

            var dataContextMock = new Mock<DataContext>();

            var errors = new List<IdentityError> { new() { Code = "DuplicateUserName", Description = "Username 'test' is already taken." } };

            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed([.. errors]));

            var authService = new AuthService(Mock.Of<IConfiguration>(), userManagerMock.Object, tokenServiceMock.Object, dataContextMock.Object, (IEmailSender)userManagerMock.Object, new Mock<ILogger<AuthService>>().Object);

            // Act
            var result = await authService.RegisterAccAsync("test@example.com", "test", "password", "displayname", "123456789");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("test@example.com", result.Email);
            Assert.Single(result.ErrorMessages);
            Assert.Contains("DuplicateUserName", result.ErrorMessages.Keys);
            Assert.Equal("Username 'test' is already taken.", result.ErrorMessages["DuplicateUserName"]);
        }

        [Fact]
        public async Task LoginAsync_AlreadyAuthenticatedUser_ReturnsAuthResultWithErrorMessage()
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
                new Mock<ILogger<UserManager<User>>>().Object,
                new Mock<IEmailSender>().Object);

            var dataContextMock = new Mock<DataContext>();

            var authService = new AuthService(Mock.Of<IConfiguration>(), userManagerMock.Object, Mock.Of<ITokenService>(), dataContextMock.Object, (IEmailSender)userManagerMock.Object, new Mock<ILogger<AuthService>>().Object);

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new(ClaimTypes.Name, "Tester5") }))
            };

            var httpRequest = httpContext.Request;
            var httpResponse = httpContext.Response;

            // Act
            var result = await authService.LoginAccAsync("tester5@test.com", "Password", "Password", httpRequest, httpResponse);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Bad credentials", result.ErrorMessages.First().Key);
            Assert.Contains("Invalid email", result.ErrorMessages.First().Value);
        }
    }
}