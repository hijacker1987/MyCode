using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MyCode_Backend_Server.Service.Authentication.Token
{
    public class TokenService(IConfiguration configuration,
                              UserManager<User> userManager,
                              DataContext dataContext,
                              IWebHostEnvironment environment,
                              ILogger<TokenService> logger) : ITokenService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly UserManager<User> _userManager = userManager;
        private readonly DataContext _dataContext = dataContext;
        private readonly IWebHostEnvironment _environment = environment;
        private readonly ILogger<TokenService> _logger = logger;

        public string CreateToken(User user, IList<string> roles)
        {
            var issueAud = "";

            if (_environment.IsEnvironment("Test"))
            {
                issueAud = "api With Test Authentication comes and goes here";
            }
            else if (_environment.IsEnvironment("Development"))
            {
                issueAud = _configuration["IssueAudience"];
            }

            JwtSecurityToken token = CreateJwtToken(CreateClaims(user, roles, _logger),
                                     CreateSigningCredentials(),
                                     issueAud!,
                                     DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["AccessTokenExp"])));

            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        private static JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials, string issueAud, DateTime expiration) =>
            new(issueAud,
                issueAud,
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
            var issueSignKey = "";

            if (_environment.IsEnvironment("Test"))
            {
                issueSignKey = "V3ryStr0ngP@ssw0rdW1thM0reTh@n256B1tsF0rT3st1ng";
            }
            else if (_environment.IsEnvironment("Development"))
            {
                issueSignKey = _configuration["IssueSign"];
            }

            if (string.IsNullOrEmpty(issueSignKey) )
            {
                throw new ArgumentNullException(nameof(issueSignKey));
            }

            if (issueSignKey.Length < 32)
            {
                throw new InvalidOperationException("The 'IssueSign' key must be at least 256 bits long.");
            }

            return new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issueSignKey)), SecurityAlgorithms.HmacSha256);
        }

        public string CreateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var issueAud = "";
            var issueSign = "";

            if (_environment.IsEnvironment("Test"))
            {
                issueAud = "api With Test Authentication comes and goes here";
                issueSign = "V3ryStr0ngP@ssw0rdW1thM0reTh@n256B1tsF0rT3st1ng";
            }
            else if (_environment.IsEnvironment("Development"))
            {
                issueAud = _configuration["IssueAudience"];
                issueSign = _configuration["IssueSign"];
            }

                var validation = new TokenValidationParameters
            {
                ValidIssuer = issueAud,
                ValidAudience = issueAud,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issueSign!)),
                ValidateLifetime = false
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
        }

        public string Refresh(string authCookie, HttpRequest request, HttpResponse response)
        {
            var refCookie = request.Cookies["RefreshAuthorization"];
            var principal = GetPrincipalFromExpiredToken(authCookie);

            if (authCookie == null || refCookie == null || principal?.Identity?.Name is null)
            {
                return null!;
            }

            var user = _userManager.FindByNameAsync(principal.Identity.Name).Result;

            if (user == null || user.RefreshToken != refCookie)
            {
                if (user!.RefreshTokenExpiry < DateTime.UtcNow)
                {
                    user.RefreshToken = null;
                    response.Cookies.Delete(authCookie);
                    response.Cookies.Delete(refCookie);
                    response.Cookies.Delete("UI");
                    response.Cookies.Delete("UR");

                    _userManager.UpdateAsync(user).Wait();
                    _dataContext.SaveChangesAsync();
                }

                return null!;
            }

            var roles = _userManager.GetRolesAsync(user).Result;
            var token = CreateToken(user, roles);
            var accessTokenExp = Convert.ToDouble(_configuration["AccessTokenExp"]);

            response.Cookies.Append("Authorization", token, TokenAndCookieHelper.GetCookieOptionsForHttpOnly(request, DateTime.UtcNow.AddMinutes(accessTokenExp)));

            _logger.LogInformation("Refresh went successfully!");

            return token;
        }

        public bool ValidateToken(string toValToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (tokenHandler.ReadToken(toValToken) is not JwtSecurityToken token)
            {
                return true;
            }

            var expirationTime = token.ValidTo;

            if (expirationTime < DateTime.UtcNow)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
