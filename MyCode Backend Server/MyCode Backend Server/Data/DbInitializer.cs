using Microsoft.AspNetCore.Identity;
using MyCode_Backend_Server.Data.Service;
using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Data
{
     public class DbInitializer(IConfiguration configuration) : IDbInitializer
     {
        private readonly IConfiguration _configuration = configuration;

        public async Task Initialize(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, FAQBotData fAQBotData, DummyData dummyData)
        {
            context.Database.EnsureCreated();

            var roleList = new List<string> { "Admin", "User" };

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

            if (!context.BotDb!.Any(b => b.BotId != Guid.Empty))
            {
                await fAQBotData.InitializeFAQBBotDataAsync(context);
            }

            if (_configuration["InitDb"] == "False" || context.Users.Any(u => u.Email == "user1@example.com"))
            {
                context.SaveChanges();

                return;
            }

            await dummyData.InitializeDummyDataAsync(userManager, context);
        }

        private async Task CreateRole(RoleManager<IdentityRole<Guid>> roleManager, string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
     }
}
