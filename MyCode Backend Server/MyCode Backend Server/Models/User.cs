using Microsoft.AspNetCore.Identity;

namespace MyCode_Backend_Server.Models
{
    public class User : IdentityUser<Guid>
    {
        public string? DisplayName { get; set; }
        public DateTime LastTimeLogin { get; set; }
        public List<Code>? Code { get; set; }

        public User(string displayName, DateTime lastTimeLogin, List<Code> code)
        {
            DisplayName = displayName;
            LastTimeLogin = lastTimeLogin;
            Code = code ?? [];
        }

        public User()
        {
            Code = [];
            LastTimeLogin = DateTime.UtcNow;
        }
    }
}
