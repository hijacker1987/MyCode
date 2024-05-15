using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Controllers
{
    [ApiController]
    [Route("/token")]
    public class TokenController(UserManager<User> userManager, ILogger<TokenController> logger) : Controller
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly ILogger<TokenController> _logger = logger;

        [HttpDelete("revoke"), Authorize(Roles = ("Admin, Support, User"))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Revoke()
        {
            var username = HttpContext.User.Identity?.Name;

            if (username == null)
                return Unauthorized();

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return Unauthorized();

            Response.Cookies.Delete("Authorization");
            Response.Cookies.Delete("RefreshAuthorization");
            Response.Cookies.Delete("UI");
            Response.Cookies.Delete("UR");
            Response.Cookies.Delete("UD");
            user.RefreshToken = null;

            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Revoked token!");

            return Ok();
        }
    }
}
