using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace MyCode_Backend_Server.Controllers
{
    [AllowAnonymous]
    [Route("cncbot/messages")]
    [ApiController]
    public class BotController(IBotFrameworkHttpAdapter adapter, IBot bot) : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter = adapter;
        private readonly IBot _bot = bot;

        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
            try
            {
                await _adapter.ProcessAsync(Request, Response, _bot);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
