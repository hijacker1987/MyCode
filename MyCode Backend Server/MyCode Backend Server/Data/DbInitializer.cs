using Microsoft.AspNetCore.Identity;
using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Data
{
     public class DbInitializer(IConfiguration configuration) : IDbInitializer
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task Initialize(DataContext context, UserManager<User> userManager)
        {
            context.Database.EnsureCreated();

            Console.WriteLine(_configuration["InitDb"]);

            if (_configuration["InitDb"] == "False" || context.Users.Any())
            {
                return;
            }

            var random = new Random();
            var userNames = new List<string>() { "John Doe", "Jane Doe" };
            var codeTypes = new List<string>() { "C#", "Java", "Python", "JavaScript", "C++",
                                                 "Ruby", "Swift", "Go", "Kotlin", "PHP",
                                                 "TypeScript", "Rust", "Objective-C", "C", "Scala",
                                                 "Perl", "Haskell", "Shell", "MATLAB", "Turbo Pascal" };

            var users = Enumerable.Range(1, 30).Select(i => new User
            {
                UserName = $"User{i}",
                Email = $"user{i}@example.com",
                DisplayName = $"{i}. example {userNames[random.Next(userNames.Count)]}",
                PhoneNumber = "123"
            }).ToArray();

            foreach (var user in users)
            {
                await userManager.CreateAsync(user, "Password");
            }
            context.SaveChanges();

            foreach (var user in users)
            {
                int numberOfCodes = random.Next(1, 6);

                for (int i = 1; i <= numberOfCodes; i++)
                {
                    var code = new Code
                    {
                        CodeTitle = $"Code of {user.UserName} - {i}",
                        MyCode = $"CodeContent{i}",
                        WhatKindOfCode = $"{codeTypes[random.Next(codeTypes.Count)]}",
                        IsBackend = random.Next(2) == 0,
                        IsVisible = random.Next(3) == 0,
                        UserId = user.Id
                    };

                    context.CodesDb!.Add(code);
                }
            }
            context.SaveChanges();
        }
    }
}
