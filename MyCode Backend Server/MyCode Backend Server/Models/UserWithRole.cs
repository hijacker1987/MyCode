namespace MyCode_Backend_Server.Models
{
    namespace MyCode_Backend_Server.Models
    {
        public class UserWithRole
        {
            public UserWithRole()
            {
                User = new User();
                Role = string.Empty;
            }

            public UserWithRole(User user, string? role)
            {
                User = user;
                Role = role;
            }

            public User User { get; set; }
            public string? Role { get; set; }
        }
    }
}
