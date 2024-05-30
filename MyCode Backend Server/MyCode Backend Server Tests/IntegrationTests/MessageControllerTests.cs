﻿using Microsoft.AspNetCore.Mvc.Testing;
using MyCode_Backend_Server.Contracts.Services;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server_Tests.Services;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.IntegrationTests
{
    [Collection("firstSequence")]

    public class MessageControllerTests(CustomWebApplicationFactory<MyCode_Backend_Server.Program> factory) : IClassFixture<CustomWebApplicationFactory<MyCode_Backend_Server.Program>>
    {
        private readonly CustomWebApplicationFactory<MyCode_Backend_Server.Program> _factory = factory;

        private async Task<(HttpClient, User)> GetClientWithAuth(string role)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            if (role == "User")
            {
                var authRequest = new AuthRequest("tester8@test.com", "Password", "Password");
                var user = TestLogin.Login_With_Test_User_Return_User(authRequest, client);

                return (client, await user);
            }
            else if (role == "Support")
            {
                var authRequest = new AuthRequest("supportuser@mycodetest.com", "SupportPassword", "SupportPassword");
                var user = TestLogin.Login_With_Test_User_Return_User(authRequest, client);

                return (client, await user);
            }
            else
            {
                var authRequest = new AuthRequest("admin@test.com", "AdminPassword", "AdminPassword");
                var user = TestLogin.Login_With_Test_User_Return_User(authRequest, client);

                return (client, await user);
            }
        }

        [Fact]
        public async Task Create_WithValidMessage_ReturnsOk()
        {
            // Arrange
            var (client, user) = await GetClientWithAuth("User");
            var message = new SupportChat { UserId = user.Id, Text = "Hello world" };

            // Act
            var response = await client.PostAsJsonAsync("/ws/message", message);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetRoom_WithValidRequest_ReturnsForbidden()
        {
            // Arrange
            var (client, user) = await GetClientWithAuth("User");

            // Act
            var response = await client.GetAsync("/ws/message/get-room");

            // Assert
            Assert.NotNull(user);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetActiveRoom_WithValidRequest_ReturnsOk()
        {
            // Arrange
            var (client, user) = await GetClientWithAuth("Admin");

            // Act
            var response = await client.GetAsync("/ws/message/get-active-room");

            // Assert
            response.EnsureSuccessStatusCode();
            var activeUserIds = await response.Content.ReadFromJsonAsync<List<string>>();
            Assert.NotNull(user);
            Assert.NotNull(activeUserIds);
        }

        [Fact]
        public async Task GetAnyArchived_WithValidRequest_ReturnsNotFound()
        {
            // Arrange
            var (client, user) = await GetClientWithAuth("User");

            // Act
            var response = await client.GetAsync("/ws/message/getAnyArchived");

            // Assert
            Assert.NotNull(user);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetOwnArchived_WithValidRequest_ReturnsNotFound()
        {
            // Arrange
            var (client, user) = await GetClientWithAuth("User");

            // Act
            var response = await client.GetAsync("/ws/message/getOwnArchived");

            // Assert
            Assert.NotNull(user);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetArchivedById_WithValidId_ReturnsForbidden()
        {
            // Arrange
            var (client, user) = await GetClientWithAuth("User");
            var id = user.Id;

            // Act
            var response = await client.GetAsync($"/ws/message/getArchived-{id}");

            // Assert
            Assert.NotNull(user);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetActiveById_WithValidId_ReturnsForbidden()
        {
            // Arrange
            var (client, user) = await GetClientWithAuth("User");
            var id = user.Id;

            // Act
            var response = await client.GetAsync($"/ws/message/getActive-{id}");

            // Assert
            Assert.NotNull(user);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DropBackActiveToSupportById_WithValidId_ReturnsNotFound()
        {
            // Arrange
            var (client, user) = await GetClientWithAuth("Admin");
            var id = user.Id;

            // Act
            var response = await client.PutAsync($"/ws/message/uActive-{id}", null);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
