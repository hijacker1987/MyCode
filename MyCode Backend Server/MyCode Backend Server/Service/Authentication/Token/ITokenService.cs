using Microsoft.AspNetCore.Identity;

namespace MyCode_Backend_Server.Service.Authentication.Token
{
    public interface ITokenService
    {
        string CreateToken(IdentityUser user, string? role);
    }
}
