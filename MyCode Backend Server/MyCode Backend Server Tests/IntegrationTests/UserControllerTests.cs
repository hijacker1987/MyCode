using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace MyCode_Backend_Server_Tests.IntegrationTests
{
    [Collection("firstSequence")]
    public class UsersControllerTests(CustomWebApplicationFactory<MyCode_Backend_Server.Program> factory)
                      : IClassFixture<CustomWebApplicationFactory<MyCode_Backend_Server.Program>>
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

            Assert.IsTrue(response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData("/registerUser")]
        public async Task Post_RegisterUserEndpoint_InvalidRequest_ReturnsBadRequest(string url)
        {
            var client = _factory.CreateClient();

            var invalidUserRegRequest = new
            {
                //Invalid request with missing required fields
            };

            var response = await client.PostAsJsonAsync(url, invalidUserRegRequest);

            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("/login")]
        public async Task Post_LoginEndpoint_InvalidCredentials_ReturnsBadRequest(string url)
        {
            var client = _factory.CreateClient();

            var invalidAuthRequest = new
            {
                //Invalid credentials
            };

            var response = await client.PostAsJsonAsync(url, invalidAuthRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
