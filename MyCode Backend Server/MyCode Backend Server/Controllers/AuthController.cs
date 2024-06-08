using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("basicsTwoFactor"), Authorize(Roles = "Admin, Support, User")]
        public ActionResult BasicsTwoFactor(string userId)
        {
            var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
            if (tokenValidationResult != null) return tokenValidationResult;

            if (string.IsNullOrEmpty(userId))
                return BadRequest("Must have an Id");

            var user = _dataContext.Users.FirstOrDefault(u => u.Id.ToString() == userId);

            if (user == null)
                return NotFound();

            var userMfa = _dataContext.MFADb!.FirstOrDefault(u => u.UserId == user.Id);

            bool isReliable = false;
            if (userMfa != null && userMfa.ReliableEmail != null && userMfa.ReliableEmail.Length > 0)
            {
                isReliable = true;
            }

            var basicInfo = new { user.TwoFactorEnabled, user.EmailConfirmed, isReliable };

            return CreatedAtAction("BasicsTwoFactor", basicInfo);
        }

        [HttpPost("enableTwoFactor"), Authorize(Roles = "Admin, Support, User")]
        public async Task<ActionResult> EnableTwoFactor([FromBody] string userId)
        {
            var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
            if (tokenValidationResult != null) return tokenValidationResult;

            if (string.IsNullOrEmpty(userId))
                return BadRequest("Must have an Id");

            var user = _dataContext.Users.FirstOrDefault(u => u.Id.ToString() == userId);

            if (user == null)
                return NotFound();

            if (user.EmailConfirmed)
                return BadRequest("The security option already set!");

            if (user.TwoFactorEnabled)
                return BadRequest("Already enabled, ask for verification E-mail!");

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            var userMfa = _dataContext.MFADb!.FirstOrDefault(u => u.UserId == user.Id);
            if (userMfa == null)
                return NotFound();

            try
            {
                await _emailSender.SendEmailAsync(userMfa.ReliableEmail!, "Activate two factor authentication", $"Your unique code for activation: {code}");
                _logger.LogInformation("Email sent with the code for 2fa.");
            }
            catch (Exception)
            {
                _logger.LogError($"Error sending email!");
                return StatusCode(500, "Error sending email. Please try again later.");
            }

            return Ok();
        }

        [HttpPost("verifyTwoFactor"), Authorize(Roles = "Admin, Support, User")]
        public async Task<ActionResult> VerifyTwoFactor([FromBody] VerifyModel request)
        {
            var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
            if (tokenValidationResult != null) return tokenValidationResult;

            if (string.IsNullOrEmpty(request.UserId))
                return BadRequest("Must provide a user Id");

            if (string.IsNullOrEmpty(request.Attachment))
                return BadRequest("Must provide a verification code");

            var user = _dataContext.Users.FirstOrDefault(u => u.Id.ToString() == request.UserId);

            if (user == null)
                return NotFound("User not found");

            if (!user.TwoFactorEnabled)
                return BadRequest("Two-factor authentication is not enabled for this user");

            if (user.EmailConfirmed)
                return BadRequest("Two-factor authentication is already confirmed for this user");

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", request.Attachment);

            if (!isValid)
                return BadRequest("Invalid verification code");

            var userMfa = await _dataContext.MFADb!.FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (userMfa != null)
            {
                user.EmailConfirmed = true;

                if (request.External)
                {
                    userMfa.SecondaryLoginMethod = true;
                }

                await _dataContext.SaveChangesAsync();
            }

            _logger.LogInformation("Email verified with the code for 2fa.");

            return Ok();
        }

        [HttpPost("disableTwoFactor"), Authorize(Roles = "Admin, Support, User")]
        public async Task<ActionResult> DisableTwoFactor([FromBody] string userId)
        {
            var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
            if (tokenValidationResult != null) return tokenValidationResult;

            if (string.IsNullOrEmpty(userId))
                return BadRequest("Must have an Id");

            var user = _dataContext.Users.FirstOrDefault(u => u.Id.ToString() == userId);

            if (user == null)
                return NotFound();

            var userMfa = _dataContext.MFADb!.FirstOrDefault(u => u.UserId == user.Id);

            if (userMfa == null)
                return NotFound();

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            _logger.LogInformation("User disabled 2fa.");

            userMfa.ReliableEmail = "";
            userMfa.SecondaryLoginMethod = false;

            user.EmailConfirmed = false;
            await _dataContext.SaveChangesAsync();

            return Ok();
        }
    }
}
