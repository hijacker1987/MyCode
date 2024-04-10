using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Service.Authentication
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(string email, string username, string password, string displayname, string phoneNumber);
        Task<string> RegisterExternalAccAsync(string email, string username, string displayName, string externalLoginMethod);
        Task<AuthResult> LoginAsync(string email, string password, string confirmPassword, HttpRequest request, HttpResponse response);
        Task<AuthResult> LoginExternalAsync(string email, HttpRequest request, HttpResponse response);
        Task ApprovedLogin(User approvedUser, HttpRequest request, HttpResponse response);
        Task<User> TryLoginUser(string email);
        Task<string> GetRoleStatusAsync(User user);
        Task<string> SetRoleStatusAsync(User user);
    }
}
