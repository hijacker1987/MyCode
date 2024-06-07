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
            // Act
            var response = await _client.GetAsync("/codes/by-user");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Get_GetAllCodesByVisibilityEndpoint_Unauthorized()
        {
            // Act
            var response = await _client.GetAsync("/codes/by-visibility");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Get_GetCodeByIdEndpoint_Unauthorized()
        {
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
            var authRequest = new AuthRequest("tester5@test.com", "Password", "Password");
            await TestLogin.Login_With_Test_User_Return_User(authRequest, _client);

            var codeRequest = new CodeRegRequest("Sample Code", "console.log('Hello, World!');", "JavaScript", false, true);

            var registerResponse = await _client.PostAsJsonAsync("/codes/register", codeRequest);
            var codeResponse = await registerResponse.Content.ReadFromJsonAsync<CodeRegResponse>();
            var codeId = codeResponse?.Id;

            using (var scope = _factory.Services.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                var existingCode = dataContext.CodesDb!.FirstOrDefault(c => c.Id.ToString() == codeId);

                if (existingCode != null)
                {
                    dataContext.CodesDb!.Remove(existingCode);
                    await dataContext.SaveChangesAsync();
                }
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
        public async Task Put_UpdateCodeEndpoint_InvalidRequest_ReturnsUnauthorized()
        {
            // Arrange
            var authRequest = new AuthRequest("tester1@test.com", "Password", "Password");
            var user = await TestLogin.Login_With_Test_User_Return_User(authRequest, _factory.CreateClient());

            var invalidUpdatedCode = new CodeRegRequest("", "", "", false, true);

            var code = _dataContext.CodesDb!.FirstOrDefault(c => c.UserId == user.Id);

            // Act
            var response = await _client.PutAsJsonAsync($"/codes/cupdate-{code!.Id}", invalidUpdatedCode);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
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
            var authRequest = new AuthRequest("tester5@test.com", "Password", "Password");
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
            var authRequest = new AuthRequest("tester5@test.com", "Password", "Password");
            var user = await TestLogin.Login_With_Test_User_Return_User(authRequest, _client);
            Assert.NotNull(user);

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

            // Clean up
            using (var scope = _factory.Services.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                var existingCode = dataContext.CodesDb!.FirstOrDefault(c => c.Id.ToString() == codeId);

                if (existingCode != null)
                {
                    dataContext.CodesDb!.Remove(existingCode);
                    await dataContext.SaveChangesAsync();
                }
            }
        }

        [Fact]
        public async Task Get_GetCodeByIdEndpoint_NonexistentId_ReturnsNotFound()
        {
            // Arrange
            var authRequest = new AuthRequest("tester5@test.com", "Password", "Password");
            var user = await TestLogin.Login_With_Test_User_Return_User(authRequest, _client);
            Assert.NotNull(user);

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
            var authRequest = new AuthRequest("tester5@test.com", "Password", "Password");
            var user = await TestLogin.Login_With_Test_User_Return_User(authRequest, _client);
            Assert.NotNull(user);
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

            using (var scope = _factory.Services.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                var existingCode = dataContext.CodesDb!.FirstOrDefault(c => c.Id.ToString() == codeId);

                if (existingCode != null)
                {
                    dataContext.CodesDb!.Remove(existingCode);
                    await dataContext.SaveChangesAsync();
                }
            }
        }

        [Fact]
        public async Task Put_UpdateCodeEndpoint_Authorized_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var authRequest = new AuthRequest("tester5@test.com", "Password", "Password");
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
        public async Task Put_UpdateCodeEndpoint_Unauthorized_Returns_Unauthorized()
        {
            // Arrange
            var invalidUpdatedCodeRequest = new CodeRegRequest("Updated Code", "console.log('Updated Hello, World!');", "JavaScript", true, false);

            // Act
            var response = await _client.PutAsJsonAsync("/codes/cupdate-some-id", invalidUpdatedCodeRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Delete_DeleteCodeByUserEndpoint_NonexistentId_ReturnsUnauthorized()
        {
            // Arrange
            var nonexistentId = Guid.NewGuid(); // Generate a random non-existent ID

            // Act
            var response = await _client.DeleteAsync($"/codes/cdelete-{nonexistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Delete_DeleteCodeByUserEndpoint_Unauthorized_Unauthorized()
        {
            // Arrange
            var nonexistentId = Guid.NewGuid(); // Generate a random non-existent ID

            // Act
            var response = await _client.DeleteAsync($"/codes/cdelete-{nonexistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
