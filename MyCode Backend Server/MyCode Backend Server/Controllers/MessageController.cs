using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Hubs;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication.Token;
using MyCode_Backend_Server.Service.Chat;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace MyCode_Backend_Server.Controllers
{
    [Route("ws/message")]
    [ApiController]
    public class MessageController([NotNull] IHubContext<MessageHub> messageHub,
                                             DataContext dataContext,
                                             IChatService chatService,
                                             ITokenService tokenService,
                                             ILogger<MessageController> logger) : ControllerBase
    {
        protected readonly IHubContext<MessageHub> _messageHub = messageHub;
        private readonly DataContext _dataContext = dataContext;
        private readonly IChatService _chatService = chatService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<MessageController> _logger = logger;

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

            if (userId == null)
            {
                _logger.LogError("No 'NameIdentifier' claim found in the ClaimsPrincipal.");
                return BadRequest("No 'NameIdentifier' claim found.");
            }

            var activeUserIds = _dataContext.SupportDb!
                .Where(u => u.IsActive && u.With == new Guid(userId!) || u.IsActive && u.With == Guid.Empty)
                .Select(u => u.UserId.ToString())
                .Distinct()
                .ToList();

            return Ok(activeUserIds);
        }

        [HttpGet("getOwnArchived"), Authorize(Roles = "Admin, Support, User")]
        public async Task<ActionResult<List<StoredMessage>>> GetOwnArchived()
        {
            try
            {
                var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
                if (tokenValidationResult != null) return tokenValidationResult;

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userIdClaim == null)
                {
                    _logger.LogError("No 'NameIdentifier' claim found in the ClaimsPrincipal.");
                    return BadRequest("No 'NameIdentifier' claim found.");
                }

                var messagesInDb = await _dataContext.SupportDb!
                                                       .Where(msg => msg.UserId.ToString() == userIdClaim && !msg.IsActive)
                                                       .OrderBy(msg => msg.When)
                                                       .ToListAsync();

                if (messagesInDb == null || messagesInDb.Count == 0) return NotFound();

                var messages = await _chatService.GetStoredMessagesByUser(true, messagesInDb);

                return Ok(messages);
            }
            catch (Exception e)
            {
                _logger.LogError("Error occurred while fetching archived conversation!");
                return StatusCode(500, new { ErrorMessage = "Error occurred while fetching archived conversation!", ExceptionDetails = e.ToString() });
            }
        }
    }
}
