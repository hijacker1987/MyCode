using Microsoft.AspNetCore.Mvc;

namespace MyCode_Backend_Server.Service.Authentication.Token
{
    public static class TokenHelper
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
    }
}
