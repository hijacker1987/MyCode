using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Contracts.Services;
using MyCode_Backend_Server_Tests.Services;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Assert = Xunit.Assert;

namespace MyCode_Backend_Server_Tests.IntegrationTests
{
    [Collection("firstSequence")]
    public class CodeControllerTests : IClassFixture<CustomWebApplicationFactory<MyCode_Backend_Server.Program>>
    {
        private readonly CustomWebApplicationFactory<MyCode_Backend_Server.Program> _factory;
        private readonly HttpClient _client;

        public CodeControllerTests(CustomWebApplicationFactory<MyCode_Backend_Server.Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

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

        [Fact]
        public async Task Get_GetAllCodesByUserEndpoint_Authorized()
        {
            // Arrange
            var authRequest = new AuthRequest("user13@example.com", "Password", "Password");
            var (authToken, cookies) = await TestLogin.Login_With_Test_User(authRequest, _client);

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            // Act
            var response = await _client.GetAsync("/codes/by-user");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_CreateCodeEndpoint_ValidRequest_ReturnsCreated()
        {
            // Arrange
            var authRequest = new AuthRequest("user13@example.com", "Password", "Password");
            var (authToken, cookies) = await TestLogin.Login_With_Test_User(authRequest, _client);

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            var codeRequest = new CodeRegRequest("Sample Code", "console.log('Hello, World!');", "JavaScript", false, true);

            // Act
            var response = await _client.PostAsJsonAsync("/codes/register", codeRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Post_CreateCodeEndpoint_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var authRequest = new AuthRequest("user13@example.com", "Password", "Password");
            var (authToken, cookies) = await TestLogin.Login_With_Test_User(authRequest, _client);

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            var invalidCodeRequest = new CodeRegRequest("", "", "", false, true);

            // Act
            var response = await _client.PostAsJsonAsync("/codes/register", invalidCodeRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
