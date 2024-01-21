using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using MyCode_Backend_Server.Models;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.IntegrationTests
{
    [Collection("firstSequence")]
    public class AdminControllerTests(CustomWebApplicationFactory<MyCode_Backend_Server.Program> factory) : IClassFixture<CustomWebApplicationFactory<MyCode_Backend_Server.Program>>
    {
        private readonly CustomWebApplicationFactory<MyCode_Backend_Server.Program> _factory = factory;

        [Theory]
        [InlineData("/getUsers")]
        public async Task Get_GetUsersEndpoint_NoAuth_ReturnsError(string url)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync(url);

            Assert.True(response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized or HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_GetCodesEndpoint_NoAuth_ReturnsError()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync("/admin/getCodes");

            Assert.True(response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized or HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Put_UpdateUserEndpoint_NoAuth_ReturnsError()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.PutAsJsonAsync("/admin/au-123", new User());

            Assert.True(response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized or HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Put_UpdateCodeEndpoint_NoAuth_ReturnsError()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.PutAsJsonAsync("/admin/acu-123", new Code());

            Assert.True(response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized or HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_DeleteCodeEndpoint_NoAuth_ReturnsError()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.DeleteAsync("/admin/ad-123");

            Assert.True(response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized or HttpStatusCode.NotFound);
        }
    }
}
