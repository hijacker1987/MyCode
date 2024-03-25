using Microsoft.AspNetCore.Mvc;

namespace MyCode_Backend_Server.Service.Authentication.Token
{
    public static class TokenAndCookieHelper
    {
        public static ActionResult ValidateAndRefreshToken(ITokenService tokenService, HttpRequest request, HttpResponse response, ILogger logger)
        {
            var authorizationCookie = request.Cookies["Authorization"];

            if (tokenService.ValidateToken(authorizationCookie!))
            {
                var checkedToken = tokenService.Refresh(authorizationCookie!, request, response);

                if (checkedToken == null)
                {
                    logger.LogError("Token expired.");
                    return new BadRequestObjectResult("Token expired.");
                }
            }

            return null!;
        }

        public static CookieOptions GetCookieOptions(HttpRequest request, double time)
        {
            var managedIdCookie = new CookieBuilder()
            {
                Domain = request.Host.Host,
                Expiration = TimeSpan.FromSeconds(180),
                HttpOnly = false,
                SecurePolicy = CookieSecurePolicy.Always,
                SameSite = SameSiteMode.None
            };

            return new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(time),
                HttpOnly = managedIdCookie.HttpOnly,
                Secure = managedIdCookie.SecurePolicy == CookieSecurePolicy.Always,
                SameSite = managedIdCookie.SameSite
            };
        }

        public static CookieOptions GetCookieOptionsForHttpOnly(HttpRequest request, DateTimeOffset time)
        {
            var extendedTime = time.AddMinutes(5);

            return new CookieOptions
            {
                Domain = request.Host.Host,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = true,
                Expires = time,
                MaxAge = TimeSpan.FromSeconds((extendedTime - DateTime.UtcNow).TotalSeconds)
            };
        }
    }
}
