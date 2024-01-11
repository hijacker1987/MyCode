using Microsoft.AspNetCore.Identity;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication.Token;

namespace MyCode_Backend_Server.Service.Authentication
{
        public class AuthService(UserManager<User> userManager, ITokenService tokenService) : IAuthService
        {
            private readonly UserManager<User> _userManager = userManager;
            private readonly ITokenService _tokenService = tokenService;

            public async Task<AuthResult> RegisterAsync(string email, string username, string password, string displayname, string phoneNumber, string role)
            {
                var user = new User { UserName = username, Email = email, DisplayName = displayname, PhoneNumber = phoneNumber };
                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    return FailedRegistration(user.Id.ToString(), result, email, username);
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, role);

                    return new AuthResult(user.Id.ToString(), true, email, username, "");
                }
            }

            private static AuthResult FailedRegistration(string id, IdentityResult result, string email, string username)
            {
                var authenticationResult = new AuthResult(id, false, email, username, "");

                foreach (var identityError in result.Errors)
                {
                    authenticationResult.ErrorMessages.Add(identityError.Code ?? "", identityError.Description);
                }

                return authenticationResult;
            }

            public async Task<AuthResult> LoginAsync(string email, string password)
            {
                var managedUser = await _userManager.FindByEmailAsync(email);

                if (managedUser == null)
                {
                    return InvalidEmail(email);
                }

                var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, password);

                if (!isPasswordValid)
                {
                    return InvalidPassword(email, managedUser.UserName!);
                }

                var roles = await _userManager.GetRolesAsync(managedUser);

                await _userManager.AddToRolesAsync(managedUser, roles);

                var accessToken = _tokenService.CreateToken(managedUser, roles.First());

                return new AuthResult(managedUser.Id.ToString(), true, managedUser.Email!, managedUser.UserName!, accessToken);
            }

        private static AuthResult InvalidEmail(string email)
            {
                var result = new AuthResult("", false, email, "", "");
                result.ErrorMessages.Add("Bad credentials", "Invalid email");

                return result;
            }

            public static AuthResult InvalidPassword(string email, string userName)
            {
                var result = new AuthResult("", false, email, userName, "");
                result.ErrorMessages.Add("Bad credentials", "Invalid password");

                return result;
            }
        }
}
