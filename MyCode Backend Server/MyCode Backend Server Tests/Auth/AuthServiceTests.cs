using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication;
using MyCode_Backend_Server.Service.Authentication.Token;
using MyCode_Backend_Server.Service.Email_Sender;
using System.Security.Claims;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Service.Auth
{
    public class AuthServiceTests
    {
        private readonly IConfiguration _configuration;

        public AuthServiceTests()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            _configuration = configBuilder.Build();
        }

        private DataContext CreateLiveDataContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(_configuration.GetConnectionString("TestConnectionString"))
                .Options;
            return new DataContext(options);
        }

        private static Mock<UserManager<User>> CreateMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            var optionsMock = new Mock<IOptions<IdentityOptions>>();
            var passwordHasherMock = new Mock<IPasswordHasher<User>>();
            var userValidatorsMock = new List<IUserValidator<User>>().ToArray();
            var passwordValidatorsMock = new List<IPasswordValidator<User>>().ToArray();
            var lookupNormalizerMock = new Mock<ILookupNormalizer>();
            var identityErrorDescriberMock = new Mock<IdentityErrorDescriber>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            var loggerMock = new Mock<ILogger<UserManager<User>>>();

            return new Mock<UserManager<User>>(
                userStoreMock.Object,
                optionsMock.Object,
                passwordHasherMock.Object,
                userValidatorsMock,
                passwordValidatorsMock,
                lookupNormalizerMock.Object,
                identityErrorDescriberMock.Object,
                serviceProviderMock.Object,
                loggerMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_SuccessfulRegistration_ReturnsAuthResult()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();
            var tokenServiceMock = new Mock<ITokenService>();
            var dataContextMock = CreateLiveDataContext();

            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var authService = new AuthService(
                Mock.Of<IConfiguration>(),
                userManagerMock.Object,
                tokenServiceMock.Object,
                dataContextMock,
                Mock.Of<IEmailSender>(),
                new Mock<ILogger<AuthService>>().Object);

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
            var userManagerMock = CreateMockUserManager();

            var tokenServiceMock = new Mock<ITokenService>();
            var dataContextMock = CreateLiveDataContext();

            var errors = new List<IdentityError>
            {
                new IdentityError { Code = "ErrorCode", Description = "ErrorDescription" }
            };

            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed([.. errors]));

            var authService = new AuthService(
                Mock.Of<IConfiguration>(),
                userManagerMock.Object,
                tokenServiceMock.Object,
                dataContextMock,
                Mock.Of<IEmailSender>(),
                new Mock<ILogger<AuthService>>().Object);

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
            var userManagerMock = CreateMockUserManager();
            var dataContextMock = CreateLiveDataContext();

            var authService = new AuthService(
                Mock.Of<IConfiguration>(),
                userManagerMock.Object,
                Mock.Of<ITokenService>(),
                dataContextMock,
                Mock.Of<IEmailSender>(),
                new Mock<ILogger<AuthService>>().Object);

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
            var userManagerMock = CreateMockUserManager();
            var dataContextMock = CreateLiveDataContext();

            var authService = new AuthService(
                Mock.Of<IConfiguration>(),
                userManagerMock.Object,
                Mock.Of<ITokenService>(),
                dataContextMock,
                Mock.Of<IEmailSender>(),
                new Mock<ILogger<AuthService>>().Object);

            var httpContext = new DefaultHttpContext();
            var httpRequest = httpContext.Request;
            var httpResponse = httpContext.Response;

            // Act
            var result = await authService.LoginAccAsync("tester10@test.com", "password", "confirmPassword", httpRequest, httpResponse);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Bad credentials", result.ErrorMessages.Keys);
        }

        [Fact]
        public async Task RegisterAsync_UserAlreadyExists_ReturnsAuthResultWithErrorMessage()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();

            var tokenServiceMock = new Mock<ITokenService>();
            var dataContextMock = CreateLiveDataContext();

            var errors = new List<IdentityError>
            {
                new IdentityError { Code = "DuplicateUserName", Description = "Username 'test' is already taken." }
            };

            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors.ToArray()));

            var authService = new AuthService(
                Mock.Of<IConfiguration>(),
                userManagerMock.Object,
                tokenServiceMock.Object,
                dataContextMock,
                Mock.Of<IEmailSender>(),
                new Mock<ILogger<AuthService>>().Object);

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
            var userManagerMock = CreateMockUserManager();
            var dataContextMock = CreateLiveDataContext();

            var authService = new AuthService(
                Mock.Of<IConfiguration>(),
                userManagerMock.Object,
                Mock.Of<ITokenService>(),
                dataContextMock,
                Mock.Of<IEmailSender>(),
                new Mock<ILogger<AuthService>>().Object);

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "Tester5") }))
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
