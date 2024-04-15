using Microsoft.AspNetCore.Identity;
using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Data
{
     public class DbInitializer(IConfiguration configuration) : IDbInitializer
     {
        private readonly IConfiguration _configuration = configuration;

        public async Task Initialize(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            context.Database.EnsureCreated();

            var random = new Random();
            var roleList = new List<string> { "Admin", "User" };
            var userNames = new List<string>() { "John Doe", "Jane Doe" };
            var codeTypes = new List<string>() { "Batch", "C", "C#", "C++", "CoffeeScript", "CSS", "Diff", "Elm", "F#", "Go",
                                                 "Handlebars", "Haskell", "HTML", "Java", "JavaScript", "JSON", "Kotlin", "LESS", "Lua", "Markdown",
                                                 "MATLAB", "Objective-C", "Perl", "PHP", "Powershell", "Pug", "Python", "R", "Razor", "Ruby",
                                                 "Rust", "SASS", "Scala", "SCSS", "Shell", "Swift", "TypeScript", "Turbo Pascal", "VB", "XML" };

            foreach (var role in roleList)
            {
                await CreateRole(roleManager, role);
            }

            var pass = _configuration["APass"];
            var mail = _configuration["AEmail"];
            var phone = _configuration["ACall"];
            var uName = _configuration["UName"];
            var dName = _configuration["AName"];

            var adminInDbExist = await userManager.FindByEmailAsync(mail!);

            if (adminInDbExist == null)
            {
            var admin = new User { UserName = uName, Email = mail, DisplayName = dName, PhoneNumber = phone };
            var adminCreated = await userManager.CreateAsync(admin, pass!);

            if (adminCreated.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            if (_configuration["InitDb"] == "False" || context.Users.Any(u => u.Email == "user1@example.com"))
            {
                context.SaveChanges();

                return;
            }

            var users = Enumerable.Range(1, 30).Select(i => new User
            {
                UserName = $"User{i}",
                Email = $"user{i}@example.com",
                DisplayName = $"{i}. example {userNames[random.Next(userNames.Count)]}",
                PhoneNumber = "123"
            }).ToArray();

            foreach (var user in users)
            {
                var userResult = await userManager.CreateAsync(user, "Password");

                if (userResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }

            var gUser = new User
            {
                UserName = "GoogleUser",
                Email = "googleuser@gmail.com",
                DisplayName = "Googler",
                PhoneNumber = "123456",
                TwoFactorEnabled = true,
                EmailConfirmed = true
            };

            var gUserResult = await userManager.CreateAsync(gUser, "Password");

            if (gUserResult.Succeeded)
            {
                await userManager.AddToRoleAsync(gUser, "User");
            }

            var mfaResult = new Mfa
            {
                UserId = gUser.Id,
                ReliableEmail = "googleuser@gmail.com",
                SecondaryLoginMethod = true,
            };

            await context.MFADb!.AddAsync(mfaResult);

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

        public async Task CreateRole(RoleManager<IdentityRole<Guid>> roleManager, string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
     }
}
