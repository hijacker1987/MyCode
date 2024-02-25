using System.Net.Http;
using System.Threading.Tasks;
using MyCode_Backend_Server.Contracts.Services;
using Newtonsoft.Json;
using System.Text;

namespace MyCode_Backend_Server_Tests.Services
{
    public static class TestLogin
    {
        public static async Task<(string authToken, IEnumerable<string> cookies)> Login_With_Test_User(AuthRequest request, HttpClient client)
        {
            var authJsonRequest = JsonConvert.SerializeObject(request);
            var authContent = new StringContent(authJsonRequest, Encoding.UTF8, "application/json");

            var authResponse = await client.PostAsync("/users/login", authContent);
            authResponse.EnsureSuccessStatusCode();

            var authToken = ExtractAuthTokenFromCookies(authResponse.Headers.GetValues("Set-Cookie"));
            var cookies = authResponse.Headers.GetValues("Set-Cookie");

            return (authToken, cookies);
        }


        private static string ExtractAuthTokenFromCookies(IEnumerable<string> cookies)
        {
            foreach (var cookie in cookies)
            {
                var cookieParts = cookie.Split(';');
                foreach (var part in cookieParts)
                {
                    if (part.StartsWith("Authorization="))
                    {
                        return part["Authorization=".Length..];
                    }
                }
            }

            return null!;
        }
    }
}
