using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Service.Authentication.Token
{
    public class RefreshToken
    {
        [Key]
        public required string Refresh { get; set; }
        public required string Token { get; set; }
        public DateTime Expiration { get; set; }
        public required string UserId { get; set; }
    }
}
