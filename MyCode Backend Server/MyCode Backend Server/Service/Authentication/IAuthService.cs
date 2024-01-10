namespace MyCode_Backend_Server.Service.Authentication
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(string email, string username, string password, string phoneNumber, string role);

        Task<AuthResult> LoginAsync(string email, string password);
    }
}
