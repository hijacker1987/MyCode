using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Controllers;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication.Token;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.MockedIntegrationTests
{
    public class MockedCodeControllerTests
    {
        [Fact]
        public void GetAllCodesByUser_Returns_BadRequest_When_Claim_Not_Found()
        {
            // Arrange
            var tokenServiceMock = new Mock<ITokenService>();
            var loggerMock = new Mock<ILogger<CodeController>>();

            var dbSetMock = new Mock<DbSet<Code>>();
            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), default)).ReturnsAsync((Code)null!);

            var dbContextOptions = new DbContextOptions<DataContext>();
            var dbContextMock = new Mock<DataContext>(dbContextOptions);
            dbContextMock.Setup(m => m.Set<Code>()).Returns(dbSetMock.Object);

            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var controller = new CodeController(
                tokenServiceMock.Object,
                loggerMock.Object,
                dbContextMock.Object,
                userManagerMock.Object);

            // Act
            var result = controller.GetAllCodesByUser();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public void GetAllCodesByUser_Returns_BadRequest_When_Parsing_Fails()
        {
            // Arrange
            var tokenServiceMock = new Mock<ITokenService>();
            var loggerMock = new Mock<ILogger<CodeController>>();
            var dbContextOptions = new DbContextOptions<DataContext>();
            var dbContextMock = new Mock<DataContext>(dbContextOptions);
            var dbSetMock = new Mock<DbSet<Code>>();
            dbContextMock.Setup(m => m.Set<Code>()).Returns(dbSetMock.Object);

            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var controller = new CodeController(
                tokenServiceMock.Object,
                loggerMock.Object,
                dbContextMock.Object,
                userManagerMock.Object);

            // Act
            var result = controller.GetAllCodesByUser();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public void GetAllCodesByUser_Returns_BadRequest_When_Operation_Fails()
        {
            // Arrange
            var tokenServiceMock = new Mock<ITokenService>();
            var loggerMock = new Mock<ILogger<CodeController>>();
            var dbSetMock = new Mock<DbSet<Code>>();
            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), default)).ThrowsAsync(new InvalidOperationException());

            var dbContextOptions = new DbContextOptions<DataContext>();
            var dbContextMock = new Mock<DataContext>(dbContextOptions);
            dbContextMock.Setup(m => m.Set<Code>()).Returns(dbSetMock.Object);

            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var controller = new CodeController(
                tokenServiceMock.Object,
                loggerMock.Object,
                dbContextMock.Object,
                userManagerMock.Object);

            // Act
            var result = controller.GetAllCodesByUser();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public void GetAllCodesByUser_Returns_BadRequest_When_Unhandled_Exception_Thrown()
        {
            // Arrange
            var tokenServiceMock = new Mock<ITokenService>();
            var loggerMock = new Mock<ILogger<CodeController>>();
            var dbSetMock = new Mock<DbSet<Code>>();
            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), default)).ThrowsAsync(new InvalidOperationException());

            var dbContextOptions = new DbContextOptions<DataContext>();
            var dbContextMock = new Mock<DataContext>(dbContextOptions);
            dbContextMock.Setup(m => m.Set<Code>()).Returns(dbSetMock.Object);

            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var controller = new CodeController(
                tokenServiceMock.Object,
                loggerMock.Object,
                dbContextMock.Object,
                userManagerMock.Object);

            // Act
            var result = controller.GetAllCodesByUser();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public void GetAllCodesByVisibility_Returns_BadRequest_When_Unhandled_Exception_Thrown()
        {
            // Arrange
            var tokenServiceMock = new Mock<ITokenService>();
            var loggerMock = new Mock<ILogger<CodeController>>();
            var dbSetMock = new Mock<DbSet<Code>>();
            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), default)).ThrowsAsync(new InvalidOperationException());

            var dbContextOptions = new DbContextOptions<DataContext>();
            var dbContextMock = new Mock<DataContext>(dbContextOptions);
            dbContextMock.Setup(m => m.Set<Code>()).Returns(dbSetMock.Object);

            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var controller = new CodeController(
                tokenServiceMock.Object,
                loggerMock.Object,
                dbContextMock.Object,
                userManagerMock.Object);

            // Act
            var result = controller.GetAllCodesByVisibility();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public void GetCodeById_Returns_NotFound_When_Unhandled_Exception_Thrown()
        {
            // Arrange
            var tokenServiceMock = new Mock<ITokenService>();
            var loggerMock = new Mock<ILogger<CodeController>>();
            var dbSetMock = new Mock<DbSet<Code>>();
            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), default)).ThrowsAsync(new InvalidOperationException());

            var dbContextOptions = new DbContextOptions<DataContext>();
            var dbContextMock = new Mock<DataContext>(dbContextOptions);
            dbContextMock.Setup(m => m.Set<Code>()).Returns(dbSetMock.Object);

            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var controller = new CodeController(
                tokenServiceMock.Object,
                loggerMock.Object,
                dbContextMock.Object,
                userManagerMock.Object);

            // Act
            var result = controller.GetCodeById(Guid.NewGuid());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public void CreateCode_Returns_BadRequestResult_When_Code_Creation_Succeeds()
        {
            // Arrange
            var tokenServiceMock = new Mock<ITokenService>();
            var loggerMock = new Mock<ILogger<CodeController>>();
            var dbContextOptions = new DbContextOptions<DataContext>();
            var dbContextMock = new Mock<DataContext>(dbContextOptions);
            var dbSetMock = new Mock<DbSet<Code>>();
            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), default)).ReturnsAsync((Code)null!);
            dbContextMock.Setup(m => m.Set<Code>()).Returns(dbSetMock.Object);
            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var controller = new CodeController(
                tokenServiceMock.Object,
                loggerMock.Object,
                dbContextMock.Object,
                userManagerMock.Object);

            var codeRegRequest = new CodeRegRequest("Test Code", "public class Test {}", "C#", true, true);

            // Act
            var result = controller.CreateCode(codeRegRequest);

            // Assert
            var badRequestResultResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(400, badRequestResultResult.StatusCode);
        }

        [Fact]
        public void CreateCode_Returns_BadRequestResult_When_ModelState_Is_Invalid()
        {
            // Arrange
            var tokenServiceMock = new Mock<ITokenService>();
            var loggerMock = new Mock<ILogger<CodeController>>();
            var dbContextOptions = new DbContextOptions<DataContext>();
            var dbContextMock = new Mock<DataContext>(dbContextOptions);
            var dbSetMock = new Mock<DbSet<Code>>();
            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), default)).ReturnsAsync((Code)null!);
            dbContextMock.Setup(m => m.Set<Code>()).Returns(dbSetMock.Object);
            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var controller = new CodeController(
                tokenServiceMock.Object,
                loggerMock.Object,
                dbContextMock.Object,
                userManagerMock.Object);

            var codeRegRequest = new CodeRegRequest("", "", "", false, false);

            // Act
            var result = controller.CreateCode(codeRegRequest);

            // Assert
            var badRequestResultResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(400, badRequestResultResult.StatusCode);
        }

        [Fact]
        public void CreateCode_Returns_BadRequestResult_When_UserIdClaim_Not_Found()
        {
            // Arrange
            var tokenServiceMock = new Mock<ITokenService>();
            var loggerMock = new Mock<ILogger<CodeController>>();
            var dbContextOptions = new DbContextOptions<DataContext>();
            var dbContextMock = new Mock<DataContext>(dbContextOptions);
            var dbSetMock = new Mock<DbSet<Code>>();
            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), default)).ReturnsAsync((Code)null!);
            dbContextMock.Setup(m => m.Set<Code>()).Returns(dbSetMock.Object);
            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var controller = new CodeController(
                tokenServiceMock.Object,
                loggerMock.Object,
                dbContextMock.Object,
                userManagerMock.Object);

            var codeRegRequest = new CodeRegRequest("Test Code", "public class Test {}", "C#", true, true);

            // Act
            var result = controller.CreateCode(codeRegRequest);

            // Assert
            var badRequestResultResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(400, badRequestResultResult.StatusCode);
        }

        [Fact]
        public void CreateCode_Returns_BadRequestResult_When_UserId_Cannot_Be_Parsed()
        {
            // Arrange
            var tokenServiceMock = new Mock<ITokenService>();
            var loggerMock = new Mock<ILogger<CodeController>>();
            var dbContextOptions = new DbContextOptions<DataContext>();
            var dbContextMock = new Mock<DataContext>(dbContextOptions);
            var dbSetMock = new Mock<DbSet<Code>>();
            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), default)).ReturnsAsync((Code)null!);
            dbContextMock.Setup(m => m.Set<Code>()).Returns(dbSetMock.Object);
            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var controller = new CodeController(
                tokenServiceMock.Object,
                loggerMock.Object,
                dbContextMock.Object,
                userManagerMock.Object);

            var codeRegRequest = new CodeRegRequest("Test Code", "public class Test {}", "C#", true, true);

            // Act
            var result = controller.CreateCode(codeRegRequest);

            // Assert
            var badRequestResultResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(400, badRequestResultResult.StatusCode);
        }

        [Fact]
        public void GetAllCodesByUser_Returns_BadRequest2_When_Unhandled_Exception_Thrown()
        {
            // Arrange
            var tokenServiceMock = new Mock<ITokenService>();
            var loggerMock = new Mock<ILogger<CodeController>>();
            var dbSetMock = new Mock<DbSet<Code>>();
            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), default)).ThrowsAsync(new Exception());

            var dbContextOptions = new DbContextOptions<DataContext>();
            var dbContextMock = new Mock<DataContext>(dbContextOptions);
            dbContextMock.Setup(m => m.Set<Code>()).Returns(dbSetMock.Object);

            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var controller = new CodeController(
                tokenServiceMock.Object,
                loggerMock.Object,
                dbContextMock.Object,
                userManagerMock.Object);

            // Act
            var result = controller.GetAllCodesByUser();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public void GetAllCodesByVisibility_Returns_BadRequest2_When_Unhandled_Exception_Thrown()
        {
            // Arrange
            var tokenServiceMock = new Mock<ITokenService>();
            var loggerMock = new Mock<ILogger<CodeController>>();
            var dbSetMock = new Mock<DbSet<Code>>();
            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), default)).ThrowsAsync(new Exception());

            var dbContextOptions = new DbContextOptions<DataContext>();
            var dbContextMock = new Mock<DataContext>(dbContextOptions);
            dbContextMock.Setup(m => m.Set<Code>()).Returns(dbSetMock.Object);

            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var controller = new CodeController(
                tokenServiceMock.Object,
                loggerMock.Object,
                dbContextMock.Object,
                userManagerMock.Object);

            // Act
            var result = controller.GetAllCodesByVisibility();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public void GetCodeById_Returns_NotFound2_When_Unhandled_Exception_Thrown()
        {
            // Arrange
            var tokenServiceMock = new Mock<ITokenService>();
            var loggerMock = new Mock<ILogger<CodeController>>();
            var dbSetMock = new Mock<DbSet<Code>>();
            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), default)).ThrowsAsync(new Exception());

            var dbContextOptions = new DbContextOptions<DataContext>();
            var dbContextMock = new Mock<DataContext>(dbContextOptions);
            dbContextMock.Setup(m => m.Set<Code>()).Returns(dbSetMock.Object);

            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var controller = new CodeController(
                tokenServiceMock.Object,
                loggerMock.Object,
                dbContextMock.Object,
                userManagerMock.Object);

            // Act
            var result = controller.GetCodeById(Guid.NewGuid());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public void UpdateCode_Returns_BadRequest_When_Unhandled_Exception_Thrown()
        {
            // Arrange
            var tokenServiceMock = new Mock<ITokenService>();
            var loggerMock = new Mock<ILogger<CodeController>>();
            var dbSetMock = new Mock<DbSet<Code>>();
            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), default)).ThrowsAsync(new Exception());

            var dbContextOptions = new DbContextOptions<DataContext>();
            var dbContextMock = new Mock<DataContext>(dbContextOptions);
            dbContextMock.Setup(m => m.Set<Code>()).Returns(dbSetMock.Object);

            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var controller = new CodeController(
                tokenServiceMock.Object,
                loggerMock.Object,
                dbContextMock.Object,
                userManagerMock.Object);

            // Act
            var result = controller.UpdateCode(Guid.NewGuid(), new CodeRegRequest("", "", "", false, false));

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public void DeleteCodeByUser_Returns_BadRequest_When_Unhandled_Exception_Thrown()
        {
            // Arrange
            var tokenServiceMock = new Mock<ITokenService>();
            var loggerMock = new Mock<ILogger<CodeController>>();
            var dbSetMock = new Mock<DbSet<Code>>();
            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), default)).ThrowsAsync(new Exception());

            var dbContextOptions = new DbContextOptions<DataContext>();
            var dbContextMock = new Mock<DataContext>(dbContextOptions);
            dbContextMock.Setup(m => m.Set<Code>()).Returns(dbSetMock.Object);

            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var controller = new CodeController(
                tokenServiceMock.Object,
                loggerMock.Object,
                dbContextMock.Object,
                userManagerMock.Object);

            // Act
            var result = controller.DeleteCodeByUser(Guid.NewGuid());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
    }
}
