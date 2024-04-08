using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Service.Authentication
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(string email, string username, string password, string displayname, string phoneNumber);
        Task<AuthResult> LoginAsync(string email, string password, string confirmPassword, HttpRequest request, HttpResponse response);
        Task<AuthResult> LoginExternalAsync(string email, HttpRequest request, HttpResponse response);
        Task<string> GetRoleStatusAsync(User user);
        Task<string> SetRoleStatusAsync(User user);
    }
}
