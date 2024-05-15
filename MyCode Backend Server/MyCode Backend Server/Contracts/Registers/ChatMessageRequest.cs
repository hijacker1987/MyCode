namespace MyCode_Backend_Server.Contracts.Registers
{
    public record ChatMessageRequest(string RoomId, string UserId, string Message);
}
