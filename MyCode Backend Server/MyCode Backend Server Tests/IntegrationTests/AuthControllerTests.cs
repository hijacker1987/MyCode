using MyCode_Backend_Server.Models;
using Newtonsoft.Json;
using System.Text;
using Xunit;
using Assert = Xunit.Assert;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using MyCode_Backend_Server.Contracts.Services;
using MyCode_Backend_Server_Tests.Services;

namespace MyCode_Backend_Server_Tests.IntegrationTests
{
    [Collection("firstSequence")]

    public class AuthControllerTests(CustomWebApplicationFactory<MyCode_Backend_Server.Program> factory) : IClassFixture<CustomWebApplicationFactory<MyCode_Backend_Server.Program>>
    {
        private readonly CustomWebApplicationFactory<MyCode_Backend_Server.Program> _factory = factory;

        [Fact]
        public async Task BasicsTwoFactor_Returns_Unauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var userId = Guid.NewGuid().ToString();

            // Act
            var response = await client.GetAsync($"/auth/basicsTwoFactor?userId={userId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task EnableTwoFactor_Returns_Unauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var userId = Guid.NewGuid().ToString();

            // Act
            var response = await client.PostAsync("/auth/enableTwoFactor", new StringContent(JsonConvert.SerializeObject(userId), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task VerifyTwoFactor_Returns_Unauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = new VerifyModel("verification_code", false);

            // Act
            var response = await client.PostAsync("/auth/verifyTwoFactor", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DisableTwoFactor_Returns_Unauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var userId = Guid.NewGuid().ToString();

            // Act
            var response = await client.PostAsync("/auth/disableTwoFactor", new StringContent(JsonConvert.SerializeObject(userId), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task EnableTwoFactor_Returns_BadRequest()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var authRequest = new AuthRequest("tester7@test.com", "Password", "Password");
            var user = await TestLogin.Login_With_Test_User_Return_User(authRequest, client);

            // Act
            var response = await client.PostAsync("/auth/enableTwoFactor", new StringContent(JsonConvert.SerializeObject(user.Id), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task VerifyTwoFactor_Returns_BadRequest()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var authRequest = new AuthRequest("tester7@test.com", "Password", "Password");
            await TestLogin.Login_With_Test_User_Return_User(authRequest, client);
            var request = new VerifyModel("verification_code", false);

            // Act
            var response = await client.PostAsync("/auth/verifyTwoFactor", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DisableTwoFactor_Returns_NotFound()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var authRequest = new AuthRequest("tester7@test.com", "Password", "Password");
            var user = await TestLogin.Login_With_Test_User_Return_User(authRequest, client);

            // Act
            var response = await client.PostAsync("/auth/disableTwoFactor", new StringContent(JsonConvert.SerializeObject(user.Id), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
