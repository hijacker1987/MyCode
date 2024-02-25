using Azure.Core;
using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Contracts.Services;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server_Tests.Services;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Xunit;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Assert = Xunit.Assert;
using User = MyCode_Backend_Server.Models.User;

namespace MyCode_Backend_Server_Tests.IntegrationTests
{
    [Collection("firstSequence")]
    public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory<MyCode_Backend_Server.Program>>
    {
        private readonly CustomWebApplicationFactory<MyCode_Backend_Server.Program> _factory;
        private readonly HttpClient _client;

        public UsersControllerTests(CustomWebApplicationFactory<MyCode_Backend_Server.Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Theory]
        [InlineData("/register")]
        public async Task Post_RegisterUserEndpoint_InvalidRequest_ReturnsNotFound(string url)
        {
            var invalidUserRegRequest = new
            {
                //Invalid request with missing required fields
            };

            var response = await _client.PostAsJsonAsync(url, invalidUserRegRequest);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("/login")]
        public async Task Post_LoginEndpoint_InvalidCredentials_ReturnsNotFound(string url)
        {
            var invalidAuthRequest = new
            {
                //Invalid credentials
            };

            var response = await _client.PostAsJsonAsync(url, invalidAuthRequest);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Get_UserEndpoint_AfterLogin_Returns_User()
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
            var response = await _client.GetAsync("/users/getUser");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var returnedUser = await response.Content.ReadFromJsonAsync<User>();
            Assert.NotNull(returnedUser);
        }

        [Fact]
        public async Task Post_RegisterUserEndpoint_AfterLogin_InvalidRequest_ReturnsBadRequest()
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
            var invalidUserRegRequest = new UserRegRequest("user13@example.com", "User13", "Password", "User 13", "123"); //already registered
            var response = await _client.PostAsJsonAsync("/users/register", invalidUserRegRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
