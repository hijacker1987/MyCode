using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Bot;
using Newtonsoft.Json;

namespace MyCode_Backend_Server.Controllers
{
    [AllowAnonymous]
    [Route("cncbot/messages")]
    [ApiController]
    public class BotController(FAQBot bot) : ControllerBase
    {
        private readonly FAQBot _bot = bot;

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
