using MyCode_Backend_Server.Contracts.Registers;
using System.Net.Http.Json;
using System.Net;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Assert = Xunit.Assert;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Service.Authentication.Token;
using MyCode_Backend_Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCode_Backend_Server.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;


namespace MyCode_Backend_Server_Tests.IntegrationTests
{
    public class AuthAndStartupTests(CustomWebApplicationFactory<Program> factory)
           : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory = factory;

        [Fact]
        public async Task ChangePasswordAsync_NotLoggedIn_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var request = new
            {
                Email = "notLoggedIn@email.com",
                CurrentPassword = "invalidLogin",
                NewPassword = "newpassword",
                ConfirmPassword = "newpassword"
            };

            // Act
            var response = await client.PatchAsync("/users/changePassword", JsonContent.Create(request));

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteAccountAsync_NonExistent_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var emailToDelete = "unauthorized@user.com";

            // Act
            var response = await client.DeleteAsync($"/users/deleteAccount?email={emailToDelete}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task RegisterUserAsync_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var invalidRequest = new UserRegRequest("invalidemail", "", "shortpw", "", "");

            // Act
            var response = await client.PostAsync("/users/register", JsonContent.Create(invalidRequest));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [Fact]
        public async Task LoginAsync_NonExistent_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var invalidLoginRequest = new
            {
                Email = "nonexistent@email.com",
                Password = "invalidpassword",
                ConfirmPassword = "invalidpassword"
            };

            // Act
            var response = await client.PostAsync("/users/login", JsonContent.Create(invalidLoginRequest));

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public void ConfigureServices_AddsRequiredServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            var environment = new Mock<IWebHostEnvironment>().Object;
            var startup = new Startup(configuration, environment);

            // Act
            startup.ConfigureServices(services);

            // Assert
            Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(DataContext));
            Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(ITokenService));
        }

        [Fact]
        public async Task Initialize_NotCreatesDatabaseAndAdminUser_WhenNoAdminExists()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "Test_DbInitializer_CreatesDatabaseAndAdminUser")
                .Options;
            var dbContext = new DataContext(dbContextOptions);

            var users = new List<User>();

            // Setup a mock UserManager<User> instance
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

            userManagerMock.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Success)
                           .Callback<User, string>((user, password) => users.Add(user)); // Add the created user to the list
            userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                           .ReturnsAsync((string email) => users.FirstOrDefault(u => u.Email == email));

            // Setup a mock RoleManager<IdentityRole<Guid>> instance
            var roles = new List<IdentityRole<Guid>>();
            var roleManagerMock = new Mock<RoleManager<IdentityRole<Guid>>>(
                new Mock<IRoleStore<IdentityRole<Guid>>>().Object,
                new List<IRoleValidator<IdentityRole<Guid>>>(),
                new Mock<ILookupNormalizer>().Object,
                new IdentityErrorDescriber(),
                new Mock<ILogger<RoleManager<IdentityRole<Guid>>>>().Object
            );

            roleManagerMock.Setup(m => m.RoleExistsAsync(It.IsAny<string>()))
                           .ReturnsAsync((string roleName) => roles.Any(r => r.Name == roleName));
            roleManagerMock.Setup(m => m.CreateAsync(It.IsAny<IdentityRole<Guid>>()))
                           .ReturnsAsync(IdentityResult.Success)
                           .Callback<IdentityRole<Guid>>(role => roles.Add(role));

            var configuration = new ConfigurationBuilder().Build();
            var initializer = new DbInitializer(configuration);

            // Act
            await initializer.Initialize(dbContext, userManagerMock.Object, roleManagerMock.Object);

            // Assert
            var adminUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "admin@example.com");
            Assert.Null(adminUser);
            Assert.False(await userManagerMock.Object.IsInRoleAsync(adminUser!, "Admin"));
        }

        [Fact]
        public async Task Initialize_CreatesUsersAndTheirCodes()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "Test_DbInitializer_CreatesUsersAndTheirCodes")
                .Options;
            var dbContext = new DataContext(dbContextOptions);

            var users = new List<User>();

            // Setup a mock UserManager<User> instance
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

            userManagerMock.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Success)
                           .Callback<User, string>((user, password) => users.Add(user)); // Add the created user to the list
            userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                           .ReturnsAsync((string email) => users.FirstOrDefault(u => u.Email == email));

            // Setup a mock RoleManager<IdentityRole<Guid>> instance
            var roles = new List<IdentityRole<Guid>>();
            var roleManagerMock = new Mock<RoleManager<IdentityRole<Guid>>>(
                new Mock<IRoleStore<IdentityRole<Guid>>>().Object,
                new List<IRoleValidator<IdentityRole<Guid>>>(),
                new Mock<ILookupNormalizer>().Object,
                new IdentityErrorDescriber(),
                new Mock<ILogger<RoleManager<IdentityRole<Guid>>>>().Object
            );

            roleManagerMock.Setup(m => m.RoleExistsAsync(It.IsAny<string>()))
                           .ReturnsAsync((string roleName) => roles.Any(r => r.Name == roleName));
            roleManagerMock.Setup(m => m.CreateAsync(It.IsAny<IdentityRole<Guid>>()))
                           .ReturnsAsync(IdentityResult.Success)
                           .Callback<IdentityRole<Guid>>(role => roles.Add(role));

            var configuration = new ConfigurationBuilder().Build();
            var initializer = new DbInitializer(configuration);

            // Act
            await initializer.Initialize(dbContext, userManagerMock.Object, roleManagerMock.Object);

            // Assert
            var usersList = await dbContext.Users.ToListAsync();
            foreach (var user in usersList)
            {
                var codes = await dbContext.CodesDb!.Where(c => c.UserId == user.Id).ToListAsync();
                Assert.NotEmpty(codes);
            }
        }

        [Fact]
        public async Task Initialize_NotCreatesTestDatabaseAndUsers()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "Test_TestDbInitializer_CreatesTestDatabaseAndUsers")
                .Options;
            var dbContext = new DataContext(dbContextOptions);

            var users = new List<User>();

            // Setup a mock UserManager<User> instance
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

            userManagerMock.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Success)
                           .Callback<User, string>((user, password) => users.Add(user)); // Add the created user to the list
            userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                           .ReturnsAsync((string email) => users.FirstOrDefault(u => u.Email == email));

            // Setup a mock RoleManager<IdentityRole<Guid>> instance
            var roles = new List<IdentityRole<Guid>>();
            var roleManagerMock = new Mock<RoleManager<IdentityRole<Guid>>>(
                new Mock<IRoleStore<IdentityRole<Guid>>>().Object,
                new List<IRoleValidator<IdentityRole<Guid>>>(),
                new Mock<ILookupNormalizer>().Object,
                new IdentityErrorDescriber(),
                new Mock<ILogger<RoleManager<IdentityRole<Guid>>>>().Object
            );

            roleManagerMock.Setup(m => m.RoleExistsAsync(It.IsAny<string>()))
                           .ReturnsAsync((string roleName) => roles.Any(r => r.Name == roleName));
            roleManagerMock.Setup(m => m.CreateAsync(It.IsAny<IdentityRole<Guid>>()))
                           .ReturnsAsync(IdentityResult.Success)
                           .Callback<IdentityRole<Guid>>(role => roles.Add(role));

            var initializer = new TestDbInitializer();

            // Act
            await initializer.Initialize(dbContext, userManagerMock.Object, roleManagerMock.Object);

            // Assert
            var testAdminUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "admin@test.com");
            Assert.Null(testAdminUser);
            Assert.False(await userManagerMock.Object.IsInRoleAsync(testAdminUser!, "Admin"));

            var testUsers = await dbContext.Users.Where(u => u.Email!.Contains("@test.com")).ToListAsync();
            Assert.Empty(testUsers);
        }
    }
}
