using Microsoft.AspNetCore.Identity;
using MyCode_Backend_Server.Data.Service;
using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Data
{
     public class DbInitializer : IDbInitializer
     {
        public async Task Initialize(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, FAQBotData fAQBotData, DummyData dummyData)
        {
            context.Database.EnsureCreated();

            var roleList = new List<string> { "Admin", "User", "Support" };

            foreach (var role in roleList)
            {
                await CreateRole(roleManager, role);
            }

            var pass = Environment.GetEnvironmentVariable("APass");
            var mail = Environment.GetEnvironmentVariable("AEmail");
            var phone = Environment.GetEnvironmentVariable("ACall");
            var uName = Environment.GetEnvironmentVariable("UName");
            var dName = Environment.GetEnvironmentVariable("AName");

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

            if (!context.BotDb!.Any(b => b.BotId != Guid.Empty))
            {
                await fAQBotData.InitializeFAQBBotDataAsync(context);
            }

            if (Environment.GetEnvironmentVariable("InitDb") == "False" || context.Users.Any(u => u.Email == "user1@example.com"))
            {
                context.SaveChanges();

                return;
            }

            await dummyData.InitializeDummyDataAsync(userManager, context);
        }

        private static async Task CreateRole(RoleManager<IdentityRole<Guid>> roleManager, string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
     }
}
