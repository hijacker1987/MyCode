using Microsoft.AspNetCore.Identity;

namespace MyCode_Backend_Server.Models
{
    public class User : IdentityUser<Guid>
    {
        public string? ReliableEmail { get; set; }
        public string? DisplayName { get; set; }
        public DateTime LastTimeLogin { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }

        public User()
        {
            LastTimeLogin = DateTime.UtcNow.AddHours(1);
        }
    }
}
