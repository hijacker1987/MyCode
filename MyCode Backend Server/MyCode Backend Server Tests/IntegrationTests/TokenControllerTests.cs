using Assert = Xunit.Assert;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using MyCode_Backend_Server.Controllers;
using MyCode_Backend_Server.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MyCode_Backend_Server_Tests.IntegrationTests
{
    public class TokenControllerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ILogger<TokenController>> _loggerMock;
        private readonly TokenController _controller;

        public TokenControllerTests()
        {
            _userManagerMock = MockUserManager<User>();
            _loggerMock = new Mock<ILogger<TokenController>>();
            _controller = new TokenController(_userManagerMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Revoke_AuthenticatedUser_TokenRevoked()
        {
            // Arrange
            var id = Guid.NewGuid();
            var user = new User { UserName = "testuser", Id = id };
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.NameIdentifier, id.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _controller.Revoke();

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task Revoke_UserNotAuthenticated_ReturnsUnauthorized()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await _controller.Revoke();

            // Assert
            var _ = Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Revoke_UserNotFound_ReturnsUnauthorized()
        {
            // Arrange
            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User)null!);

            var claims = new[] { new Claim(ClaimTypes.Name, "testuser") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _controller.Revoke();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result);
        }

        private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            return mgr;
        }
    }
}
