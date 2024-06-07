using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Assert = Xunit.Assert;
using User = MyCode_Backend_Server.Models.User;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server_Tests.Services;
using MyCode_Backend_Server.Contracts.Services;
using MyCode_Backend_Server.Models.MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace MyCode_Backend_Server_Tests.IntegrationTests
{
    [Collection("firstSequence")]
    public class AdminControllerTests : IClassFixture<CustomWebApplicationFactory<MyCode_Backend_Server.Program>>
    {
        private readonly CustomWebApplicationFactory<MyCode_Backend_Server.Program> _factory;
        private readonly DataContext _dataContext;

        public AdminControllerTests(CustomWebApplicationFactory<MyCode_Backend_Server.Program> factory)
        {
            _factory = factory;
            _dataContext = _factory.Services.GetRequiredService<DataContext>();
        }

        [Theory]
        [InlineData("/getUsers")]
        public async Task Get_GetUsersEndpoint_NoAuth_ReturnsError(string url)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync(url);

            Assert.True(response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized or HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_GetCodesEndpoint_NoAuth_ReturnsError()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync("/admin/getCodes");

            Assert.True(response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized or HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Put_UpdateUserEndpoint_NoAuth_ReturnsError()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.PutAsJsonAsync("/admin/au-123", new User());

            Assert.True(response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized or HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Put_UpdateCodeEndpoint_NoAuth_ReturnsError()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.PutAsJsonAsync("/admin/acu-123", new Code());

            Assert.True(response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized or HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_DeleteCodeEndpoint_NoAuth_ReturnsError()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.DeleteAsync("/admin/ad-123");

            Assert.True(response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized or HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_GetUserByIdEndpoint_NoAuth_ReturnsError()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync("/admin/user-by-123");
            
            Assert.True(response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized or HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_GetCodeByIdEndpoint_NoAuth_ReturnsError()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync("/admin/code-by-123");

            Assert.True(response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized or HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_GetUsersEndpoint_Unauthorized_ReturnsError()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            // Act
            var response = await client.GetAsync("/admin/getUsers");

            // Assert
            Assert.True(response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized or HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData("/admin/getUsers")]
        [InlineData("/admin/getCodes")]
        public async Task Get_Endpoints_Without_Authentication_Returns_Unauthorized(string url)
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Put_UpdateUserEndpoint_NoAuth_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var userId = Guid.NewGuid();
            var updatedUser = new User();

            // Act
            var response = await client.PutAsJsonAsync($"/admin/aupdate-{userId}", updatedUser);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Put_UpdateCodeEndpoint_NoAuth_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var codeId = Guid.NewGuid();
            var updatedCode = new Code();

            // Act
            var response = await client.PutAsJsonAsync($"/admin/acupdate-{codeId}", updatedCode);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Delete_DeleteUserEndpoint_NoAuth_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var userId = Guid.NewGuid();

            // Act
            var response = await client.DeleteAsync($"/admin/aduser-{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Delete_DeleteCodeEndpoint_NoAuth_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var codeId = Guid.NewGuid();

            // Act
            var response = await client.DeleteAsync($"/admin/adcode-{codeId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Get_GetUserByIdEndpoint_NoAuth_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var userId = Guid.NewGuid();

            // Act
            var response = await client.GetAsync($"/admin/user-by-{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Get_GetCodeByIdEndpoint_NoAuth_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var codeId = Guid.NewGuid();

            // Act
            var response = await client.GetAsync($"/admin/code-by-{codeId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllUsersAsync_Returns_OK_With_Users_When_Authenticated_As_Admin()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var authRequest = new AuthRequest("admin@test.com", "AdminPassword", "AdminPassword");
            await TestLogin.Login_With_Test_User_Return_User(authRequest, client);

            // Act
            var response = await client.GetAsync("/admin/getUsers");

            // Assert
            response.EnsureSuccessStatusCode();
            var users = await response.Content.ReadFromJsonAsync<List<UserWithRole>>();
            Assert.NotNull(users);
            Assert.NotEmpty(users);
        }

        [Fact]
        public async Task GetAllUsersAsync_Returns_Forbidden_When_Authenticated_As_Non_Admin_User_UserRole()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var authRequest = new AuthRequest("tester1@test.com", "Password", "Password");
            await TestLogin.Login_With_Test_User_Return_User(authRequest, client);

            // Act
            var response = await client.GetAsync("/admin/getUsers");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetAllUsersAsync_Returns_Ok_When_Authenticated_As_Non_Admin_User_SupportRole()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var authRequest = new AuthRequest("support@test.com", "SupportPassword", "SupportPassword");
            await TestLogin.Login_With_Test_User_Return_User(authRequest, client);

            // Act
            var response = await client.GetAsync("/admin/getUsers");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAllUsersAsync_Returns_Unauthorized_When_Not_Authenticated()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            // Act
            var response = await client.GetAsync("/admin/getUsers");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_Returns_NotFound_When_User_Deleted_Successfully_As_Admin()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var authRequest = new AuthRequest("admin@test.com", "AdminPassword", "AdminPassword");
            await TestLogin.Login_With_Test_User_Return_User(authRequest, client);

            var userId = Guid.NewGuid();

            // Act
            var response = await client.DeleteAsync($"/admin/aduser-{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_Returns_Forbidden_When_Authenticated_As_Non_Admin_User()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var authRequest = new AuthRequest("tester7@test.com", "Password", "Password");
            await TestLogin.Login_With_Test_User_Return_User(authRequest, client);

            var userId = Guid.NewGuid();

            // Act
            var response = await client.DeleteAsync($"/admin/aduser-{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_Returns_Unauthorized_When_Not_Authenticated()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var userId = Guid.NewGuid();

            // Act
            var response = await client.DeleteAsync($"/admin/aduser-{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_Returns_Ok_When_User_Updated_Successfully_As_Admin()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var authRequest = new AuthRequest("admin@test.com", "AdminPassword", "AdminPassword");
            var admin = await TestLogin.Login_With_Test_User_Return_User(authRequest, client);
            
            // Act
            var user = _dataContext.Users.FirstOrDefault(u => u.Id != admin.Id);
            var updatedUser = new User()
            {
                DisplayName = user!.DisplayName,
                UserName = "Test",
                Email = user!.Email,
                PhoneNumber = user!.PhoneNumber,
            };

            var response = await client.PutAsJsonAsync($"/admin/aupdate-{user!.Id}", updatedUser);

            // Assert
            response.EnsureSuccessStatusCode();
            var updatedUserResponse = await response.Content.ReadFromJsonAsync<User>();
            Assert.NotNull(updatedUserResponse);
        }

        [Fact]
        public async Task UpdateUser_Returns_NotFound_When_Invalid_Data_Provided_As_Admin()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var authRequest = new AuthRequest("admin@test.com", "AdminPassword", "AdminPassword");
            await TestLogin.Login_With_Test_User_Return_User(authRequest, client);

            var userId = Guid.NewGuid();
            var updatedUser = new User()
            {
                Email = null
            };

            // Act
            var response = await client.PutAsJsonAsync($"/admin/aupdate-{userId}", updatedUser);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_Returns_BadRequest_When_Invalid_Data_Provided_As_Admin()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var authRequest = new AuthRequest("admin@test.com", "AdminPassword", "AdminPassword");
            var admin = await TestLogin.Login_With_Test_User_Return_User(authRequest, client);

            // Act
            var user = _dataContext.Users.FirstOrDefault(u => u.Id != admin.Id);
            var updatedUser = new User()
            {
                DisplayName = "Support User",
                UserName = "support user",
                Email = "supportuser@mycodetest.com",
                PhoneNumber = "987654321"
            };

            // Act
            var response = await client.PutAsJsonAsync($"/admin/aupdate-{user!.Id}", updatedUser);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCode_Returns_Ok_When_Code_Updated_Successfully_As_Admin()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var authRequest = new AuthRequest("admin@test.com", "AdminPassword", "AdminPassword");
            await TestLogin.Login_With_Test_User_Return_User(authRequest, client);

            var code = _dataContext.CodesDb!.FirstOrDefault();
            var updatedCode = new Code()
            {
                CodeTitle = code!.CodeTitle + '*',
                MyCode = code.MyCode + '*',
                WhatKindOfCode = code.WhatKindOfCode + '*',
                IsBackend = !code.IsBackend,
                IsVisible = !code.IsVisible,
            };

            var resultVis = !code.IsVisible;
            var resultBe = !code.IsBackend;

            // Act
            var response = await client.PutAsJsonAsync($"/admin/acupdate-{code.Id}", updatedCode);

            // Assert
            response.EnsureSuccessStatusCode();
            var updatedCodeResponse = await response.Content.ReadFromJsonAsync<Code>();
            Assert.NotNull(updatedCodeResponse);
            Assert.Equal(updatedCodeResponse.IsVisible, resultVis);
            Assert.Equal(updatedCodeResponse.IsBackend, resultBe);
            Assert.Contains('*', updatedCodeResponse.CodeTitle!);
            Assert.Contains('*', updatedCodeResponse.MyCode!);
            Assert.Contains('*', updatedCodeResponse.WhatKindOfCode!);

            var backdatedCode = new Code()
            {
                CodeTitle = code!.CodeTitle,
                MyCode = code.MyCode,
                WhatKindOfCode = code.WhatKindOfCode,
                IsBackend = !code.IsBackend,
                IsVisible = !code.IsVisible,
            };

            var response2 = await client.PutAsJsonAsync($"/admin/acupdate-{code.Id}", backdatedCode);

            // Assert
            response2.EnsureSuccessStatusCode();
            var backupdatedCodeResponse = await response2.Content.ReadFromJsonAsync<Code>();
            Assert.NotNull(backupdatedCodeResponse);
            Assert.Equal(backupdatedCodeResponse.IsVisible, resultVis);
            Assert.Equal(backupdatedCodeResponse.IsBackend, resultBe);
            Assert.DoesNotContain('*', backupdatedCodeResponse.CodeTitle!);
            Assert.DoesNotContain('*', backupdatedCodeResponse.MyCode!);
            Assert.DoesNotContain('*', backupdatedCodeResponse.WhatKindOfCode!);
        }

        [Fact]
        public async Task UpdateUser_With_Invalid_Data_Returns_Unauthorized()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var userId = Guid.NewGuid();
            var updatedUser = new User()
            {
                Email = null // Invalid email format
            };

            // Act
            var response = await client.PutAsJsonAsync($"/admin/aupdate-{userId}", updatedUser);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllUsersAsync_Returns_Unauthorized_When_No_Users_In_Database()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            // Act
            var response = await client.GetAsync("/admin/getUsers");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Token_Expired_Returns_Unauthorized()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            // Simulate an expired token
            var expiredToken = "ExpiredToken";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", expiredToken);

            // Act
            var response = await client.GetAsync("/admin/getUsers");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllCodes_Returns_OK_With_Codes_When_Authenticated_As_Admin()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var authRequest = new AuthRequest("admin@test.com", "AdminPassword", "AdminPassword");
            await TestLogin.Login_With_Test_User_Return_User(authRequest, client);

            // Act
            var response = await client.GetAsync("/admin/getCodes");

            // Assert
            response.EnsureSuccessStatusCode();
            var codes = await response.Content.ReadFromJsonAsync<List<Code>>();
            Assert.NotNull(codes);
            Assert.NotEmpty(codes);
        }

        [Fact]
        public async Task UpdateCode_Returns_Unauthorized_When_Not_Authenticated()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var codeId = Guid.NewGuid();
            var updatedCode = new Code();

            // Act
            var response = await client.PutAsJsonAsync($"/admin/acupdate-{codeId}", updatedCode);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
