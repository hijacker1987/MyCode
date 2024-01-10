namespace MyCode_Backend_Server.Service.Authentication
{
    public record AuthResponse(string Email, string Username, string Token, string Role);
}
