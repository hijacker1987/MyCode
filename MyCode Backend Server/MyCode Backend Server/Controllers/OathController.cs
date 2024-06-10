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
using System.Diagnostics.CodeAnalysis;

namespace MyCode_Backend_Server.Controllers
{
    [ApiController]
    [Route("/account")]
    public class OathController(IAuthService authService, UserManager<User> userManager) : ControllerBase
    {
        private readonly IAuthService _authenticationService = authService;
        private readonly UserManager<User> _userManager = userManager;

        [ExcludeFromCodeCoverage]
        [AllowAnonymous]
        [HttpGet("google-login")]
        public async Task GoogleLogin() => await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = Url.Action(nameof(GoogleResponse)) });

        [AllowAnonymous]
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return result?.Principal != null ? await ExternalLoginHelper(result, "Google user") : BadRequest("Authentication failed");
        }

        [ExcludeFromCodeCoverage]
        [AllowAnonymous]
        [HttpGet("facebook-login")]
        public async Task FacebookLogin() => await HttpContext.ChallengeAsync(FacebookDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = Url.Action(nameof(FacebookResponse)) });

        [AllowAnonymous]
        [HttpGet("facebook-response")]
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return result?.Principal != null ? await ExternalLoginHelper(result, "Facebook user") : BadRequest("Authentication failed");
        }

        [ExcludeFromCodeCoverage]
        [AllowAnonymous]
        [HttpGet("github-login")]
        public async Task GitHubLogin() => await HttpContext.ChallengeAsync(GitHubAuthenticationDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = Url.Action(nameof(GitHubResponse)) });

        [AllowAnonymous]
        [HttpGet("github-response")]
        public async Task<IActionResult> GitHubResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return result?.Principal != null ? await ExternalLoginHelper(result, "GitHub user") : BadRequest("Authentication failed");
        }

        [ExcludeFromCodeCoverage]
        [AllowAnonymous]
        [HttpGet("google-addon")]
        public async Task GoogleAddon(string attachment)
        {
            Response.Cookies.Append("Attachment", attachment);

            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = Url.Action(nameof(GoogleResponse2)) });
        }

        [ExcludeFromCodeCoverage]
        [AllowAnonymous]
        [HttpGet("google-response2")]
        public async Task<IActionResult> GoogleResponse2()
        {
            var attachment = Request.Cookies["Attachment"];

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return result?.Principal != null ? await ExternalAddonHelper(result, attachment!) : BadRequest("Addon failed");
        }

        [ExcludeFromCodeCoverage]
        [AllowAnonymous]
        [HttpGet("facebook-addon")]
        public async Task FacebookAddon(string attachment)
        {
            Response.Cookies.Append("Attachment", attachment);

            await HttpContext.ChallengeAsync(FacebookDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = Url.Action(nameof(FacebookResponse2)) });
        }

        [ExcludeFromCodeCoverage]
        [AllowAnonymous]
        [HttpGet("facebook-response2")]
        public async Task<IActionResult> FacebookResponse2()
        {
            var attachment = Request.Cookies["Attachment"];

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return result?.Principal != null ? await ExternalAddonHelper(result, attachment!) : BadRequest("Addon failed");
        }

        [ExcludeFromCodeCoverage]
        [AllowAnonymous]
        [HttpGet("github-addon")]
        public async Task GitHubAddon(string attachment)
        {
            Response.Cookies.Append("Attachment", attachment);

            await HttpContext.ChallengeAsync(GitHubAuthenticationDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = Url.Action(nameof(GitHubResponse2)) });
        }

        [ExcludeFromCodeCoverage]
        [AllowAnonymous]
        [HttpGet("github-response2")]
        public async Task<IActionResult> GitHubResponse2()
        {
            var attachment = Request.Cookies["Attachment"];

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return result?.Principal != null ? await ExternalAddonHelper(result, attachment!) : BadRequest("Addon failed");
        }

        [ExcludeFromCodeCoverage]
        private async Task<IActionResult> ExternalAddonHelper(AuthenticateResult result, string attachment)
        {
            var claims = result.Principal!.Identities.FirstOrDefault()!.Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

            if (claims == null)
            {
                return BadRequest("Authentication failed: No claims found.");
            }

            var email = claims.FirstOrDefault(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")!.Value;

            if (email == null)
            {
                return BadRequest("Authentication failed: Missing required claims.");
            }

            var managedUser = await _authenticationService.AddReliableAddress(email, attachment, Request, Response);

            if (managedUser != null)
            {
                var addonResult = await _authenticationService.TryGetUserById(attachment);

                if (addonResult != null)
                {
                    Response.Cookies.Delete("Authorization");
                    Response.Cookies.Delete("RefreshAuthorization");
                    Response.Cookies.Delete("UI");
                    Response.Cookies.Delete("UR");
                    Response.Cookies.Delete("UD");
                    Response.Cookies.Delete("Attachment");

                    await _authenticationService.LoginExternalAsync(email, Request, Response);
                    await _authenticationService.ApprovedAccLogin(addonResult, Request, Response);

                    return Redirect($"https://localhost:5173/myCodeHome/");
                }
                else
                {
                    return BadRequest($"Failed to login user.");
                }
            }
            else
            {
                return BadRequest($"Failed to add {email}");
            }
        }

        [ExcludeFromCodeCoverage]
        private async Task<IActionResult> ExternalLoginHelper(AuthenticateResult result, string externalUse)
        {
            var claims = result.Principal!.Identities.FirstOrDefault()!.Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

            if (claims == null)
            {
                return BadRequest("Authentication failed: No claims found.");
            }

            var email = claims.FirstOrDefault(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")!.Value;
            var username = claims.FirstOrDefault(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")!.Value;
            var displayName = claims.FirstOrDefault(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")!.Value;

            if (email == null || username == null || displayName == null)
            {
                return BadRequest("Authentication failed: Missing required claims.");
            }

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
                return BadRequest($"Failed to login user.");
            }
        }
    }
}
