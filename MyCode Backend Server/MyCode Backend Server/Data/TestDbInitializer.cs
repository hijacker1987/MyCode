using Microsoft.AspNetCore.Identity;
using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Data
{
    public class TestDbInitializer : IDbInitializer
    {
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

            var testAdminInDbExist = await userManager.FindByEmailAsync("admin@test.com");

            if (testAdminInDbExist == null)
            {
                var testAdmin = new User { UserName = "testAdmin", Email = "admin@test.com", DisplayName = "Test God", PhoneNumber = "010101" };
                var adminCreated = await userManager.CreateAsync(testAdmin, "AdminPassword");

                if (adminCreated.Succeeded)
                {
                    await userManager.AddToRoleAsync(testAdmin, "Admin");
                }
            }

            var testUserExist = await userManager.FindByEmailAsync("tester1@test.com");

            if (testUserExist == null)
            {
                var testUsers = Enumerable.Range(1, 10).Select(i => new User
                {
                    ReliableEmail = $"tester{i}@gmail.com",
                    UserName = $"Tester{i}",
                    Email = $"tester{i}@test.com",
                    DisplayName = $"{i}. test {userNames[random.Next(userNames.Count)]}",
                    PhoneNumber = "123"
                }).ToArray();

                foreach (var testUser in testUsers)
                {
                    var userResult = await userManager.CreateAsync(testUser, "Password");

                    if (userResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(testUser, "User");
                    }
                }
                context.SaveChanges();

                foreach (var testUser in testUsers)
                {
                    int numberOfCodes = random.Next(1, 6);

                    for (int i = 1; i <= numberOfCodes; i++)
                    {
                        var code = new Code
                        {
                            CodeTitle = $"Test Code of {testUser.UserName} - {i}",
                            MyCode = $"TestCodeContent{i}",
                            WhatKindOfCode = $"{codeTypes[random.Next(codeTypes.Count)]}",
                            IsBackend = random.Next(2) == 0,
                            IsVisible = random.Next(3) == 0,
                            UserId = testUser.Id
                        };

                        context.CodesDb!.Add(code);
                    }
                }
                context.SaveChanges();
            }
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
