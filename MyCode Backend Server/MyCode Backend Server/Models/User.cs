using Microsoft.AspNetCore.Identity;

namespace MyCode_Backend_Server.Models
{
    public class User : IdentityUser<Guid>
    {
        public string? DisplayName { get; set; }
        public DateTime LastTimeLogin { get; set; }

        public User(string displayName, DateTime lastTimeLogin)
        {
            DisplayName = displayName;
            LastTimeLogin = lastTimeLogin;
        }

        public User()
        {
            LastTimeLogin = DateTime.UtcNow;
        }
    }
}
