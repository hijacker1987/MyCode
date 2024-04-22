using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace MyCode_Backend_Server.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class BotController(IBotFrameworkHttpAdapter adapter, IBot bot) : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter = adapter;
        private readonly IBot _bot = bot;

        [HttpPost]
        public async Task PostAsync()
        {
            await _adapter.ProcessAsync(Request, Response, _bot);
        }
    }
}
