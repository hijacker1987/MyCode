using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyCode_Backend_Server.Service.Authentication;
using Microsoft.AspNetCore.Identity;
using MyCode_Backend_Server.Models;
using Microsoft.AspNetCore.Authentication.Facebook;
using AspNet.Security.OAuth.GitHub;

namespace MyCode_Backend_Server.Controllers
{
    [ApiController]
    [Route("/account")]
    public class OathController(IAuthService authenticationService,
                                UserManager<User> userManager) : ControllerBase
    {
        private readonly IAuthService _authenticationService = authenticationService;
        private readonly UserManager<User> _userManager = userManager;

        [AllowAnonymous]
        [HttpGet("google-login")]
        public async Task GoogleLogin() => await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = Url.Action(nameof(GoogleResponse)) });

        [AllowAnonymous]
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return result?.Principal != null ? await ExternalLoginHelper(result, [4, 0, 1], "Google user") : BadRequest("Authentication failed");
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

            return result?.Principal != null ? await ExternalLoginHelper(result, [1, 0, 2], "Facebook user") : BadRequest("Authentication failed");
        }

        [AllowAnonymous]
        [HttpGet("github-login")]
        public async Task GitHubLogin() => await HttpContext.ChallengeAsync(GitHubAuthenticationDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = Url.Action(nameof(GitHubResponse)) });

        [AllowAnonymous]
        [HttpGet("github-response")]
        public async Task<IActionResult> GitHubResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return result?.Principal != null ? await ExternalLoginHelper(result, [7, 2, 1], "GitHub user") : BadRequest("Authentication failed");
        }

        private async Task<IActionResult> ExternalLoginHelper(AuthenticateResult result, int[] query, string externalUse)
        {
            var claims = result.Principal!.Identities.FirstOrDefault()!.Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

            var email = claims.ElementAtOrDefault(query[0])?.Value!;
            var username = claims.ElementAtOrDefault(query[1])?.Value!;
            var displayName = claims.ElementAtOrDefault(query[2])?.Value!;

            var loginResult = await _authenticationService.LoginExternalAsync(email, Request, Response);

            if (!loginResult.Success)
            {
                var regPass = await _authenticationService.RegisterExternalAccAsync(email, username, displayName, externalUse);

                loginResult = await _authenticationService.LoginAccAsync(email, regPass, regPass, Request, Response);
            }

            if (loginResult.Success)
            {
                var managedUser = await _authenticationService.TryGetUser(email);
                if (managedUser == null)
                {
                    return NotFound("User not found.");
                }

                await _authenticationService.ApprovedAccLogin(managedUser, Request, Response);

                return Redirect($"https://localhost:5173/myCodeHome/");
            }
            else
            {
                return BadRequest("Failed to login user");
            }
        }
    }
}
