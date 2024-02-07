namespace MyCode_Backend_Server.Models
{
    public class RefreshTM
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
