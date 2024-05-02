using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication.Token;
using MyCode_Backend_Server.Service.Email_Sender;

namespace MyCode_Backend_Server.Service.Authentication
{
    public class AuthService(IConfiguration configuration,
                             UserManager<User> userManager,
                             ITokenService tokenService,
                             DataContext dataContext,
                             IEmailSender emailSender,
                             ILogger<AuthService> logger) : IAuthService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly UserManager<User> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly DataContext _dataContext = dataContext;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly ILogger<AuthService> _logger = logger;

        private const string LowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
        private const string UpperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string NumericChars = "0123456789";
        private const string SpecialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        private static readonly Random random = new();

        public async Task<AuthResult> RegisterAccAsync(string email, string username, string password, string displayname, string phoneNumber)
        {
            bool isExternalRegister = false;

            if (phoneNumber == "Ext")
            {
                isExternalRegister = true;
                phoneNumber = "Not provided yet!";
            }

            var user = new User { UserName = username, Email = email, DisplayName = displayname, PhoneNumber = phoneNumber };
            var secData = await TryGetUser(email);

            if (secData != null)
            {
                _logger.LogError($"Registration failed: user exist");
                return FailedRegistration("", await _userManager.CreateAsync(user, password) ?? IdentityResult
                    .Failed(new IdentityError { Code = "UnknownError", Description = "Registration failed." }), email, username);
            }

            var result = await _userManager.CreateAsync(user, password);

            if (result == null || user == null)
            {
                _logger.LogError($"Registration failed: result is null or user is null");
                return FailedRegistration("", result ?? IdentityResult.Failed(new IdentityError { Code = "UnknownError", Description = "Registration failed." }),
                                                                                                  email, username);
            }

            if (isExternalRegister)
            {
                var regReliable = new Mfa(email, true)
                {
                    UserId = user.Id,
                };

                await _dataContext.MFADb!.AddAsync(regReliable);
            }

            if (!result.Succeeded)
            {
                return FailedRegistration(user.Id.ToString(), result, email, username);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _dataContext.SaveChangesAsync();

                return new AuthResult(user.Id.ToString(), true, email, username, displayname, phoneNumber, "");
            }
        }

        public async Task<string> RegisterExternalAccAsync(string email, string username, string displayName, string externalLoginMethod)
        {
            var generatedPassword = GeneratePassword();
            await RegisterAccAsync(email,
                                   username,
                                   generatedPassword,
                                   displayName,
                                   "Ext");

            var subject = $"Greetings {externalLoginMethod}, Welcome to My Code!!!";

            var message = $"{displayName}, Your automatically generated password to the website is: {generatedPassword} Thank You very much to use my application, ENJOY IT!";

            await _emailSender.SendEmailAsync(email, subject, message);

            return generatedPassword;
        }

        public async Task<AuthResult> LoginAccAsync(string email, string password, string confirmPassword, HttpRequest request, HttpResponse response)
        {
            if (request.HttpContext.User.Identity!.IsAuthenticated)
            {
                return AlreadyLoggedIn(request.HttpContext.User.Identity.Name!);
            }

            var managedUser = await TryGetUser(email);

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

            return await AuthBuilder(managedUser, request, response);
        }

        public async Task<AuthResult> LoginExternalAsync(string email, HttpRequest request, HttpResponse response)
        {
            User? userToLogin = await TryGetUser(email);

            if (userToLogin == null)
            {
                return InvalidEmail(email);
            }

            return await AuthBuilder(userToLogin, request, response);
        }

        public async Task ApprovedAccLogin(User approvedUser, HttpRequest request, HttpResponse response)
        {
            var role = await GetRoleStatusAsync(approvedUser);

            approvedUser.LastTimeLogin = DateTime.UtcNow;

            await _userManager.UpdateAsync(approvedUser);
            await _dataContext.SaveChangesAsync();

            var userId = approvedUser.Id.ToString();

            response.Cookies.Append("UI", userId, TokenAndCookieHelper.GetCookieOptions(request, 3));
            response.Cookies.Append("UR", role!, TokenAndCookieHelper.GetCookieOptions(request, 3));
            response.Cookies.Append("UD", approvedUser.DisplayName!, TokenAndCookieHelper.GetCookieOptions(request, 3));
        }

        public async Task<User?> TryGetUser(string email)
        {
            var managedUser = await _userManager.FindByEmailAsync(email);

            var secondaryData = await _dataContext.MFADb!.FirstOrDefaultAsync(u => u.ReliableEmail == email);

            if (secondaryData != null)
            {
                var result = await _dataContext.Users!.FirstOrDefaultAsync(u => u.Id == secondaryData.UserId);
                if (result != null)
                {
                    managedUser = result;
                }
            }

            return managedUser;
        }

        public async Task<User?> TryGetUserById(string id)
        {
            var managedUser = await _userManager.FindByIdAsync(id);

            var secondaryData = await _dataContext.MFADb!.FirstOrDefaultAsync(u => u.UserId.ToString() == id);

            if (secondaryData != null)
            {
                var result = await _dataContext.Users!.FirstOrDefaultAsync(u => u.Id == secondaryData.UserId);
                if (result != null)
                {
                    managedUser = result;
                }
            }

            return managedUser;
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

        private async Task<AuthResult> AuthBuilder(User user, HttpRequest request, HttpResponse response)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.CreateToken(user, roles);
            var accessTokenExp = Convert.ToDouble(_configuration["AccessTokenExp"]);
            var refreshToken = _tokenService.CreateRefreshToken();
            var refreshTokenExp = Convert.ToDouble(_configuration["RefreshTokenExp"]);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["RefreshTokenExp"]));

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

            return new AuthResult(user.Id.ToString(),
                                  true,
                                  user.Email,
                                  user.UserName,
                                  user.DisplayName,
                                  user.PhoneNumber,
                                  accessToken);
        }

        public async Task<User> AddReliableAddress(string email, string toAttachTo, HttpRequest request, HttpResponse response)
        {
            var checker = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == toAttachTo);

            var existing = await _dataContext.MFADb!.FirstOrDefaultAsync(u => u.ReliableEmail == email);

            if (checker != null && existing != null)
            {
                return checker;
            }

            if (checker != null && existing == null)
            {
                var regReliable = new Mfa(email, true)
                {
                    UserId = checker.Id,
                };

                await _dataContext.MFADb!.AddAsync(regReliable);
                await _dataContext.SaveChangesAsync();

                return checker;
            }

            return null!;
        }

        private static string GeneratePassword()
        {
            string allChars = LowerCaseChars + UpperCaseChars + NumericChars + SpecialChars;
            return new string(Enumerable.Range(1, 10)
                                        .Select(_ => allChars[random.Next(allChars.Length)])
                                        .ToArray());
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
