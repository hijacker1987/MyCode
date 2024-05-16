using Azure.Core;
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

        public async Task<List<StoredMessage>> GetStoredMessagesByUser(bool ownRequest, List<SupportChat> messagesInConversation)
        {
            List<StoredMessage> messages = [];
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == messagesInConversation[0].UserId);
            string? userName = ownRequest ? "Me" : user!.DisplayName;

            var support = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == messagesInConversation[0].With);
            string? supportName = support == null || messagesInConversation[0].With == Guid.Empty ? "Customer Service" : support!.DisplayName;

            foreach (var msg in messagesInConversation)
            {
                if (msg.IsUser)
                {
                    messages.Add(new StoredMessage { Whom = userName, Text = msg.Text!, When = msg.When });
                }
                else
                {
                    messages.Add(new StoredMessage { Whom = supportName, Text = msg.Text!, When = msg.When });
                }
            }

            return messages;
        }

        public async Task<List<SupportChat>> GetStoredActiveMessagesByRoom(string room)
        {
            var activeMessages = await _dataContext.SupportDb!
                                                   .Where(msg => msg.UserId.ToString() == room && msg.IsActive && msg.With != Guid.Empty)
                                                   .OrderBy(msg => msg.When)
                                                   .ToListAsync();

            return activeMessages;
        }
    }
}
