using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Hubs;
using MyCode_Backend_Server.Models;
using Hub = Microsoft.AspNetCore.SignalR.Hub;

namespace MyCode_Backend_Server.Service.Chat
{
    public class ChatService(IHubContext<MessageHub> hubContext,
                             DataContext dataContext) : Hub, IChatService
    {
        private readonly IHubContext<MessageHub> _hubContext = hubContext;
        private readonly DataContext _dataContext = dataContext;

        public async Task ConnectToRoom(string connectWho, string connectTo)
        {
            await _hubContext.Groups.AddToGroupAsync(connectWho, connectTo);
        }

        public async Task JoinRoomAndAttachMessagesToUser(User currentUser, List<SupportChat> messages)
        {
            foreach (var message in messages)
            {
                message.With = currentUser.Id;
                _dataContext.Entry(message).State = EntityState.Modified;
            }
            await _dataContext.SaveChangesAsync();
        }

        public async Task<List<StoredMessage>> SendActiveStoredMessages(string currentUser, string userRole, List<SupportChat> messages)
        {
            List<StoredMessage> storedMessages = [];

            foreach (var message in messages)
            {
                var whom = "";

                if (userRole == "User")
                {
                    var support = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == message.With)!;

                    whom = message.IsUser ? currentUser : support!.DisplayName;
                }
                else
                {
                    var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == message.UserId)!;

                    whom = !message.IsUser ? currentUser : user!.DisplayName;
                }

                storedMessages.Add(new StoredMessage { Whom = whom, Text = message.Text, When = message.When });
            }

            return storedMessages;
        }
    }
}
