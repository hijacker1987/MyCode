namespace MyCode_Backend_Server.Service.Authentication
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(string email, string username, string password, string displayname, string phoneNumber);

        Task<AuthResult> LoginAsync(string email, string password, HttpRequest request, HttpResponse response);
    }
}
