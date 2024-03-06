using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication.Token;
using MyCode_Backend_Server.Service.Email_Sender;

namespace MyCode_Backend_Server.Controllers
{
    [ApiController]
    [Route("/auth")]
    public class AuthController(UserManager<User> userManager, DataContext dataContext, ITokenService tokenService, IEmailSender emailSender, ILogger<TokenController> logger) : Controller
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly DataContext _dataContext = dataContext;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly ILogger<TokenController> _logger = logger;


        [HttpPost("enableTwoFactor"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult> EnableTwoFactor([FromBody] string userId)
        {
            var authorizationCookie = Request.Cookies["Authorization"];

            if (_tokenService.ValidateToken(authorizationCookie!))
            {
                var checkedToken = _tokenService.Refresh(authorizationCookie!, Request, Response);

                if (checkedToken == null)
                {
                    _logger.LogError("Token expired.");
                    return BadRequest("Token expired.");
                }
            }

            var user = _dataContext.Users.FirstOrDefault(u => u.Id.ToString() == userId);
            if (user == null)
                return NotFound();

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            await _emailSender.SendEmailAsync(user.Email!, "Activate two factor authentication", $"Your unique code for activation: {code}");
            _logger.LogInformation("Email sent with the code for 2fa.");

            return Ok();
        }

        [HttpPost("verifyTwoFactor"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult> VerifyTwoFactor([FromBody] string userId, string code)
        {
            var authorizationCookie = Request.Cookies["Authorization"];

            if (_tokenService.ValidateToken(authorizationCookie!))
            {
                var checkedToken = _tokenService.Refresh(authorizationCookie!, Request, Response);

                if (checkedToken == null)
                {
                    _logger.LogError("Token expired.");
                    return BadRequest("Token expired.");
                }
            }

            var user = _dataContext.Users.FirstOrDefault(u => u.Id.ToString() == userId);
            if (user == null)
                return NotFound();

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", code);
            if (!isValid)
                return BadRequest("Code doesn't match.");

            _logger.LogInformation("Email verified with the code for 2fa.");

            return Ok();
        }

        [HttpPost("disableTwoFactor"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult> DisableTwoFactor([FromBody] string userId)
        {
            var authorizationCookie = Request.Cookies["Authorization"];

            if (_tokenService.ValidateToken(authorizationCookie!))
            {
                var checkedToken = _tokenService.Refresh(authorizationCookie!, Request, Response);

                if (checkedToken == null)
                {
                    _logger.LogError("Token expired.");
                    return BadRequest("Token expired.");
                }
            }

            var user = _dataContext.Users.FirstOrDefault(u => u.Id.ToString() == userId);
            if (user == null)
                return NotFound();

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            _logger.LogInformation($"{user.UserName} disabled 2fa.");

            return Ok();
        }
    }
}
