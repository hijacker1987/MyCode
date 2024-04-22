using Microsoft.AspNetCore.Identity;
using MyCode_Backend_Server.Data.Service;
using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Data
{
    public interface IDbInitializer
    {
        Task Initialize(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, FAQBotData fAQBotData, DummyData dummyData);
    }
}
