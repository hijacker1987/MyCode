using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Hubs;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication;
using MyCode_Backend_Server.Service.Authentication.Token;
using MyCode_Backend_Server.Service.Chat;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace MyCode_Backend_Server.Controllers
{
    [Route("ws/message")]
    [ApiController]
    public class MessageController([NotNull] IHubContext<MessageHub> messageHub,
                                             DataContext dataContext,
                                             IChatService chatService,
                                             ITokenService tokenService,
                                             IAuthService authService,
                                             ILogger<MessageController> logger) : ControllerBase
    {
        protected readonly IHubContext<MessageHub> _messageHub = messageHub;
        private readonly DataContext _dataContext = dataContext;
        private readonly IChatService _chatService = chatService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IAuthService _authService = authService;
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

        [HttpGet("get-active-room"), Authorize(Roles = "Admin")]
        public IActionResult GetActiveRoom()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                _logger.LogError("No 'NameIdentifier' claim found in the ClaimsPrincipal.");
                return BadRequest("No 'NameIdentifier' claim found.");
            }

            var activeUserIds = _dataContext.SupportDb!
                                            .Where(u => u.IsActive && u.With == new Guid(userId!) || u.IsActive && u.With != Guid.Empty)
                                            .Select(u => u.UserId.ToString())
                                            .Distinct()
                                            .ToList();

            return Ok(activeUserIds);
        }

        [HttpGet("getAnyArchived"), Authorize(Roles = "Admin, Support, User")]
        public async Task<ActionResult<List<string>>> GetAnyArchived()
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

                if (await _authService.GetRoleStatusByIdAsync(userIdClaim) == "User")
                {
                    var messagesInDb = await _dataContext.SupportDb!
                                                        .Where(u => !u.IsActive && u.UserId.ToString() == userIdClaim)
                                                        .Select(u => u.UserId.ToString())
                                                        .Distinct()
                                                        .ToListAsync();
                 
                    return messagesInDb.Count != 0 ? Ok(messagesInDb) : NotFound();
                }
                else
                {
                    var messagesInDb = await _dataContext.SupportDb!
                                                         .Where(msg => !msg.IsActive)
                                                         .Select(u => u.UserId.ToString())
                                                         .Distinct()
                                                         .ToListAsync();

                    if (messagesInDb == null || messagesInDb.Count == 0) return NotFound();

                    return messagesInDb.Count != 0 ? Ok(messagesInDb) : NotFound();
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error occurred while fetching archived conversation!");
                return StatusCode(500, new { ErrorMessage = "Error occurred while fetching archived conversation!", ExceptionDetails = e.ToString() });
            }
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

        [HttpGet("getArchived-{id}"), Authorize(Roles = "Admin, Support")]
        public async Task<ActionResult<List<StoredMessage>>> GetArchivedById([FromRoute] Guid id)
        {
            try
            {
                var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
                if (tokenValidationResult != null) return tokenValidationResult;

                var messagesInDb = await _dataContext.SupportDb!
                                                     .Where(msg => msg.UserId == id && !msg.IsActive)
                                                     .OrderBy(msg => msg.When)
                                                     .ToListAsync();

                if (messagesInDb == null || messagesInDb.Count == 0) return NotFound();

                var messages = await _chatService.GetStoredMessagesByUser(false, messagesInDb);

                return Ok(messages);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}", e);
                return NotFound();
            }
        }

        [HttpGet("getActive-{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<StoredMessage>>> GetActiveById([FromRoute] Guid id)
        {
            try
            {
                var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
                if (tokenValidationResult != null) return tokenValidationResult;

                List<SupportChat> activeMessages = await _chatService.GetStoredActiveMessagesByRoom(id.ToString());

                if (activeMessages == null || activeMessages.Count == 0) return NotFound();

                List<StoredMessage> storedMessages = await _chatService.GetStoredMessagesByUser(false, activeMessages);

                return Ok(storedMessages);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}", e);
                return NotFound();
            }
        }

        [HttpPut("uActive-{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> DropBackActiveToSupportById([FromRoute] Guid id)
        {
            try
            {
                var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
                if (tokenValidationResult != null) return tokenValidationResult;

                List<SupportChat> activeMessages = await _chatService.GetStoredActiveMessagesByRoom(id.ToString());

                if (activeMessages == null || activeMessages.Count == 0) return NotFound();

                foreach (var msg in activeMessages)
                {
                    msg.With = Guid.Empty;
                }
                await _dataContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}", e);
                return NotFound();
            }
        }
    }
}
