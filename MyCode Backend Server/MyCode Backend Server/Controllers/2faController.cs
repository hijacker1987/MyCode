using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Email_Sender;

namespace MyCode_Backend_Server.Controllers
{
    [ApiController]
    [Route("/auth")]
    public class _2faController(UserManager<User> userManager, IEmailSender emailSender, ILogger<TokenController> logger) : Controller
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly ILogger<TokenController> _logger = logger;

        [HttpPost("enableTwoFactor"), Authorize("Admin, Useer")]
        public async Task<IActionResult> EnableTwoFactor(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            await _emailSender.SendEmailAsync(user.Email!, "Activate two factor authentication", $"Your unique code for activation: {code}");
            _logger.LogInformation("Email sent with the code for 2fa.");

            return Ok();
        }

        [HttpPost("verifyTwoFactor")]
        public async Task<IActionResult> VerifyTwoFactor(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", code);
            if (!isValid)
                return BadRequest("Code doesn't match.");

            _logger.LogInformation("Email verified with the code for 2fa.");

            return Ok();
        }

        [HttpPost("disableTwoFactor"), Authorize("Admin, User")]
        public async Task<IActionResult> DisableTwoFactor(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            _logger.LogInformation($"{user.UserName} disabled 2fa.");

            return Ok();
        }
    }
}
