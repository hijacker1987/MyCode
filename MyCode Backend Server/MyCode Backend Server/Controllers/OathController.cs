using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyCode_Backend_Server.Service.Authentication;
using Microsoft.AspNetCore.Identity;
using MyCode_Backend_Server.Models;
using Microsoft.AspNetCore.Authentication.Facebook;
using MyCode_Backend_Server.Data;

namespace MyCode_Backend_Server.Controllers
{
    [ApiController]
    [Route("/account")]
    public class OathController(IAuthService authenticationService,
                                DataContext dataContext,
                                UserManager<User> userManager) : ControllerBase
    {
        private readonly IAuthService _authenticationService = authenticationService;
        private readonly DataContext _dataContext = dataContext;
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
                    var regPass = await _authenticationService.RegisterExternalAccAsync(email, username, displayName, "Googler");

                    loginResult = await _authenticationService.LoginAsync(email, regPass, regPass, Request, Response);
                }

                if (loginResult.Success)
                {
                    var managedUser = await _authenticationService.TryLoginUser(email);
                    if (managedUser == null)
                    {
                        return NotFound("User not found.");
                    }

                    await _authenticationService.ApprovedLogin(managedUser, Request, Response);

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
                    var regPass = await _authenticationService.RegisterExternalAccAsync(email, username, displayName, "Facebook user");

                    loginResult = await _authenticationService.LoginAsync(email, regPass, regPass, Request, Response);
                }

                if (loginResult.Success)
                {
                    var managedUser = await _authenticationService.TryLoginUser(email);
                    if (managedUser == null)
                    {
                        return NotFound("User not found.");
                    }

                    await _authenticationService.ApprovedLogin(managedUser, Request, Response);

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
    }
}
