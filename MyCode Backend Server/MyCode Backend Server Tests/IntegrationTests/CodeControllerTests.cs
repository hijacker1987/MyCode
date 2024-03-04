using Microsoft.Extensions.DependencyInjection;
using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Contracts.Services;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
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
        private readonly DataContext _dataContext;

        public CodeControllerTests(CustomWebApplicationFactory<MyCode_Backend_Server.Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _dataContext = _factory.Services.GetRequiredService<DataContext>();
        }

        [Fact]
        public async Task Get_GetAllCodesByUserEndpoint_Unauthorized()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/codes/by-user");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Get_GetAllCodesByVisibilityEndpoint_Unauthorized()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/codes/by-visibility");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Get_GetCodeByIdEndpoint_Unauthorized()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/codes/code-some-id");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Post_CreateCodeEndpoint_Unauthorized()
        {
            // Arrange
            var codeRequest = new CodeRegRequest("", "", "", false, false);

            // Act
            var response = await _client.PostAsJsonAsync("/codes/register", codeRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Put_UpdateCodeEndpoint_Unauthorized()
        {
            // Arrange
            var updatedCode = new CodeRegRequest("", "", "", false, false);

            // Act
            var response = await _client.PutAsJsonAsync("/codes/cupdate-some-id", updatedCode);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Delete_DeleteCodeByUserEndpoint_Unauthorized()
        {
            // Arrange

            // Act
            var response = await _client.DeleteAsync("/codes/cdelete-some-id");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Get_GetAllCodesByUserEndpoint_Authorized()
        {
            // Arrange
            var authRequest = new AuthRequest("tester4@test.com", "Password", "Password");
            var (authToken, cookies, _) = await TestLogin.Login_With_Test_User(authRequest, _client);

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
            var authRequest = new AuthRequest("tester7@test.com", "Password", "Password");
            var (authToken, cookies, _) = await TestLogin.Login_With_Test_User(authRequest, _client);

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }
            var codeRequest = new CodeRegRequest("Sample Code", "console.log('Hello, World!');", "JavaScript", false, true);

            var registerResponse = await _client.PostAsJsonAsync("/codes/register", codeRequest);
            var codeResponse = await registerResponse.Content.ReadFromJsonAsync<CodeRegResponse>();
            var codeId = codeResponse?.Id;

            var existingCode = _dataContext.CodesDb!.FirstOrDefault(c => c.Id.ToString() == codeId);

            if (existingCode != null)
            {
                _dataContext.CodesDb!.Remove(existingCode);
                await _dataContext.SaveChangesAsync();
            }

            // Act
            var response = await _client.PostAsJsonAsync("/codes/register", codeRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Post_CreateCodeEndpoint_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var authRequest = new AuthRequest("tester2@test.com", "Password", "Password");
            var (authToken, cookies, _) = await TestLogin.Login_With_Test_User(authRequest, _client);

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

        [Fact]
        public async Task Put_UpdateCodeEndpoint_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var authRequest = new AuthRequest("tester9@test.com", "Password", "Password");
            var (authToken, cookies, _) = await TestLogin.Login_With_Test_User(authRequest, _factory.CreateClient());

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            var invalidUpdatedCode = new CodeRegRequest("", "", "", false, true);

            // Act
            var response = await _client.PutAsJsonAsync("/codes/cupdate-valid_code_id", invalidUpdatedCode);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_DeleteCodeByUserEndpoint_ValidRequest_ReturnsOk()
        {
            // Arrange
            var authRequest = new AuthRequest("tester10@test.com", "Password", "Password");
            var (authToken, cookies, _) = await TestLogin.Login_With_Test_User(authRequest, _factory.CreateClient());

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            var codeRequest = new CodeRegRequest("Sample Code", "console.log('Hello, World!');", "JavaScript", false, true);
            var registerResponse = await _client.PostAsJsonAsync("/codes/register", codeRequest);
            var codeId = (await registerResponse.Content.ReadFromJsonAsync<CodeRegResponse>())?.Id;

            // Assert
            Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
            Assert.NotNull(codeId);

            var getCodeResponse = await _client.GetAsync($"/codes/code-{codeId}");
            var code = await getCodeResponse.Content.ReadFromJsonAsync<Code>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, getCodeResponse.StatusCode);
            Assert.NotNull(code);

            // Act
            var deleteResponse = await _client.DeleteAsync($"/codes/cdelete-{codeId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task Get_GetAllCodesByVisibilityEndpoint_Authorized()
        {
            // Arrange
            var authRequest = new AuthRequest("tester3@test.com", "Password", "Password");
            var (authToken, cookies, _) = await TestLogin.Login_With_Test_User(authRequest, _client);

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            // Act
            var response = await _client.GetAsync("/codes/by-visibility");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Get_GetCodeByIdEndpoint_BadRequest()
        {
            // Arrange
            var authRequest = new AuthRequest("tester1@test.com", "Password", "Password");
            var (authToken, cookies, _) = await TestLogin.Login_With_Test_User(authRequest, _client);

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            // Act
            var response = await _client.GetAsync("/codes/code-nonexistent-id");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get_GetCodeByIdEndpoint_Authorized_ReturnsOk()
        {
            // Arrange
            var authRequest = new AuthRequest("tester3@test.com", "Password", "Password");
            var (authToken, cookies, _) = await TestLogin.Login_With_Test_User(authRequest, _client);

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            var codeRequest = new CodeRegRequest("Sample Code", "console.log('Hello, World!');", "JavaScript", false, true);
            var registerResponse = await _client.PostAsJsonAsync("/codes/register", codeRequest);
            var codeId = (await registerResponse.Content.ReadFromJsonAsync<CodeRegResponse>())?.Id;

            // Assert
            Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
            Assert.NotNull(codeId);

            // Act
            var response = await _client.GetAsync($"/codes/code-{codeId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var existingCode = _dataContext.CodesDb!.FirstOrDefault(c => c.Id.ToString() == codeId);

            if (existingCode != null)
            {
                _dataContext.CodesDb!.Remove(existingCode);
                await _dataContext.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task Get_GetCodeByIdEndpoint_NonexistentId_ReturnsNotFound()
        {
            // Arrange
            var authRequest = new AuthRequest("tester3@test.com", "Password", "Password");
            var (authToken, cookies, _) = await TestLogin.Login_With_Test_User(authRequest, _client);

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            var nonexistentId = Guid.NewGuid(); // Generate a random non-existent ID

            // Act
            var response = await _client.GetAsync($"/codes/code-{nonexistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Put_UpdateCodeEndpoint_Authorized_ValidRequest_ReturnsOk()
        {
            // Arrange
            var authRequest = new AuthRequest("tester3@test.com", "Password", "Password");
            var (authToken, cookies, _) = await TestLogin.Login_With_Test_User(authRequest, _client);

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            var codeRequest = new CodeRegRequest("Sample Code", "console.log('Hello, World!');", "JavaScript", false, true);
            var registerResponse = await _client.PostAsJsonAsync("/codes/register", codeRequest);
            var codeId = (await registerResponse.Content.ReadFromJsonAsync<CodeRegResponse>())?.Id;

            // Assert
            Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
            Assert.NotNull(codeId);

            var updatedCodeRequest = new CodeRegRequest("Updated Code", "console.log('Updated Hello, World!');", "JavaScript", true, false);

            // Act
            var response = await _client.PutAsJsonAsync($"/codes/cupdate-{codeId}", updatedCodeRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var existingCode = _dataContext.CodesDb!.FirstOrDefault(c => c.Id.ToString() == codeId);

            if (existingCode != null)
            {
                _dataContext.CodesDb!.Remove(existingCode);
                await _dataContext.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task Put_UpdateCodeEndpoint_Authorized_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var authRequest = new AuthRequest("tester3@test.com", "Password", "Password");
            var (authToken, cookies, _) = await TestLogin.Login_With_Test_User(authRequest, _client);

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            var codeRequest = new CodeRegRequest("Sample Code", "console.log('Hello, World!');", "JavaScript", false, true);
            var registerResponse = await _client.PostAsJsonAsync("/codes/register", codeRequest);
            var codeId = (await registerResponse.Content.ReadFromJsonAsync<CodeRegResponse>())?.Id;

            // Assert
            Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
            Assert.NotNull(codeId);

            var invalidUpdatedCodeRequest = new CodeRegRequest("", "", "", false, true); // Invalid request

            // Act
            var response = await _client.PutAsJsonAsync($"/codes/cupdate-{codeId}", invalidUpdatedCodeRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Put_UpdateCodeEndpoint_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var invalidUpdatedCodeRequest = new CodeRegRequest("Updated Code", "console.log('Updated Hello, World!');", "JavaScript", true, false);

            // Act
            var response = await client.PutAsJsonAsync("/codes/cupdate-some-id", invalidUpdatedCodeRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Delete_DeleteCodeByUserEndpoint_NonexistentId_ReturnsNotFound()
        {
            // Arrange
            var authRequest = new AuthRequest("tester3@test.com", "Password", "Password");
            var (authToken, cookies, _) = await TestLogin.Login_With_Test_User(authRequest, _client);

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            var nonexistentId = Guid.NewGuid(); // Generate a random non-existent ID

            // Act
            var response = await _client.DeleteAsync($"/codes/cdelete-{nonexistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_DeleteCodeByUserEndpoint_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var nonexistentId = Guid.NewGuid(); // Generate a random non-existent ID

            // Act
            var response = await client.DeleteAsync($"/codes/cdelete-{nonexistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
