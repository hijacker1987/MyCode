namespace MyCode_Backend_Server.Models
{
    public class UserWithRole(User user, string? role)
    {
        public User User { get; set; } = user;
        public string? Role { get; set; } = role;
    }
}