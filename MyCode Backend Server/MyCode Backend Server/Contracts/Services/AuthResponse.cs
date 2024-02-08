namespace MyCode_Backend_Server.Contracts.Services
{
    public class AuthResponse(string Token, string RefreshToken, DateTime RtExpired)
    {
        public string Token { get; set; } = Token;
        public string RefreshToken { get; set; } = RefreshToken;
        public DateTime RtExpired { get; set; } = RtExpired;
    }
}
