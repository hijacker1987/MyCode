using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MyCode_Backend_Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MyCode_Backend_Server.Service.Authentication.Token
{
    public class TokenService(IConfiguration configuration, UserManager<User> userManager, ILogger<TokenService> logger) : ITokenService
    {
        private const int ExpirationMinutes = 1;
        private readonly IConfiguration _configuration = configuration;
        private readonly UserManager<User> _userManager = userManager;
        private readonly ILogger<TokenService> _logger = logger;

        public string CreateToken(User user, IList<string> roles)
        {
            var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
            var token = CreateJwtToken(CreateClaims(user, roles, _logger), CreateSigningCredentials(), expiration);
            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials, DateTime expiration) =>
            new(
                _configuration["IssueAudience"],
                _configuration["IssueAudience"],
                claims,
                notBefore: DateTime.UtcNow,
                expires: expiration,
                signingCredentials: credentials
                );

        private static List<Claim> CreateClaims(User user, IList<string> roles, ILogger<TokenService> _logger)
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

                foreach (var role in roles)
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

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var validation = new TokenValidationParameters
            {
                ValidIssuer = _configuration["IssueAudience"],
                ValidAudience = _configuration["IssueAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["IssueSign"]!)),
                ValidateLifetime = false
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
        }

        public string Refresh(string authCookie, string refCookie)
        {
            var principal = GetPrincipalFromExpiredToken(authCookie);

            if (principal?.Identity?.Name is null)
                return null!;

            var user = _userManager.FindByNameAsync(principal.Identity.Name).Result;

            if (user == null || user.RefreshToken != refCookie || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                if (user!.RefreshTokenExpiry < DateTime.UtcNow)
                {
                    user.RefreshToken = null;

                    _userManager.UpdateAsync(user).Wait();
                }

                return null!;
            }

            var roles = _userManager.GetRolesAsync(user).Result;
            var token = CreateToken(user, roles);

            _logger.LogInformation("Refresh went successfully!");

            return token;
        }
    }
}
