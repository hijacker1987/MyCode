using Microsoft.IdentityModel.Tokens;
using MyCode_Backend_Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyCode_Backend_Server.Service.Authentication.Token
{
    public class TokenService(IConfiguration configuration) : ITokenService
    {
        private const int ExpirationMinutes = 30;
        private readonly IConfiguration _configuration = configuration;

        public string CreateToken(User user, string? role)
        {
            var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
            var token = CreateJwtToken(CreateClaims(user, role), CreateSigningCredentials(), expiration);
            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials, DateTime expiration) =>
            new(
                _configuration["IssueAudience"],
                _configuration["IssueAudience"],
                claims,
                expires: expiration,
                signingCredentials: credentials
                );

        private static List<Claim> CreateClaims(User user, string? role)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Sub, "TokenForTheApiWithAuth"),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                    new(ClaimTypes.NameIdentifier, value: user.Id.ToString()),
                    new(ClaimTypes.Name, user.UserName!),
                    new(ClaimTypes.Email, user.Email!)
                };
                if (role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                return claims;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private SigningCredentials CreateSigningCredentials()
        {
            return new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["IssueSign"]!)), SecurityAlgorithms.HmacSha256);
        }
    }
}
