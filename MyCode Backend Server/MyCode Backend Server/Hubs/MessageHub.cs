using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication;
using MyCode_Backend_Server.Service.Chat;

namespace MyCode_Backend_Server.Hubs
{
    public class MessageHub(IDictionary<string, ChatRooms> connections,
                            UserManager<User> userManager,
                            DataContext dataContext,
                            IChatService chatService,
                            IAuthService authService,
                            ILogger<MessageHub> logger) : Hub
    {
        private readonly IDictionary<string, ChatRooms> _connections = connections;
        private readonly UserManager<User> _userManager = userManager;
        private readonly DataContext _dataContext = dataContext;
        private readonly IChatService _chatService = chatService;
        private readonly IAuthService _authService = authService;
        private readonly ILogger<MessageHub> _logger = logger;

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public async Task<IActionResult> JoinRoom(ChatRooms userConnection)
        {
            try
            {
                var currentUser = _dataContext.Users.FirstOrDefault(u => u.Id.ToString() == userConnection.User);
                if (currentUser == null)
                {
                    _logger.LogError("Unable to find user with the provided ID.");
                    return new StatusCodeResult(404);
                }

                var userRole = await _authService.GetRoleStatusByIdAsync(currentUser!.Id.ToString());
                if (userRole == null)
                {
                    _logger.LogError("Unable to find role with the provided user ID.");
                    return new StatusCodeResult(404);
                }

                string sendMessage = userRole == "User" ? "Your connection to the Support established!" : "Your connection to the User established!";

                if (userRole == "User")
                {
                    await _chatService.ConnectToRoom(Context.ConnectionId, userConnection.ChatRoom!);
                    _connections[Context.ConnectionId] = userConnection;
                }

                if (userRole == "Support")
                {
                    var activeMessages = await _dataContext.SupportDb!
                                                           .Where(msg => msg.UserId.ToString() == userConnection.ChatRoom && msg.IsActive && (msg.With == Guid.Empty || msg.With == currentUser.Id))
                                                           .ToListAsync();

                    if (activeMessages.Count != 0)
                    {
                        await _chatService.ConnectToRoom(Context.ConnectionId, userConnection.ChatRoom!);
                        _connections[Context.ConnectionId] = userConnection;

                        await _chatService.JoinRoomAndAttachMessagesToUser(currentUser, activeMessages);
                    }
                    else
                    {
                        await Clients.Caller.SendAsync("FailedToJoinRoom", "Server response", "Already in progress!", DateTime.Now);
                        return new StatusCodeResult(409);
                    }
                }

                await Clients.Caller.SendAsync("ReceiveMessage", currentUser.DisplayName, $"{sendMessage}", DateTime.Now);

                var messagesInRoom = await _dataContext.SupportDb!
                                                       .Where(msg => msg.UserId.ToString() == userConnection.ChatRoom && msg.IsActive)
                                                       .OrderBy(msg => msg.When)
                                                       .ToListAsync();

                if (messagesInRoom.Count != 0)
                {
                    var messages = userRole == "User" ? await _chatService.GetStoredMessagesByUser(true, messagesInRoom) : await _chatService.GetStoredMessagesByUser(false, messagesInRoom);
                    foreach (var message in messages)
                    {
                        await Clients.Caller.SendAsync("ReceiveMessage", message.Whom, $"{message.Text}", message.When);
                    }
                }

                await SendConnectedUser(userConnection.ChatRoom!);

                return new StatusCodeResult(200);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "An error occurred in {MethodName}", nameof(JoinRoom));
                return new StatusCodeResult(409);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred in {MethodName}", nameof(JoinRoom));
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> LeaveRoom(ChatRooms room)
        {
            await Clients.Group(room.ChatRoom!).SendAsync("ReceiveMessage", "Server feedback", "Problem marked as solved, the request has been archived. Thank You for choosing us!", DateTime.Now);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.ChatRoom!);

            var messagesInRoom = await _dataContext.SupportDb!
                                                   .Where(msg => msg.UserId.ToString() == room.ChatRoom && msg.IsActive)
                                                   .ToListAsync();

            if (messagesInRoom.Count > 0)
            {
                foreach (var msg in messagesInRoom) { msg.IsActive = false; }
                await _dataContext.SaveChangesAsync();

                var remainingParticipants = await _dataContext.SupportDb!
                                                              .Where(msg => msg.UserId.ToString() == room.ChatRoom && msg.IsActive)
                                                              .Select(msg => msg.UserId.ToString())
                                                              .Distinct()
                                                              .CountAsync();

                if (remainingParticipants == 0)
                {
                    await Clients.Group(room.ChatRoom!.ToLower()).SendAsync("ConnectedUser", new Dictionary<string, string>());
                    _connections.Remove(Context.ConnectionId);

                    return new StatusCodeResult(200);
                }
            }

            return new StatusCodeResult(200);
        }

        private Task SendConnectedUser(string room)
        {
            var users = _connections.Values.Where(user => user.ChatRoom!.Equals(room, StringComparison.CurrentCultureIgnoreCase)).Select(_connections => _connections.User);
            var newDictionary = new Dictionary<string, string>();

            foreach (var user in users)
            {
                newDictionary.TryAdd(user!, _userManager.Users.FirstOrDefault(u => u.Id.ToString() == user)!.Id.ToString());
            }

            return Clients.Group(room.ToLower()).SendAsync("ConnectedUser", newDictionary);
        }

        public async Task SendMessage(ChatMessageRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Request is null!");
                return;
            }

            if (string.IsNullOrEmpty(request.UserId))
            {
                _logger.LogError("No ID attached to the request.");
                return;
            }

            if (Context.ConnectionId != null && _connections.TryGetValue(Context.ConnectionId, out ChatRooms? roomConnection))
            {
                if (await _authService.GetRoleStatusByIdAsync(request.UserId) == "User")
                {
                    List<SupportChat> activeMessages = await _chatService.GetStoredActiveMessagesByRoom(request.RoomId);

                    if (activeMessages.Count != 0)
                    {
                        await _dataContext.SupportDb!.AddAsync(new SupportChat { Text = request.Message, With = activeMessages.First().With, UserId = new Guid(request.UserId) });
                    }
                    else
                    {
                        await _dataContext.SupportDb!.AddAsync(new SupportChat { Text = request.Message, UserId = new Guid(request.UserId) });
                    }
                }
                else if (await _authService.GetRoleStatusByIdAsync(request.UserId) == "Support")
                {
                    var activeMessages = await _dataContext.SupportDb!
                                                           .Where(msg => msg.UserId.ToString() == roomConnection.ChatRoom! && msg.IsActive && msg.With.ToString() != request.UserId)
                                                           .ToListAsync();

                    if (activeMessages.Count != 0)
                    {
                        await Clients.Caller.SendAsync("FailedToJoinRoom", "Already in progress!", DateTime.Now);
                        return;
                    }
                    await _dataContext.SupportDb!.AddAsync(new SupportChat { Text = request.Message, IsUser = false, With = new Guid(request.UserId), UserId = new Guid(request.RoomId) });
                }

                await _dataContext.SaveChangesAsync();

                var user = await _authService.TryGetUserById(request.UserId);
                if (user == null)
                {
                    _logger.LogError("Unable to find user with the provided ID.");
                    return;
                }

                await Clients.Group(roomConnection.ChatRoom!).SendAsync("ReceiveMessage", user!.DisplayName, request.Message, DateTime.Now);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exp)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out ChatRooms? roomConnection))
            {
                var user = await _authService.TryGetUserById(roomConnection.User!);

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomConnection.ChatRoom!);
                await Clients.Group(roomConnection.ChatRoom!).SendAsync("UserDisconnected", user!.DisplayName, " has left the room!", DateTime.Now);
                _connections.Remove(Context.ConnectionId);

                await SendConnectedUser(roomConnection.ChatRoom!);
            }

            await base.OnDisconnectedAsync(exp);
        }
    }
}
