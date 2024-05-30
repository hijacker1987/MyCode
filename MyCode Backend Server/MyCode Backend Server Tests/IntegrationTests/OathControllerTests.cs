using Xunit;
using System.Net;
using MyCode_Backend_Server;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.IntegrationTests
{
    public class OathControllerTests(CustomWebApplicationFactory<Startup> factory) : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task GoogleResponse_WithoutAuthentication_ReturnsBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/account/google-response");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task FacebookResponse_WithoutAuthentication_ReturnsBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/account/facebook-response");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GitHubResponse_WithoutAuthentication_ReturnsBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/account/github-response");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}