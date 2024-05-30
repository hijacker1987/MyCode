using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Bot;
using Newtonsoft.Json;

namespace MyCode_Backend_Server.Controllers
{
    [AllowAnonymous]
    [Route("cncbot/botio")]
    [ApiController]
    public class BotController(IFAQBot bot) : ControllerBase
    {
        private readonly IFAQBot _bot = bot;

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] BotMessage message)
        {
            try
            {
                string result = "";

                if (message != null)
                {
                    result = await _bot.OnMessageActivityAsync(message);
                }

                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
