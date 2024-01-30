using Microsoft.AspNetCore.Identity;
using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Data
{
    public interface IDbInitializer
    {
        Task Initialize(DataContext context, UserManager<User> userManager);
    }
}
