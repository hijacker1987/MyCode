﻿using Azure;
using MyCode_Backend_Server.Contracts.Services;
using MyCode_Backend_Server.Service.Authentication;
using Newtonsoft.Json;
using System.Text;

namespace MyCode_Backend_Server_Tests.Services
{
    public static class TestLogin
    {
        public static async Task<(string authToken, IEnumerable<string> cookies, AuthResponse result)> Login_With_Test_User(AuthRequest request, HttpClient client)
        {
            var authJsonRequest = JsonConvert.SerializeObject(request);
            var authContent = new StringContent(authJsonRequest, Encoding.UTF8, "application/json");

            var authResponse = await client.PostAsync("/users/login", authContent);
            authResponse.EnsureSuccessStatusCode();

            var content = await authResponse.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<AuthResponse>(content);

            var authToken = ExtractAuthTokenFromCookies(authResponse.Headers.GetValues("Set-Cookie"));
            var cookies = authResponse.Headers.GetValues("Set-Cookie");

            return (authToken, cookies, result!);
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
