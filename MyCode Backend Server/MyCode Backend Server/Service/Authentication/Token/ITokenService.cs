using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Service.Authentication.Token
{
    public interface ITokenService
    {
        string CreateToken(User user, IList<string> roles);
        string GenerateRefreshToken();
    }
}
