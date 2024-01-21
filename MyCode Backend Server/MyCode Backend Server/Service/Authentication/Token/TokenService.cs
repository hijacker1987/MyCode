using Microsoft.IdentityModel.Tokens;
using MyCode_Backend_Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyCode_Backend_Server.Service.Authentication.Token
{
    public class TokenService(IConfiguration configuration, ILogger<TokenService> logger) : ITokenService
    {
        private const int ExpirationMinutes = 10;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<TokenService> _logger = logger;

        public string CreateToken(User user, string? role)
        {
            var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
            var token = CreateJwtToken(CreateClaims(user, role, _logger), CreateSigningCredentials(), expiration);
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

        private static List<Claim> CreateClaims(User user, string? role, ILogger<TokenService> _logger)
        {
            try
            {
                var claims = new List<Claim>
                {
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
                _logger.LogError($"Error: {e.Message}", e);
                throw;
            }
        }

        private SigningCredentials CreateSigningCredentials()
        {
            string issueSignKey = _configuration["IssueSign"]!;

            if (issueSignKey.Length < 32)
            {
                throw new InvalidOperationException("The 'IssueSign' key must be at least 256 bits long.");
            }

            return new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issueSignKey)), SecurityAlgorithms.HmacSha256);
        }

    }
}
