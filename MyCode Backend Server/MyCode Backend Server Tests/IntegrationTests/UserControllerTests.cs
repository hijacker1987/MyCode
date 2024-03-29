﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Contracts.Services;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server_Tests.Services;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Assert = Xunit.Assert;
using User = MyCode_Backend_Server.Models.User;

namespace MyCode_Backend_Server_Tests.IntegrationTests
{
    [Collection("firstSequence")]
    public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory<MyCode_Backend_Server.Program>>
    {
        private readonly CustomWebApplicationFactory<MyCode_Backend_Server.Program> _factory;
        private readonly HttpClient _client;
        private readonly DataContext _dataContext;

        public UsersControllerTests(CustomWebApplicationFactory<MyCode_Backend_Server.Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _dataContext = _factory.Services.GetRequiredService<DataContext>();
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
            var authRequest = new AuthRequest("tester9@test.com", "Password", "Password");
            var (authToken, cookies, result) = await TestLogin.Login_With_Test_User(authRequest, _client);

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            // Act
            var response = await _client.GetAsync("/users/getUser");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Post_RegisterUserEndpoint_AfterLogin_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var authRequest = new AuthRequest("tester9@test.com", "Password", "Password");
            var (authToken, cookies, _) = await TestLogin.Login_With_Test_User(authRequest, _client);

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            // Act
            var invalidUserRegRequest = new UserRegRequest("tester9@test.com", "Tester9", "Password", "User 13", "123"); //already registered
            var response = await _client.PostAsJsonAsync("/users/register", invalidUserRegRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Post_RegistrationEndpoint_SuccessfulRegistration()
        {
            // Arrange
            var registeredUser = new UserRegRequest("regtest@test.com", "registeredthroughtest", "Password", "Test Via Reg", "123456789");

            var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == "regtest@test.com");
            if (existingUser != null)
            {
                _dataContext.Users.Remove(existingUser);
                await _dataContext.SaveChangesAsync();
            }

            // Act
            var response = await _client.PostAsJsonAsync("/users/register", registeredUser);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var user = await response.Content.ReadFromJsonAsync<UserRegResponse>();
            Assert.NotNull(user);
            Assert.Equal("regtest@test.com", user.Email);
        }

        [Fact]
        public async Task Post_LoginEndpoint_SuccessfulLogin_Returns200StatusCode()
        {
            // Arrange
            var loginData = new AuthRequest("regtest@test.com", "Password", "Password");

            // Act
            var response = await _client.PostAsJsonAsync("/users/login", loginData);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            Assert.NotNull(authResponse);
            Assert.Equal("User", authResponse.Role);
        }

        [Fact]
        public async Task Get_UserEndpoint_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/users/getUser");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Put_UpdateUserEndpoint_InvalidData_ReturnsUnauth()
        {
            // Arrange
            var invalidUserData = new User { /* invalid or missing data */ };

            // Act
            var response = await _client.PutAsJsonAsync($"/users/user-{Guid.NewGuid()}", invalidUserData);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Patch_ChangePasswordEndpoint_InvalidData_ReturnsUnauth()
        {
            // Arrange
            var invalidPasswordChangeRequest = new ChangePassRequest("", "cheese", "breeze", "breeze");

            // Act
            var response = await _client.PatchAsJsonAsync("/users/changePassword", invalidPasswordChangeRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Delete_DeleteAccountEndpoint_InvalidData_ReturnsUnauth()
        {
            // Arrange
            Guid invalidUserId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"/users/delete-{invalidUserId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Get_UserEndpoint_WithoutSufficientPermissions_ReturnsUnauth()
        {
            // Arrange - assuming user without necessary role

            // Act
            var response = await _client.GetAsync("/users/getUser");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Put_UpdateUserEndpoint_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var authRequest = new AuthRequest("tester9@test.com", "Password", "Password");
            var (authToken, cookies, _) = await TestLogin.Login_With_Test_User(authRequest, _client);

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            var invalidUserData = new User { /* invalid or missing data */ };

            // Act
            var response = await _client.PutAsJsonAsync("/users/user-<valid_user_id>", invalidUserData);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Patch_ChangePasswordEndpoint_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var authRequest = new AuthRequest("regtest@test.com", "Password", "Password");
            var (authToken, cookies, _) = await TestLogin.Login_With_Test_User(authRequest, _client);

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            var invalidPasswordChangeRequest = new ChangePassRequest("", "", "", "");

            // Act
            var response = await _client.PatchAsJsonAsync("/users/changePassword", invalidPasswordChangeRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_DeleteAccountEndpoint_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var authRequest = new AuthRequest("regtest@test.com", "Password", "Password");
            var (authToken, cookies, _) = await TestLogin.Login_With_Test_User(authRequest, _client);

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            Guid invalidUserId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"/users/delete-{invalidUserId}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get_UserEndpoint_NonExistingUser_ReturnsNotFound()
        {
            // Arrange
            Guid nonExistingUserId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/users/getUser/{nonExistingUserId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Put_UpdateUserEndpoint_ValidData_ReturnsOk()
        {
            // Arrange
            Random rdm = new();

            var authRequest = new AuthRequest("tester8@test.com", "Password", "Password");
            var (authToken, cookies, result) = await TestLogin.Login_With_Test_User(authRequest, _client);
            var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == result.Id);
            var updatedUser = new User { DisplayName = existingUser!.DisplayName,
                                         UserName = existingUser.UserName,
                                         NormalizedUserName = existingUser.UserName,
                                         Email = existingUser.Email,
                                         NormalizedEmail = existingUser.UserName,
                                         PhoneNumber = rdm.Next(100, 10000).ToString() };

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            // Act
            var response = await _client.PutAsJsonAsync($"/users/user-{result.Id}", updatedUser);
            await _dataContext.SaveChangesAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_LoginEndpoint_InvalidEmail_ReturnsNotFound()
        {
            // Arrange
            var invalidAuthRequest = new AuthRequest("invalidemail@test.com", "Password", "Password");

            // Act
            var response = await _client.PostAsJsonAsync("/users/login", invalidAuthRequest);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Post_RegisterUserEndpoint_ExistingEmail_ReturnsBadRequest()
        {
            // Arrange
            var existingUserEmail = "tester9@test.com";
            var existingUserRegRequest = new UserRegRequest(existingUserEmail, "ExistingTester", "Password", "Existing User", "123456789");

            // Act
            var response = await _client.PostAsJsonAsync("/users/register", existingUserRegRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get_UserEndpoint_ExistingUser_ReturnsUser()
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
            var response = await _client.GetAsync("/users/getUser");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var user = await response.Content.ReadFromJsonAsync<UserRegResponse>();
            Assert.NotNull(user);
        }

        [Fact]
        public async Task Post_RegisterUserEndpoint_SuccessfulRegistration_ReturnsOk()
        {
            // Arrange
            var newUser = new UserRegRequest("newuser@example.com", "NewUser", "Password", "New User", "123456789");

            var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == "newuser@example.com");
            if (existingUser != null)
            {
                _dataContext.Users.Remove(existingUser);
                await _dataContext.SaveChangesAsync();
            }

            // Act
            var response = await _client.PostAsJsonAsync("/users/register", newUser);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var user = await response.Content.ReadFromJsonAsync<UserRegResponse>();
            Assert.NotNull(user);
            Assert.Equal("newuser@example.com", user.Email);
        }

        [Fact]
        public async Task Post_LoginEndpoint_SuccessfulLogin_ReturnsOk()
        {
            // Arrange
            var loginData = new AuthRequest("newuser@example.com", "Password", "Password");

            // Act
            var response = await _client.PostAsJsonAsync("/users/login", loginData);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            Assert.NotNull(authResponse);
            Assert.Equal("User", authResponse.Role);
        }

        [Fact]
        public async Task Patch_ChangePasswordEndpoint_SuccessfulChange_ReturnsOk()
        {
            // Arrange
            var authRequest = new AuthRequest("newuser@example.com", "Password", "Password");
            var (authToken, cookies, result) = await TestLogin.Login_With_Test_User(authRequest, _client);
            var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == result.Id);
            var newPasswordRequest = new ChangePassRequest(existingUser!.Email!, "Password", "NewPassword", "NewPassword");

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            // Act
            var response = await _client.PatchAsJsonAsync("/users/changePassword", newPasswordRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var newPasswordReq = new ChangePassRequest(existingUser!.Email!, "NewPassword", "Password", "Password");
            await _client.PatchAsJsonAsync("/users/changePassword", newPasswordReq);
        }
    }
}
