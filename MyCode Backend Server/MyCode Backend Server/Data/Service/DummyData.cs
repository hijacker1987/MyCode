using Microsoft.AspNetCore.Identity;
using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Data.Service
{
    public class DummyData
    {
        public async Task InitializeDummyDataAsync(UserManager<User> userManager, DataContext context)
        {
            var random = new Random();
            var roleList = new List<string> { "Admin", "User" };
            var userNames = new List<string>() { "John Doe", "Jane Doe" };
            var codeTypes = new List<string>() { "Batch", "C", "C#", "C++", "CoffeeScript", "CSS", "Diff", "Elm", "F#", "Go",
                                                 "Handlebars", "Haskell", "HTML", "Java", "JavaScript", "JSON", "Kotlin", "LESS", "Lua", "Markdown",
                                                 "MATLAB", "Objective-C", "Perl", "PHP", "Powershell", "Pug", "Python", "R", "Razor", "Ruby",
                                                 "Rust", "SASS", "Scala", "SCSS", "Shell", "Swift", "TypeScript", "Turbo Pascal", "VB", "XML" };

            var users = Enumerable.Range(1, 30).Select(i => new User
            {
                UserName = $"User{i}",
                Email = $"user{i}@example.com",
                DisplayName = $"{i}. example {userNames[random.Next(userNames.Count)]}",
                PhoneNumber = "123",
                TwoFactorEnabled = true,
                EmailConfirmed = true
            }).ToArray();

            foreach (var user in users)
            {
                var userResult = await userManager.CreateAsync(user, "Password");

                if (userResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");

                    var mfaUserResult = new Mfa
                    {
                        UserId = user.Id,
                        ReliableEmail = $"{user.UserName}@reliable.com",
                        SecondaryLoginMethod = true,
                    };

                    await context.MFADb!.AddAsync(mfaUserResult);
                }
            }

            var justRegisteredUser = new User
            {
                UserName = "JustRegisteredUser",
                Email = "user@justregistered.com",
                DisplayName = "Newcomer",
                PhoneNumber = "123456",
            };

            var justRegisteredUserResult = await userManager.CreateAsync(justRegisteredUser, "Password");

            if (justRegisteredUserResult.Succeeded)
            {
                await userManager.AddToRoleAsync(justRegisteredUser, "User");
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

            var cSupports = Enumerable.Range(1, 2).Select(i => new User
            {
                UserName = $"CusSupport{i}",
                Email = $"customersupport{i}@mycode.com",
                DisplayName = $"{i}. Support {userNames[random.Next(userNames.Count)]}",
                PhoneNumber = "12345678",
                EmailConfirmed = true
            }).ToArray();

            foreach (var cSupport in cSupports)
            {

                var cSupportResult = await userManager.CreateAsync(cSupport, "Password");

                if (cSupportResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(cSupport, "Support");
                }
            }

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
