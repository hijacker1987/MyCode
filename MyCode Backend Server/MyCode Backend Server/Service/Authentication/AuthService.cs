using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication.Token;

namespace MyCode_Backend_Server.Service.Authentication
{
    public class AuthService(IConfiguration configuration,
                             UserManager<User> userManager,
                             ITokenService tokenService,
                             DataContext dataContext,
                             ILogger<AuthService> logger) : IAuthService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly UserManager<User> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly DataContext _dataContext = dataContext;
        private readonly ILogger<AuthService> _logger = logger;

        public async Task<AuthResult> RegisterAsync(string email, string username, string password, string displayname, string phoneNumber)
        {
            bool isExternalRegister = false;

            if (phoneNumber == "Ext")
            {
                isExternalRegister = true;
                phoneNumber = "Not provided yet!";
            }

            var user = new User { UserName = username, Email = email, DisplayName = displayname, PhoneNumber = phoneNumber };

            if (email.EndsWith("@gmail.com") || isExternalRegister)
            {
                user.ReliableEmail = email;
            }

            var result = await _userManager.CreateAsync(user, password);

            if (result == null || user == null)
            {
                _logger.LogError($"Registration failed: result is null or user is null");
                return FailedRegistration("", result ?? IdentityResult.Failed(new IdentityError { Code = "UnknownError", Description = "Registration failed." }),
                                                                                                  email, username);
            }

            if (!result.Succeeded)
            {
                return FailedRegistration(user.Id.ToString(), result, email, username);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "User");
                return new AuthResult(user.Id.ToString(), true, email, username, displayname, phoneNumber, "");
            }
        }

        public async Task<AuthResult> LoginAsync(string email, string password, string confirmPassword, HttpRequest request, HttpResponse response)
        {
            if (request.HttpContext.User.Identity!.IsAuthenticated)
            {
                return AlreadyLoggedIn(request.HttpContext.User.Identity.Name!);
            }

            var managedUser = await _userManager.FindByEmailAsync(email);

            if (managedUser == null)
            {
                return InvalidEmail(email);
            }

            if (password == null || confirmPassword == null || password != confirmPassword)
            {
                return InvalidPassword(email, managedUser.UserName!);
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, password);

            if (!isPasswordValid)
            {
                return InvalidPassword(email, managedUser.UserName!);
            }

            var roles = await _userManager.GetRolesAsync(managedUser);
            var accessToken = _tokenService.CreateToken(managedUser, roles);
            var accessTokenExp = Convert.ToDouble(_configuration["AccessTokenExp"]);
            var refreshToken = _tokenService.CreateRefreshToken();
            var refreshTokenExp = Convert.ToDouble(_configuration["RefreshTokenExp"]);

            managedUser.RefreshToken = refreshToken;
            managedUser.RefreshTokenExpiry = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["RefreshTokenExp"]));

            var cookieOptions1 = TokenAndCookieHelper.GetCookieOptionsForHttpOnly(request, DateTime.UtcNow.AddMinutes(accessTokenExp));
            var cookieOptions2 = TokenAndCookieHelper.GetCookieOptionsForHttpOnly(request, DateTime.UtcNow.AddHours(refreshTokenExp));

            if (cookieOptions1 != null && cookieOptions2 != null)
            {
                response.Cookies.Append("Authorization", accessToken, cookieOptions1);
                response.Cookies.Append("RefreshAuthorization", refreshToken, cookieOptions2);
            }
            else
            {
                _logger.LogError("Cookie options are null");
                return new AuthResult("", false, "", "", "", "", "");
            }

            return new AuthResult(managedUser.Id.ToString(),
                                  true, managedUser.Email,
                                  managedUser.UserName,
                                  managedUser.DisplayName,
                                  managedUser.PhoneNumber,
                                  accessToken);
        }

        public async Task<AuthResult> LoginExternalAsync(string email, HttpRequest request, HttpResponse response)
        {
            var managedUser = await _userManager.FindByEmailAsync(email);
            var reliableUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.ReliableEmail == email);

            if (managedUser == null && reliableUser == null)
            {
                return InvalidEmail(email);
            }

            User userToLogin;
            if (managedUser != null)
            {
                userToLogin = managedUser;
            }
            else
            {
                userToLogin = reliableUser!;
            }

            var roles = await _userManager.GetRolesAsync(userToLogin);
            var accessToken = _tokenService.CreateToken(userToLogin, roles);
            var accessTokenExp = Convert.ToDouble(_configuration["AccessTokenExp"]);
            var refreshToken = _tokenService.CreateRefreshToken();
            var refreshTokenExp = Convert.ToDouble(_configuration["RefreshTokenExp"]);

            userToLogin.RefreshToken = refreshToken;
            userToLogin.RefreshTokenExpiry = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["RefreshTokenExp"]));

            var cookieOptions1 = TokenAndCookieHelper.GetCookieOptionsForHttpOnly(request, DateTime.UtcNow.AddMinutes(accessTokenExp));
            var cookieOptions2 = TokenAndCookieHelper.GetCookieOptionsForHttpOnly(request, DateTime.UtcNow.AddHours(refreshTokenExp));

            if (cookieOptions1 != null && cookieOptions2 != null)
            {
                response.Cookies.Append("Authorization", accessToken, cookieOptions1);
                response.Cookies.Append("RefreshAuthorization", refreshToken, cookieOptions2);
            }
            else
            {
                _logger.LogError("Cookie options are null");
                return new AuthResult("", false, "", "", "", "", "");
            }

            return new AuthResult(userToLogin.Id.ToString(),
                                  true, userToLogin.Email,
                                  userToLogin.UserName,
                                  userToLogin.DisplayName,
                                  userToLogin.PhoneNumber,
                                  accessToken);
        }

        public async Task<string> GetRoleStatusAsync(User user)
        {
            var result = await _userManager.GetRolesAsync(user);

            return result.FirstOrDefault()!;
        }

        public async Task<string> SetRoleStatusAsync(User user)
        {
            var userRole = await GetRoleStatusAsync(user);

            if (userRole == "User")
            {
                await _userManager.RemoveFromRoleAsync(user, "User");
                await _userManager.AddToRoleAsync(user, "Admin");
                _logger.LogInformation("Successfully set A role");
            } 
            else
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                await _userManager.AddToRoleAsync(user, "User");
                _logger.LogInformation("Successfully set U role");
            }

            return user.Email!;
        }

        private static AuthResult FailedRegistration(string id, IdentityResult result, string email, string username)
        {
            var authenticationResult = new AuthResult(id, false, email, username, "", "", "");

            foreach (var identityError in result.Errors)
            {
                authenticationResult.ErrorMessages.Add(identityError.Code ?? "", identityError.Description);
            }

            return authenticationResult;
        }

        private static AuthResult InvalidEmail(string email)
        {
            var result = new AuthResult("", false, email, "", "", "", "");
            result.ErrorMessages.Add("Bad credentials", "Invalid email");

            return result;
        }

        public static AuthResult InvalidPassword(string email, string userName)
        {
            var result = new AuthResult("", false, email, userName, "", "", "");
            result.ErrorMessages.Add("Bad credentials", "Invalid password");

            return result;
        }

        private static AuthResult AlreadyLoggedIn(string username)
        {
            var result = new AuthResult("", false, "", "", "", "", "");
            result.ErrorMessages.Add("AlreadyLoggedIn", $"The user '{username}' is already logged in.");

            return result;
        }
    }
}
