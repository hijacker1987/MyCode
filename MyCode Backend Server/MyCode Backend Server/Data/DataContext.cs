using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
    {
        public DbSet<Code>? CodesDb { get; set; }
        public DbSet<Mfa>? MFADb { get; set; }
        public DbSet<BotModel>? BotDb { get; set; }
        public DbSet<SupportChat>? SupportDb { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
