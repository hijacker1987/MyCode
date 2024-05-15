using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Hubs;
using MyCode_Backend_Server.Models;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace MyCode_Backend_Server.Controllers
{
    [Route("ws/message")]
    [ApiController]
    public class MessageController([NotNull] IHubContext<MessageHub> messageHub, DataContext dataContext) : ControllerBase
    {
        protected readonly IHubContext<MessageHub> _messageHub = messageHub;
        private readonly DataContext _dataContext = dataContext;

        [HttpPost, Authorize(Roles = "User, Support, Admin")]
        public async Task<IActionResult> Create(SupportChat message)
        {
            await _messageHub.Clients.Group(message.UserId.ToString()).SendAsync("ReceiveMessage", "The message '" + message.Text + "' has been received.");

            return Ok();
        }

        [HttpGet("get-room"), Authorize(Roles = "Support, Admin")]
        public IActionResult GetRoom()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var activeUserIds = _dataContext.SupportDb!
                .Where(u => u.IsActive && u.With == new Guid(userId!) || u.IsActive && u.With == Guid.Empty)
                .Select(u => u.UserId.ToString())
                .Distinct()
                .ToList();

            return Ok(activeUserIds);
        }
    }
}
