using Microsoft.AspNetCore.Identity;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication.Token;

namespace MyCode_Backend_Server.Service.Authentication
{
        public class AuthService(IConfiguration configuration, UserManager<User> userManager, ITokenService tokenService, ILogger<AuthService> logger) : IAuthService
        {
        private readonly IConfiguration _configuration = configuration;
        private readonly UserManager<User> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<AuthService> _logger = logger;

        public async Task<AuthResult> RegisterAsync(string email, string username, string password, string displayname, string phoneNumber)
        {
            var user = new User { UserName = username, Email = email, DisplayName = displayname, PhoneNumber = phoneNumber };
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
                return new AuthResult(user.Id.ToString(), true, email, username, displayname, phoneNumber,"");
            }
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

        public async Task<AuthResult> LoginAsync(string email, string password, string confirmPassword, HttpRequest request, HttpResponse response)
        {

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

            var cookieOptions1 = _tokenService.GetCookieOptions(request, DateTime.UtcNow.AddMinutes(accessTokenExp));
            var cookieOptions2 = _tokenService.GetCookieOptions(request, DateTime.UtcNow.AddHours(refreshTokenExp));

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
    }
}
