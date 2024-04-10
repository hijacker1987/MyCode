using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyCode_Backend_Server.Service.Authentication;
using MyCode_Backend_Server.Service.Email_Sender;
using Microsoft.AspNetCore.Identity;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication.Token;
using Microsoft.AspNetCore.Authentication.Facebook;

namespace MyCode_Backend_Server.Controllers
{
    [ApiController]
    [Route("/account")]
    public class OathController(IAuthService authenticationService,
                                IEmailSender emailSender,
                                DataContext dataContext,
                                UserManager<User> userManager) : ControllerBase
    {
        private readonly IAuthService _authenticationService = authenticationService;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly DataContext _dataContext = dataContext;
        private readonly UserManager<User> _userManager = userManager;

        private const string LowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
        private const string UpperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string NumericChars = "0123456789";
        private const string SpecialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        private static readonly Random random = new();

        [AllowAnonymous]
        [HttpGet("google-login")]
        public async Task GoogleLogin() => await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = Url.Action(nameof(GoogleResponse)) });

        [AllowAnonymous]
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                var claims = result.Principal!.Identities.FirstOrDefault()!.Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                });

                var email = claims.ElementAtOrDefault(4)?.Value!;
                var username = claims.ElementAtOrDefault(0)?.Value!;
                var displayName = claims.ElementAtOrDefault(1)?.Value!;

                var loginResult = await _authenticationService.LoginExternalAsync(email, Request, Response);

                if (!loginResult.Success)
                {
                    var regPass = await RegisterExternalAccAsync(email, username, displayName, "Googler");

                    loginResult = await _authenticationService.LoginAsync(email, regPass, regPass, Request, Response);
                }

                if (loginResult.Success)
                {
                    var managedUser = await _userManager.FindByEmailAsync(email);
                    if (managedUser == null)
                    {
                        return NotFound("User not found.");
                    }

                    await ApprovedExternalLogin(managedUser);

                    return Redirect($"https://localhost:5173/myCodeHome/");
                }
                else
                {
                    return BadRequest("Failed to login user");
                }
            }
            else
            {
                return BadRequest("Authentication failed");
            }
        }

        [AllowAnonymous]
        [HttpGet("facebook-login")]
        public async Task FacebookLogin() => await HttpContext.ChallengeAsync(FacebookDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = Url.Action(nameof(FacebookResponse)) });

        [AllowAnonymous]
        [HttpGet("facebook-response")]
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                var claims = result.Principal!.Identities.FirstOrDefault()!.Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                });

                var email = claims.ElementAtOrDefault(1)?.Value!;
                var username = claims.ElementAtOrDefault(0)?.Value!;
                var displayName = claims.ElementAtOrDefault(2)?.Value!;
                
                var loginResult = await _authenticationService.LoginExternalAsync(email, Request, Response);

                if (!loginResult.Success)
                {
                    var regPass = await RegisterExternalAccAsync(email, username, displayName, "Facebook user");

                    loginResult = await _authenticationService.LoginAsync(email, regPass, regPass, Request, Response);
                }

                if (loginResult.Success)
                {
                    var managedUser = await _userManager.FindByEmailAsync(email);
                    if (managedUser == null)
                    {
                        return NotFound("User not found.");
                    }

                    await ApprovedExternalLogin(managedUser);

                    return Redirect($"https://localhost:5173/myCodeHome/");
                }
                else
                {
                    return BadRequest("Failed to login user");
                }
            }
            else
            {
                return BadRequest("Authentication failed");
            }
        }

        private async Task ApprovedExternalLogin(User approvedUser)
        {
            var roles = await _userManager.GetRolesAsync(approvedUser);
            await _userManager.AddToRolesAsync(approvedUser, roles);

            approvedUser.LastTimeLogin = DateTime.UtcNow;

            await _userManager.UpdateAsync(approvedUser);
            await _dataContext.SaveChangesAsync();

            var userId = approvedUser.Id.ToString();
            var userRole = roles.FirstOrDefault();

            Response.Cookies.Append("UI", userId, TokenAndCookieHelper.GetCookieOptions(Request, 3));
            Response.Cookies.Append("UR", userRole!, TokenAndCookieHelper.GetCookieOptions(Request, 3));
        }

        private async Task<string> RegisterExternalAccAsync(string email, string username, string displayName, string externalLoginMethod)
        {
            var generatedPassword = GeneratePassword();
            await _authenticationService.RegisterAsync(email,
                                                       username,
                                                       generatedPassword,
                                                       displayName,
                                                       "Ext");

            var subject = $"Greetings {externalLoginMethod}, Welcome to My Code!!!";

            var message = $"{displayName}, Your automatically generated password to the website is: {generatedPassword} Thank You very much to use my application, ENJOY IT!";

            await _emailSender.SendEmailAsync(email, subject, message);

            return generatedPassword;
        }
        private static string GeneratePassword()
        {
            string allChars = LowerCaseChars + UpperCaseChars + NumericChars + SpecialChars;
            return new string(Enumerable.Range(1, 10)
                                        .Select(_ => allChars[random.Next(allChars.Length)])
                                        .ToArray());
        }
    }
}
