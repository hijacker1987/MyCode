using Microsoft.AspNetCore.Identity;

namespace MyCode_Backend_Server.Models
{
    public class User : IdentityUser<Guid>
    {
        public string? DisplayName { get; set; }
        public DateTime LastTimeLogin { get; set; }

        public User()
        {
            LastTimeLogin = DateTime.UtcNow.AddHours(1);
        }
    }
}
