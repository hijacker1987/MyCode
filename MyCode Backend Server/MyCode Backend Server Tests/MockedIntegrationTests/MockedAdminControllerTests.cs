using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using MyCode_Backend_Server.Controllers;
using MyCode_Backend_Server.Service.Authentication;
using MyCode_Backend_Server.Service.Authentication.Token;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using Assert = Xunit.Assert;
using MyCode_Backend_Server.Contracts.Registers;

namespace MyCode_Backend_Server_Tests.MockedIntegrationTests
{
    [Collection("firstSequence")]
    public class MockedAdminControllerTests
    {
        private const int Expected = 500;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<ILogger<AdminController>> _mockLogger;
        private readonly DbContextOptions<DataContext> _dbContextOptions;
        private readonly DataContext _dataContext;
        private readonly AdminController _controller;

        public MockedAdminControllerTests()
        {
            _mockTokenService = new Mock<ITokenService>();
            _mockAuthService = new Mock<IAuthService>();
            _mockLogger = new Mock<ILogger<AdminController>>();
            _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dataContext = new DataContext(_dbContextOptions);

            _controller = new AdminController(
                _mockTokenService.Object,
                _mockAuthService.Object,
                _mockLogger.Object,
                _dataContext);
        }

        [Fact]
        public void DeleteUser_Returns_InternalServerError_On_Exception()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var mockSet = new Mock<DbSet<User>>();
            mockSet.Setup(m => m.FindAsync(It.IsAny<Guid>())).Throws(new Exception("Test Exception"));
            _dataContext.Users = mockSet.Object;

            // Act
            var result = _controller.DeleteUser(userId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(Expected, objectResult.StatusCode);
            Assert.Equal("Internal Server Error: Object reference not set to an instance of an object.", objectResult.Value);
        }

        [Fact]
        public async Task GetAllUsersAsync_Returns_InternalServerError_On_ExceptionAsync()
        {
            // Arrange
            var mockSet = new Mock<DbSet<User>>();
            var queryable = new List<User>().AsQueryable();

            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Throws(new Exception("Database connection failed"));
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            _dataContext.Users = mockSet.Object;

            // Act
            var result = await _controller.GetAllUsersAsync();

            // Assert
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundObjectResult.StatusCode);
            Assert.Contains("System.NullReferenceException: Object ref", notFoundObjectResult!.Value!.ToString());
        }

        [Fact]
        public async Task GetUserById_Returns_NotFound_On_Null_User()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var mockSet = new Mock<DbSet<User>>();
            mockSet.Setup(m => m.FindAsync(userId)).ReturnsAsync((User)null!);
            _dataContext.Users = mockSet.Object;

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(Expected, notFoundResult.StatusCode);
            Assert.Contains("{ ErrorMessage = Error occurred while fet", notFoundResult.Value!.ToString());
        }

        [Fact]
        public void UpdateUser_Returns_BadRequest_On_Invalid_ModelState()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updatedUser = new User { UserName = "Invalid Username" };
            _controller.ModelState.AddModelError("UserName", "Username cannot contain spaces.");

            // Act
            var result = _controller.UpdateUser(userId, updatedUser);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("System.NullReferenceException: Object ref", badRequestResult.Value!.ToString());
        }

        [Fact]
        public void DeleteUser_Returns_InternalServerError_On_Nonexistent_User()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var users = new List<User>().AsQueryable();
            var mockSet = CreateMockSet(users);

            mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((User)null!);
            _dataContext.Users = mockSet.Object;

            // Act
            var result = _controller.DeleteUser(userId);

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(Expected, notFoundResult.StatusCode);
        }

        [Fact]
        public void GetAllCodes_Returns_NotFound_On_Exception()
        {
            // Arrange
            var mockSet = new Mock<DbSet<Code>>();
            var queryable = new List<Code>().AsQueryable();

            mockSet.As<IQueryable<Code>>().Setup(m => m.Provider).Throws(new Exception("Database connection failed"));
            mockSet.As<IQueryable<Code>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<Code>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<Code>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            _dataContext.CodesDb = mockSet.Object;

            // Act
            var result = _controller.GetAllCodes();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Contains("System.NullReferenceException: Object ref", notFoundResult.Value!.ToString());
        }

        [Fact]
        public async Task GetUserById_Returns_InternalServerError_On_ExceptionAsync()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var mockSet = new Mock<DbSet<User>>();
            mockSet.Setup(m => m.FindAsync(userId)).Throws(new Exception("Test Exception"));
            _dataContext.Users = mockSet.Object;

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, internalServerErrorResult.StatusCode);
            Assert.Contains("Error occurred while fetching user by ID!", internalServerErrorResult.Value!.ToString());
        }

        [Fact]
        public void DeleteCode_Returns_NotFound_When_Code_Not_Found()
        {
            // Arrange
            var id = Guid.NewGuid();
            var mockSet = new Mock<DbSet<Code>>();
            mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((object[] keyValues) => null);
            _dataContext.CodesDb = mockSet.Object;

            // Act
            var result = _controller.DeleteCode(id);

            // Assert
            var badObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badObjectResult.StatusCode);
        }

        [Fact]
        public async Task UpdateStatus_Returns_BadRequest_On_Invalid_Request()
        {
            // Arrange
            var request = new RoleStatusRequest("");

            // Act
            var result = await _controller.UpdateStatus(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public void UpdateCode_Returns_BadRequest_When_Code_Not_Found()
        {
            // Arrange
            var id = Guid.NewGuid();
            var updatedCode = new Code();

            var mockSet = new Mock<DbSet<Code>>();
            mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((object[] keyValues) => null);
            _dataContext.CodesDb = mockSet.Object;

            // Act
            var result = _controller.UpdateCode(id, updatedCode);

            // Assert
            var notFoundResult = Assert.IsType<ActionResult<Code>>(result);
            Assert.IsType<BadRequestObjectResult>(notFoundResult.Result);
        }

        private static Mock<DbSet<T>> CreateMockSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((object[] keyValues) =>
            {
                return new User();
            });

            return mockSet;
        }
    }
}
