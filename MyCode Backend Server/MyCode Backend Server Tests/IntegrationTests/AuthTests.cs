using Microsoft.AspNetCore.Identity;
using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Service.Authentication;
using System.Net.Http.Json;
using System.Net;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Assert = Xunit.Assert;
using Microsoft.Extensions.DependencyInjection;

namespace MyCode_Backend_Server_Tests.IntegrationTests
{
    public class AuthTests : IClassFixture<CustomWebApplicationFactory<MyCode_Backend_Server.Program>>
    {
        private readonly CustomWebApplicationFactory<MyCode_Backend_Server.Program> _factory;

        public AuthTests(CustomWebApplicationFactory<MyCode_Backend_Server.Program> factory)
        {
            _factory = factory;
        }
 
        [Fact]
        public async Task ChangePasswordAsync_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var request = new
            {
                Email = "nonexistent@email.com",
                CurrentPassword = "invalidpassword",
                NewPassword = "newpassword"
            };

            // Act
            var response = await client.PatchAsync("/changePassword", JsonContent.Create(request));

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteAccountAsync_UnauthorizedUser_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var emailToDelete = "unauthorized@user.com";

            // Act
            var response = await client.DeleteAsync($"/deleteAccount?email={emailToDelete}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task RegisterUserAsync_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var invalidRequest = new UserRegRequest("", "", "", "");

            // Act
            var response = await client.PostAsync("/registerUser", JsonContent.Create(invalidRequest));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var invalidLoginRequest = new
            {
                Email = "nonexistent@email.com",
                Password = "invalidpassword"
            };

            // Act
            var response = await client.PostAsync("/login", JsonContent.Create(invalidLoginRequest));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
