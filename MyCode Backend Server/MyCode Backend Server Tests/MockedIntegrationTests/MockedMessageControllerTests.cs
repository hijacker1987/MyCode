using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MyCode_Backend_Server.Controllers;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Hubs;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication;
using MyCode_Backend_Server.Service.Authentication.Token;
using MyCode_Backend_Server.Service.Chat;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.MockedIntegrationTests
{
    public class MockedMessageControllerTests
    {
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IChatService> _mockChatService;
        private readonly Mock<ILogger<MessageController>> _mockLogger;
        private readonly Mock<DbSet<SupportChat>> _mockSupportDbSet;
        private readonly DataContext _dataContext;

        public MockedMessageControllerTests()
        {
            _mockTokenService = new Mock<ITokenService>();
            _mockAuthService = new Mock<IAuthService>();
            _mockChatService = new Mock<IChatService>();
            _mockLogger = new Mock<ILogger<MessageController>>();
            _mockSupportDbSet = new Mock<DbSet<SupportChat>>();

            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dataContext = new DataContext(dbContextOptions)
            {
                SupportDb = _mockSupportDbSet.Object
            };
        }

        private MessageController CreateController()
        {
            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                ], "mock"))
            };

            var controllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var controller = new MessageController(
                new Mock<IHubContext<MessageHub>>().Object,
                _dataContext,
                _mockChatService.Object,
                _mockTokenService.Object,
                _mockAuthService.Object,
                _mockLogger.Object
            )
            {
                ControllerContext = controllerContext
            };

            return controller;
        }

        [Fact]
        public void GetRoom_Returns_BadRequest_When_UserId_Not_Found()
        {
            // Arrange
            var controller = CreateController();
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // Act
            var result = controller.GetRoom();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal("No 'NameIdentifier' claim found.", badRequestResult?.Value);
        }

        [Fact]
        public void GetRoom_Returns_StatusCode500_On_Exception()
        {
            // Arrange
            _mockSupportDbSet.Setup(db => db.Find(It.IsAny<object[]>())).Throws(new Exception("Simulated Exception"));
            var controller = CreateController();

            // Act
            var result = controller.GetRoom();

            // Assert
            Assert.IsType<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.Equal(500, objectResult?.StatusCode);
        }

        [Fact]
        public async Task GetAnyArchived_Returns_StatusCode500_On_Exception()
        {
            // Arrange
            _mockSupportDbSet.Setup(db => db.Find(It.IsAny<object[]>())).Throws(new Exception("Simulated Exception"));
            var controller = CreateController();

            // Act
            var result = await controller.GetAnyArchived();

            // Assert
            Assert.IsType<ObjectResult>(result.Result);
            var objectResult = result.Result as ObjectResult;
            Assert.Equal(500, objectResult?.StatusCode);
        }

        [Fact]
        public async Task GetOwnArchived_Returns_StatusCode500_On_Exception()
        {
            // Arrange
            _mockSupportDbSet.Setup(db => db.Find(It.IsAny<object[]>())).Throws(new Exception("Simulated Exception"));
            var controller = CreateController();

            // Act
            var result = await controller.GetOwnArchived();

            // Assert
            Assert.IsType<ObjectResult>(result.Result);
            var objectResult = result.Result as ObjectResult;
            Assert.Equal(500, objectResult?.StatusCode);
        }

        [Fact]
        public void GetActiveRoom_Returns_StatusCode500_On_Exception()
        {
            // Arrange
            _mockSupportDbSet.Setup(db => db.Find(It.IsAny<object[]>())).Throws(new Exception("Simulated Exception"));
            var controller = CreateController();

            // Act
            var result = controller.GetActiveRoom();

            // Assert
            Assert.IsType<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.Equal(500, objectResult?.StatusCode);
        }

        [Fact]
        public async Task GetArchivedById_Returns_StatusCode404_On_Exception()
        {
            // Arrange
            _mockSupportDbSet.Setup(db => db.Find(It.IsAny<object[]>())).Throws(new Exception("Simulated Exception"));
            var controller = CreateController();

            // Act
            var result = await controller.GetArchivedById(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
            var objectResult = result.Result as NotFoundResult;
            Assert.Equal(404, objectResult?.StatusCode);
        }

        [Fact]
        public async Task GetActiveById_Returns_StatusCode404_On_Exception()
        {
            // Arrange
            _mockChatService.Setup(service => service.GetStoredActiveMessagesByRoom(It.IsAny<string>())).Throws(new Exception("Simulated Exception"));
            var controller = CreateController();

            // Act
            var result = await controller.GetActiveById(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
            var objectResult = result.Result as NotFoundResult;
            Assert.Equal(404, objectResult?.StatusCode);
        }

        [Fact]
        public async Task DropBackActiveToSupportById_Returns_StatusCode404_On_Exception()
        {
            // Arrange
            _mockChatService.Setup(service => service.GetStoredActiveMessagesByRoom(It.IsAny<string>())).Throws(new Exception("Simulated Exception"));
            var controller = CreateController();

            // Act
            var result = await controller.DropBackActiveToSupportById(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result);
            var objectResult = result as NotFoundResult;
            Assert.Equal(404, objectResult?.StatusCode);
        }
    }
}
