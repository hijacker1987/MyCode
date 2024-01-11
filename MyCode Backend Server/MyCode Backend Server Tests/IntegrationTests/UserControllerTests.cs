using System.Net;
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
    }
}
