namespace MyCode_Backend_Server.Contracts.Services
{
    public class AuthResponse(string Role, string Id)
    {
        public string Role { get; set; } = Role;
        public string Id { get; set; } = Id;
    }
}
