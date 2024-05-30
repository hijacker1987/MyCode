using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication.Token;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Auth
{
    public class TokenServiceTests
    {
        [Fact]
        public void CreateToken_InvalidUser_ThrowsException()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger<TokenService>>();
            var options = new DbContextOptionsBuilder<DataContext>()
                                .Options;

            var dataContextMock = new Mock<DataContext>(options);
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<User>>(),
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<User>>>()
            );

            var tokenService = new TokenService(Mock.Of<IConfiguration>(), userManagerMock.Object, dataContextMock.Object, Mock.Of<IWebHostEnvironment>(), loggerMock.Object);

            User user = null!;

            var roles = new List<string> { "User" };

            // Act and Assert
            Assert.Throws<NullReferenceException>(() => tokenService.CreateToken(user, roles));
        }

        [Fact]
        public void CreateToken_ValidUser_ReturnsToken()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["IssueSign"]).Returns("V3ryStr0ngP@ssw0rdW1thM0reTh@n256B1tsF0rT3st1ng");
            configurationMock.Setup(c => c["AccessTokenExp"]).Returns("15");

            var loggerMock = new Mock<ILogger<TokenService>>();
            var options = new DbContextOptionsBuilder<DataContext>().Options;

            var dataContextMock = new Mock<DataContext>(options);
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<User>>(),
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<User>>>()
            );

            var environmentMock = new Mock<IWebHostEnvironment>();
            environmentMock.Setup(e => e.EnvironmentName).Returns("Test");

            var tokenService = new TokenService(configurationMock.Object, userManagerMock.Object, dataContextMock.Object, environmentMock.Object, loggerMock.Object);

            var user = new User { Id = Guid.NewGuid(), UserName = "TestUser", Email = "test@test.com" };
            var roles = new List<string> { "User" };

            // Act
            var token = tokenService.CreateToken(user, roles);

            // Assert
            Assert.NotNull(token);
        }

        [Fact]
        public void CreateToken_MissingIssueSignKey_ThrowsException()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger<TokenService>>();
            var options = new DbContextOptionsBuilder<DataContext>().Options;

            var dataContextMock = new Mock<DataContext>(options);
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<User>>(),
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<User>>>()
            );

            var environmentMock = new Mock<IWebHostEnvironment>();
            environmentMock.Setup(e => e.EnvironmentName).Returns("Test");

            var tokenService = new TokenService(configurationMock.Object, userManagerMock.Object, dataContextMock.Object, environmentMock.Object, loggerMock.Object);

            var user = new User { Id = Guid.NewGuid(), UserName = "TestUser", Email = "test@test.com" };
            var roles = new List<string> { "User" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => tokenService.CreateToken(user, roles));
        }

        [Fact]
        public void CreateRefreshToken_ReturnsToken()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger<TokenService>>();
            var options = new DbContextOptionsBuilder<DataContext>().Options;

            var dataContextMock = new Mock<DataContext>(options);
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<User>>(),
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<User>>>()
            );

            var tokenService = new TokenService(configurationMock.Object, userManagerMock.Object, dataContextMock.Object, Mock.Of<IWebHostEnvironment>(), loggerMock.Object);

            // Act
            var refreshToken = tokenService.CreateRefreshToken();

            // Assert
            Assert.NotNull(refreshToken);
            Assert.Equal(88, refreshToken.Length); //String length for 64 bytes
        }

        [Fact]
        public void ValidateToken_TokenExpired_ReturnsFalse()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["IssueSign"]).Returns("V3ryStr0ngP@ssw0rdW1thM0reTh@n256B1tsF0rT3st1ng");
            var loggerMock = new Mock<ILogger<TokenService>>();
            var options = new DbContextOptionsBuilder<DataContext>().Options;

            var dataContextMock = new Mock<DataContext>(options);
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<User>>(),
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<User>>>()
            );

            var environmentMock = new Mock<IWebHostEnvironment>();
            environmentMock.Setup(e => e.EnvironmentName).Returns("Test");

            var tokenService = new TokenService(configurationMock.Object, userManagerMock.Object, dataContextMock.Object, environmentMock.Object, loggerMock.Object);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("V3ryStr0ngP@ssw0rdW1thM0reTh@n256B1tsF0rT3st1ng"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expiredToken = new JwtSecurityToken(
                issuer: "api With Test Authentication comes and goes here",
                audience: "api With Test Authentication comes and goes here",
                claims: [new Claim(ClaimTypes.Name, "TestUser")],
                expires: DateTime.UtcNow.AddDays(+1),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(expiredToken);

            // Act
            var isValid = tokenService.ValidateToken(tokenString);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Refresh_ValidData_ReturnsNewToken()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["IssueSign"]).Returns("V3ryStr0ngP@ssw0rdW1thM0reTh@n256B1tsF0rT3st1ng");
            configurationMock.Setup(c => c["AccessTokenExp"]).Returns("15");

            var loggerMock = new Mock<ILogger<TokenService>>();
            var options = new DbContextOptionsBuilder<DataContext>().Options;

            var dataContextMock = new Mock<DataContext>(options);
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<User>>(),
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<User>>>()
            );

            var environmentMock = new Mock<IWebHostEnvironment>();
            environmentMock.Setup(e => e.EnvironmentName).Returns("Test");

            var httpContextMock = new Mock<HttpContext>();
            var requestMock = new Mock<HttpRequest>();
            var responseMock = new Mock<HttpResponse>();

            var cookiesMock = new Mock<IRequestCookieCollection>();
            var cookiesResponseMock = new Mock<IResponseCookies>();

            requestMock.Setup(r => r.Cookies).Returns(cookiesMock.Object);
            responseMock.Setup(r => r.Cookies).Returns(cookiesResponseMock.Object);
            httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);
            httpContextMock.Setup(c => c.Response).Returns(responseMock.Object);

            var tokenService = new TokenService(configurationMock.Object, userManagerMock.Object, dataContextMock.Object, environmentMock.Object, loggerMock.Object);

            var user = new User { Id = Guid.NewGuid(), UserName = "TestUser", Email = "test@test.com", RefreshToken = "validRefreshToken", RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(5) };

            userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(["User"]);

            cookiesMock.Setup(c => c["RefreshAuthorization"]).Returns("validRefreshToken");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("V3ryStr0ngP@ssw0rdW1thM0reTh@n256B1tsF0rT3st1ng"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expiredToken = new JwtSecurityToken(
                issuer: "api With Test Authentication comes and goes here",
                audience: "api With Test Authentication comes and goes here",
                claims: [new Claim(ClaimTypes.Name, user.UserName)],
                expires: DateTime.UtcNow.AddDays(-1),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(expiredToken);

            // Act
            var newToken = tokenService.Refresh(tokenString, requestMock.Object, responseMock.Object);

            // Assert
            Assert.NotNull(newToken);
        }
    }
}
