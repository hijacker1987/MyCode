using System.Net;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using MyCode_Backend_Server;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.IntegrationTests
{
    public class IntegrationTests(WebApplicationFactory<Startup> factory) : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory = factory;

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
    }
}
