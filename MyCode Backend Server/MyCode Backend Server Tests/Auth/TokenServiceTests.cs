using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication.Token;
using System.Data;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.Auth
{
    public class TokenServiceTests
    {
        [Fact]
        public void CreateToken_ValidUser_ReturnsAccessToken()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.SetupGet(c => c["IssueAudience"]).Returns("V3ryStr0ngP@ssw0rdW1thM0reTh@n256B1ts*");
            configurationMock.SetupGet(c => c["IssueSign"]).Returns("V3ryStr0ngP@ssw0rdW1thM0reTh@n256B1ts");

            var loggerMock = new Mock<ILogger<TokenService>>();

            var tokenService = new TokenService(configurationMock.Object, loggerMock.Object);

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "test@example.com"
            };

            var roles = new List<string> { "UserRole" };

            // Act
            var accessToken = tokenService.CreateToken(user, roles);

            // Assert
            Assert.NotNull(accessToken);
            Assert.NotEmpty(accessToken);
        }

        [Fact]
        public void CreateToken_InvalidUser_ThrowsException()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger<TokenService>>();

            var tokenService = new TokenService(configurationMock.Object, loggerMock.Object);

            User user = null!;

            var roles = new List<string> { "UserRole" };

            // Act and Assert
            Assert.Throws<NullReferenceException>(() => tokenService.CreateToken(user, roles));
        }
    }

}
