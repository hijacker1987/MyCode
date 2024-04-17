using Microsoft.AspNetCore.Mvc;
using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Service.Authentication
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAccAsync(string email, string username, string password, string displayname, string phoneNumber);
        Task<string> RegisterExternalAccAsync(string email, string username, string displayName, string externalLoginMethod);
        Task<AuthResult> LoginAccAsync(string email, string password, string confirmPassword, HttpRequest request, HttpResponse response);
        Task<AuthResult> LoginExternalAsync(string email, HttpRequest request, HttpResponse response);
        Task ApprovedAccLogin(User approvedUser, HttpRequest request, HttpResponse response);
        Task<User> AddReliableAddress(string email, string toAttachTo,  HttpRequest request, HttpResponse response);
        Task<User?> TryGetUser(string email);
        Task<string> GetRoleStatusAsync(User user);
        Task<string> SetRoleStatusAsync(User user);
    }
}
