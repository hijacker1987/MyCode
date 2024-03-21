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

                var loginResult = await _authenticationService.LoginGoogleAsync(email, Request, Response);

                if (!loginResult.Success)
                {
                    var regPass = await RegisterGoogleAccAsync(email, username, displayName);
                    
                    loginResult = await _authenticationService.LoginAsync(email, regPass, regPass, Request, Response);
                }

                if (loginResult.Success)
                {
                    var managedUser = await _userManager.FindByEmailAsync(email);
                    if (managedUser == null)
                    {
                        return NotFound("User not found.");
                    }

                    var roles = await _userManager.GetRolesAsync(managedUser);
                    await _userManager.AddToRolesAsync(managedUser, roles);

                    managedUser.LastTimeLogin = DateTime.UtcNow;

                    await _userManager.UpdateAsync(managedUser);
                    await _dataContext.SaveChangesAsync();

                    var managedIdCookie = new CookieBuilder()
                    {
                        Domain = Request.Host.Host,
                        Expiration = TimeSpan.FromSeconds(18000),
                        HttpOnly = false,
                        SecurePolicy = CookieSecurePolicy.Always,
                        SameSite = SameSiteMode.None
                    };

                    var userId = managedUser.Id.ToString();
                    var userRole = roles.FirstOrDefault();

                    Response.Cookies.Append("UI", userId, new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddMinutes(30),
                        HttpOnly = managedIdCookie.HttpOnly,
                        Secure = managedIdCookie.SecurePolicy == CookieSecurePolicy.Always,
                        SameSite = managedIdCookie.SameSite
                    });

                    Response.Cookies.Append("UR", userRole!, new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddMinutes(3),
                        HttpOnly = managedIdCookie.HttpOnly,
                        Secure = managedIdCookie.SecurePolicy == CookieSecurePolicy.Always,
                        SameSite = managedIdCookie.SameSite
                    });

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

        private async Task<string> RegisterGoogleAccAsync(string email, string username, string displayName)
        {
            var generatedPassword = GeneratePassword();
            await _authenticationService.RegisterAsync(email,
                                                       username,
                                                       generatedPassword,
                                                       displayName,
                                                       "");

            var subject = "Greetings Googler, Welcome to My Code!!!";

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
