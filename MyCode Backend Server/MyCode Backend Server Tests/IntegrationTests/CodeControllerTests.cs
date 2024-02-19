using MyCode_Backend_Server.Contracts.Registers;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.IntegrationTests
{
    [Collection("firstSequence")]
    public class CodeControllerTests(CustomWebApplicationFactory<MyCode_Backend_Server.Program> factory) : IClassFixture<CustomWebApplicationFactory<MyCode_Backend_Server.Program>>
    {
        private readonly CustomWebApplicationFactory<MyCode_Backend_Server.Program> _factory = factory;

        [Fact]
        public async Task Get_GetAllCodesByUserEndpoint_Unauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/codes/by-user");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Get_GetAllCodesByVisibilityEndpoint_Unauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/codes/by-visibility");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Get_GetCodeByIdEndpoint_Unauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/codes/code-some-id");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Post_CreateCodeEndpoint_Unauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var codeRequest = new CodeRegRequest("", "", "", false, false);

            // Act
            var response = await client.PostAsJsonAsync("/codes/register", codeRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Put_UpdateCodeEndpoint_Unauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var updatedCode = new CodeRegRequest("", "", "", false, false);

            // Act
            var response = await client.PutAsJsonAsync("/codes/cupdate-some-id", updatedCode);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Delete_DeleteCodeByUserEndpoint_Unauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("/codes/cdelete-some-id");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
