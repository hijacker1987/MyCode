using MyCode_Backend_Server.Contracts.Registers;
using System.Net.Http.Json;
using System.Net;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Assert = Xunit.Assert;
using System.Net.Http.Headers;

namespace MyCode_Backend_Server_Tests.IntegrationTests
{
    public class AuthTests(CustomWebApplicationFactory<MyCode_Backend_Server.Program> factory)
           : IClassFixture<CustomWebApplicationFactory<MyCode_Backend_Server.Program>>
    {
        private readonly CustomWebApplicationFactory<MyCode_Backend_Server.Program> _factory = factory;

        [Fact]
        public async Task Swagger_EndpointIsAccessible()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/swagger");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

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
    }
}
