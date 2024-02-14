using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Service.Authentication.Token
{
    public interface ITokenService
    {
        string CreateToken(User user, IList<string> roles);
        string CreateRefreshToken();
        bool ValidateToken(string toValToken);
        CookieOptions GetCookieOptions(HttpRequest request, DateTimeOffset time);
        string Refresh(string authCookie, string refCookie, HttpRequest request, HttpResponse response);
    }
}
