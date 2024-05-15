using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Service.Chat
{
    public interface IChatService
    {
        Task ConnectToRoom(string connectWho, string connectTo);
        Task JoinRoomAndAttachMessagesToUser(User currentUser, List<SupportChat> messages);
        Task<List<StoredMessage>> SendActiveStoredMessages(string currentUser, string userRole, List<SupportChat> messages);
    }
}
