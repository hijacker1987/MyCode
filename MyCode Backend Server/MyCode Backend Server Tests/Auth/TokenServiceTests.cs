using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication.Token;
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

            var tokenService = new TokenService(Mock.Of<IConfiguration>(), userManagerMock.Object, dataContextMock.Object, loggerMock.Object);

            User user = null!;

            var roles = new List<string> { "UserRole" };

            // Act and Assert
            Assert.Throws<NullReferenceException>(() => tokenService.CreateToken(user, roles));
        }
    }
}
