using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MyCode_Backend_Server.Hubs;
using MyCode_Backend_Server.Models;
using System.Diagnostics.CodeAnalysis;

namespace MyCode_Backend_Server.Controllers
{
    [Route("ws/message")]
    [ApiController]
    public class MessageController([NotNull] IHubContext<MessageHub> messageHub) : ControllerBase
    {
        protected readonly IHubContext<MessageHub> _messageHub = messageHub;

        [HttpPost]
        public async Task<IActionResult> Create(SupportChat message)
        {
            await _messageHub.Clients.All.SendAsync("sendToFrontend", "The message " + message.Text + "' has been received.");

            return Ok();
        }
    }
}
