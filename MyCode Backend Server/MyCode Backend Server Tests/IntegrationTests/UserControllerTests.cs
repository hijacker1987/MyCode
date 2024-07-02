using Microsoft.EntityFrameworkCore;
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
            var authRequest = new AuthRequest("tester4@test.com", "Password", "Password");
            var returnedUser = await TestLogin.Login_With_Test_User_Return_User(authRequest, _client);

            // Act
            var response = await _client.GetAsync("/users/getUser");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(returnedUser);
            Assert.Equal("tester4@test.com", returnedUser!.Email);
            Assert.Equal("TESTER4@TEST.COM", returnedUser!.NormalizedEmail);
            Assert.Equal("Tester4", returnedUser!.UserName);
            Assert.Equal("TESTER4", returnedUser!.NormalizedUserName);
            Assert.Equal("123", returnedUser!.PhoneNumber);
        }

        [Fact]
        public async Task Get_Test_Service_Login_Returns_User()
        {
            // Arrange
            var authRequest = new AuthRequest("tester6@test.com", "Password", "Password");

            // Act
            User result = await TestLogin.Login_With_Test_User_Return_User(authRequest, _client);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("tester6@test.com", result!.Email);
            Assert.Equal("TESTER6@TEST.COM", result!.NormalizedEmail);
            Assert.Equal("Tester6", result!.UserName);
            Assert.Equal("TESTER6", result!.NormalizedUserName);
            Assert.Equal("123", result!.PhoneNumber);
        }

        [Fact]
        public async Task Post_RegisterUserEndpoint_AfterLogin_InvalidRequest_ReturnsBadRequest()
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
            var invalidUserRegRequest = new UserRegRequest("tester4@test.com", "Tester4", "Password", "User 13", "123"); //already registered
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
        public async Task Put_UpdateUserEndpoint_InvalidData_ReturnsUnauthorized()
        {
            // Arrange
            var invalidUserData = new User { /* invalid or missing data */ };

            // Act
            var response = await _client.PutAsJsonAsync("/users/userUpdate", invalidUserData);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Patch_ChangePasswordEndpoint_InvalidData_ReturnsUnauthorized()
        {
            // Arrange
            var invalidPasswordChangeRequest = new ChangePassRequest("", "cheese", "breeze", "breeze");

            // Act
            var response = await _client.PatchAsJsonAsync("/users/changePassword", invalidPasswordChangeRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Delete_DeleteAccountEndpoint_InvalidData_ReturnsUnauthorized()
        {
            // Arrange & Act
            var response = await _client.DeleteAsync("/users/userDelete");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Delete_DeleteAccountEndpoint_ReturnsOk()
        {
            // Pre Register
                // Arrange
                var registeredUser = new UserRegRequest("regtest2@test.com", "registeredthroughtest2", "Password", "Test Via Reg For Delete", "123456789");

                var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == "regtest2@test.com");
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
                Assert.Equal("regtest2@test.com", user.Email);

            // Deletion
            // Arrange
            var loginData = new AuthRequest("regtest2@test.com", "Password", "Password");

            User loggedInUser = await TestLogin.Login_With_Test_User_Return_User(loginData, _client);
            var userExists = await _dataContext.Users.AnyAsync(u => u.Id == loggedInUser.Id);
            Assert.True(userExists, "User does not exist in the database.");

            // Act
            var delResponse = await _client.DeleteAsync("/users/userDelete");

            // Assert
            Assert.Equal(HttpStatusCode.OK, delResponse.StatusCode);

            userExists = await _dataContext.Users.AnyAsync(u => u.Id == loggedInUser.Id);
            Assert.False(userExists, "User still exists in the database after deletion.");
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
            var user = await TestLogin.Login_With_Test_User_Return_User(authRequest, _client);
            var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            var updatedUser = new User { DisplayName = existingUser!.DisplayName,
                                         UserName = existingUser.UserName,
                                         NormalizedUserName = existingUser.UserName,
                                         Email = existingUser.Email,
                                         NormalizedEmail = existingUser.UserName,
                                         PhoneNumber = rdm.Next(100, 10000).ToString() };

            // Act
            var response = await _client.PutAsJsonAsync("/users/userUpdate", updatedUser);
            await _dataContext.SaveChangesAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Put_UpdateUserEndpoint_ValidData_WithSupportRole_ReturnsOk()
        {
            // Arrange
            Random rdm = new();

            var authRequest = new AuthRequest("support@test.com", "SupportPassword", "SupportPassword");
            var user = await TestLogin.Login_With_Test_User_Return_User(authRequest, _client);
            var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            var updatedUser = new User
            {
                DisplayName = existingUser!.DisplayName,
                UserName = existingUser.UserName,
                NormalizedUserName = existingUser.UserName,
                Email = existingUser.Email,
                NormalizedEmail = existingUser.UserName,
                PhoneNumber = rdm.Next(100, 10000).ToString()
            };

            // Act
            var response = await _client.PutAsJsonAsync("/users/userUpdate", updatedUser);
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
        public async Task Get_UserEndpoint_ExistingUser_ReturnsOK()
        {
            // Arrange
            var authRequest = new AuthRequest("tester7@test.com", "Password", "Password");
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
        }
        
        [Fact]
        public async Task Patch_ChangePasswordEndpoint_SuccessfulChange_ReturnsOk()
        {
            // Arrange
            var authRequest = new AuthRequest("newuser@example.com", "Password", "Password");
            var result = await TestLogin.Login_With_Test_User_Return_User(authRequest, _client);
            var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == result.Id);
            var newPasswordRequest = new ChangePassRequest(existingUser!.Email!, "Password", "NewPassword", "NewPassword");

            // Act
            var response = await _client.PatchAsJsonAsync("/users/changePassword", newPasswordRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var newPasswordReq = new ChangePassRequest(existingUser!.Email!, "NewPassword", "Password", "Password");
            await _client.PatchAsJsonAsync("/users/changePassword", newPasswordReq);
        }

        [Fact]
        public async Task Patch_ChangePasswordEndpoint_SuccessfulChange_ReturnsOk_WithSupportRole()
        {
            // Arrange
            var authRequest = new AuthRequest("support@test.com", "SupportPassword", "SupportPassword");
            var result = await TestLogin.Login_With_Test_User_Return_User(authRequest, _client);
            var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == result.Id);
            var newPasswordRequest = new ChangePassRequest(existingUser!.Email!, "SupportPassword", "NewPassword", "NewPassword");

            // Act
            var response = await _client.PatchAsJsonAsync("/users/changePassword", newPasswordRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var newPasswordReq = new ChangePassRequest(existingUser!.Email!, "NewPassword", "SupportPassword", "SupportPassword");
            await _client.PatchAsJsonAsync("/users/changePassword", newPasswordReq);
        }

        [Fact]
        public async Task Patch_ChangePasswordEndpoint_SuccessfulChange_ReturnsOk_WithAdminRole()
        {
            // Arrange
            var authRequest = new AuthRequest("admin@test.com", "AdminPassword", "AdminPassword");
            var result = await TestLogin.Login_With_Test_User_Return_User(authRequest, _client);
            var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == result.Id);
            var newPasswordRequest = new ChangePassRequest(existingUser!.Email!, "AdminPassword", "NewPassword", "NewPassword");

            // Act
            var response = await _client.PatchAsJsonAsync("/users/changePassword", newPasswordRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var newPasswordReq = new ChangePassRequest(existingUser!.Email!, "NewPassword", "AdminPassword", "AdminPassword");
            await _client.PatchAsJsonAsync("/users/changePassword", newPasswordReq);
        }

        [Fact]
        public async Task Post_RegisterUserEndpoint_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var invalidUserRegRequest = new UserRegRequest("invalidemail", "", "", "", "");

            // Act
            var response = await _client.PostAsJsonAsync("/users/register", invalidUserRegRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("The Email field is not a valid e-mail address.", responseContent);
        }

        [Fact]
        public async Task Post_RegisterUserEndpoint_InvalidEmailFormat_ReturnsBadRequest()
        {
            var invalidEmailRequest = new UserRegRequest("invalid-email", "username", "Password", "Display Name", "123456789");

            var response = await _client.PostAsJsonAsync("/users/register", invalidEmailRequest);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Post_RegisterUserEndpoint_ShortPassword_ReturnsBadRequest()
        {
            var weakPasswordRequest = new UserRegRequest("user@example.com", "PassW", "123", "Display Name", "123456789");

            var response = await _client.PostAsJsonAsync("/users/register", weakPasswordRequest);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get_UserEndpoint_Unauthorized_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/users/getUser");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Put_UpdateUserEndpoint_Unauthorized_ReturnsUnauthorized()
        {
            var updateUserRequest = new User();

            var response = await _client.PutAsJsonAsync("/users/userUpdate", updateUserRequest);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Patch_ChangePasswordEndpoint_IncorrectCurrentPassword_ReturnsBadRequest()
        {
            var authRequest = new AuthRequest("tester1@test.com", "Password", "Password");
            await TestLogin.Login_With_Test_User_Return_User(authRequest, _client);

            var changePasswordRequest = new ChangePassRequest("tester1@test.com", "IncorrectPassword", "NewPassword", "NewPassword");

            var response = await _client.PatchAsJsonAsync("/users/changePassword", changePasswordRequest);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_DeleteAccountEndpoint_AdminUser_ReturnsForbidden()
        {
            var authRequest = new AuthRequest("admin@test.com", "AdminPassword", "AdminPassword");
            await TestLogin.Login_With_Test_User_Return_User(authRequest, _client);

            var response = await _client.DeleteAsync("/users/userDelete");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Post_LoginEndpoint_EmptyPassword_ReturnsBadRequest()
        {
            var loginRequest = new AuthRequest("user@example.com", "", "");

            var response = await _client.PostAsJsonAsync("/users/login", loginRequest);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Post_LoginEndpoint_MissingConfirmPassword_ReturnsBadRequest()
        {
            var loginRequest = new AuthRequest("user@example.com", "Password", "");

            var response = await _client.PostAsJsonAsync("/users/login", loginRequest);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get_UserIdEndpoint_ValidUser_ReturnsUserId()
        {
            var authRequest = new AuthRequest("tester2@test.com", "Password", "Password");
            await TestLogin.Login_With_Test_User_Return_User(authRequest, _client);

            var response = await _client.GetAsync("/users/getUserId");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var userId = await response.Content.ReadAsStringAsync();
            Assert.NotNull(userId);
        }

        [Fact]
        public async Task Get_UserIdEndpoint_ValidSupportUser_ReturnsUserId()
        {
            var authRequest = new AuthRequest("support@test.com", "SupportPassword", "SupportPassword");
            await TestLogin.Login_With_Test_User_Return_User(authRequest, _client);

            var response = await _client.GetAsync("/users/getUserId");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var userId = await response.Content.ReadAsStringAsync();
            Assert.NotNull(userId);
        }

        [Fact]
        public async Task Get_UserIdEndpoint_ValidAdminUser_ReturnsUserId()
        {
            var authRequest = new AuthRequest("admin@test.com", "AdminPassword", "AdminPassword");
            await TestLogin.Login_With_Test_User_Return_User(authRequest, _client);

            var response = await _client.GetAsync("/users/getUserId");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var userId = await response.Content.ReadAsStringAsync();
            Assert.NotNull(userId);
        }

        [Fact]
        public async Task Put_UpdateUserEndpoint_Admin_ReturnsMethodNotAllowed()
        {
            var authRequest = new AuthRequest("admin@test.com", "AdminPassword", "AdminPassword");
            await TestLogin.Login_With_Test_User_Return_User(authRequest, _client);
            var loginRequest = new AuthRequest("user@example.com", "", "");

            var response = await _client.PostAsJsonAsync($"/users/userUpdate", loginRequest);

            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact]
        public async Task Put_UpdateUserEndpoint_Admin_ReturnsForbidden()
        {
            var authRequest = new AuthRequest("admin@test.com", "AdminPassword", "AdminPassword");
            await TestLogin.Login_With_Test_User_Return_User(authRequest, _client);
            var loginRequest = new AuthRequest("user@example.com", "", "");

            var response = await _client.PutAsJsonAsync("/users/userUpdate", loginRequest);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Post_RegisterUserEndpoint_ValidRequest_ReturnsOk()
        {
            var validUserRegRequest = new UserRegRequest("newuser@test.com", "newusername", "Password", "Test User", "123456789");

            var exists = _dataContext.Users.FirstOrDefault(u => u.Email ==  validUserRegRequest.Email);

            var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == "newuser@test.com");
            if (existingUser != null)
            {
                _dataContext.Users.Remove(existingUser);
                await _dataContext.SaveChangesAsync();
            }

            var response = await _client.PostAsJsonAsync("/users/register", validUserRegRequest);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var user = await response.Content.ReadFromJsonAsync<UserRegResponse>();
            Assert.NotNull(user);
            Assert.Equal("newuser@test.com", user.Email);
        }

        [Fact]
        public async Task Patch_ChangePasswordEndpoint_ValidData_ReturnsOk()
        {
            var authRequest = new AuthRequest("tester8@test.com", "Password", "Password");
            var (authToken, cookies, _) = await TestLogin.Login_With_Test_User(authRequest, _client);

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            var validPasswordChangeRequest = new ChangePassRequest("tester8@test.com", "Password", "NewPassword", "NewPassword");

            var response = await _client.PatchAsJsonAsync("/users/changePassword", validPasswordChangeRequest);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var validPasswordChangeRequestBackRoll = new ChangePassRequest("tester8@test.com", "NewPassword", "Password", "Password");

            var responseBackRoll = await _client.PatchAsJsonAsync("/users/changePassword", validPasswordChangeRequestBackRoll);
            Assert.Equal(HttpStatusCode.OK, responseBackRoll.StatusCode);
        }

        [Fact]
        public async Task Post_RegisterUserEndpoint_DuplicateEmail_ReturnsBadRequest()
        {
            var duplicateUserRegRequest = new UserRegRequest("tester5@test.com", "duplicateuser", "Password", "Duplicate User", "123456789");

            var response = await _client.PostAsJsonAsync("/users/register", duplicateUserRegRequest);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Post_LogoutEndpoint_ReturnsOk()
        {
            var authRequest = new AuthRequest("tester6@test.com", "Password", "Password");
            var (authToken, cookies, _) = await TestLogin.Login_With_Test_User(authRequest, _client);

            _client.DefaultRequestHeaders.Add("Authorization", authToken);
            foreach (var cookie in cookies)
            {
                _client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            var response = await _client.DeleteAsync("/token/revoke");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Get_UserEndpoint_InvalidToken_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Add("Authorization", "InvalidToken");

            var response = await _client.GetAsync("/users/getUser");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Post_LoginEndpoint_WrongPassword_ReturnsNotFound()
        {
            var wrongPasswordAuthRequest = new AuthRequest("tester10@test.com", "WrongPassword", "WrongPassword");

            var response = await _client.PostAsJsonAsync("/users/login", wrongPasswordAuthRequest);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
