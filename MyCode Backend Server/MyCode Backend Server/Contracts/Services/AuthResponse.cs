namespace MyCode_Backend_Server.Contracts.Services
{
    public class AuthResponse(string Email, string Username, string Token, string RefreshToken)
    {
        public string Email { get; set; } = Email;
        public string Username { get; set; } = Username;
        public string Token { get; set; } = Token;
        public string RefreshToken { get; set; } = RefreshToken;
    }
}
