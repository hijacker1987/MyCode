﻿using System.Net;
using System.Net.Http.Json;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.IntegrationTests
{
    [Collection("firstSequence")]
    public class UsersControllerTests(CustomWebApplicationFactory<MyCode_Backend_Server.Program> factory) : IClassFixture<CustomWebApplicationFactory<MyCode_Backend_Server.Program>>
    {
        private readonly CustomWebApplicationFactory<MyCode_Backend_Server.Program> _factory = factory;

        [Theory]
        [InlineData("/register")]
        public async Task Post_RegisterUserEndpoint_InvalidRequest_ReturnsNotFound(string url)
        {
            var client = _factory.CreateClient();

            var invalidUserRegRequest = new
            {
                //Invalid request with missing required fields
            };

            var response = await client.PostAsJsonAsync(url, invalidUserRegRequest);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("/login")]
        public async Task Post_LoginEndpoint_InvalidCredentials_ReturnsNotFound(string url)
        {
            var client = _factory.CreateClient();

            var invalidAuthRequest = new
            {
                //Invalid credentials
            };

            var response = await client.PostAsJsonAsync(url, invalidAuthRequest);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
