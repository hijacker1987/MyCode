using Microsoft.AspNetCore.SignalR;

namespace MyCode_Backend_Server.Hubs
{
    public class MessageHub : Hub
    {
        public async Task SendMessage(string userid, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", userid, message);
        }
    }
}
