using MyCode_Backend_Server.Contracts.Services;
using MyCode_Backend_Server.Models;
using Newtonsoft.Json;
using System.Text;

namespace MyCode_Backend_Server_Tests.Services
{
    public static class TestLogin
    {
        public static async Task<(string authToken, IEnumerable<string> cookies, string result)> Login_With_Test_User(AuthRequest request, HttpClient client)
        {
            var authJsonRequest = JsonConvert.SerializeObject(request);
            var authContent = new StringContent(authJsonRequest, Encoding.UTF8, "application/json");

            var authResponse = await client.PostAsync("/users/login", authContent);
            authResponse.EnsureSuccessStatusCode();
            var result = await authResponse.Content.ReadAsStringAsync();

            var authToken = ExtractAuthTokenFromCookies(authResponse.Headers.GetValues("Set-Cookie"));
            var cookies = authResponse.Headers.GetValues("Set-Cookie");

            return (authToken, cookies, result!);
        }

        public static async Task<User> Login_With_Test_User_Return_User(AuthRequest request, HttpClient client)
        {
            var (authToken, cookies, result) = await Login_With_Test_User(request, client);

            if (result == null)
                return null!;

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
            foreach (var cookie in cookies)
            {
                client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
            }

            var response = await client.GetAsync("/users/getUser");

            var content = await response.Content.ReadAsStringAsync();
            var returnedUser = JsonConvert.DeserializeObject<User>(content);

            return returnedUser!;
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
