using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Data
{
    public class DataContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DataContext() : base(new DbContextOptionsBuilder<DataContext>().Options) { }

        public virtual DbSet<Code>? CodesDb { get; set; }
        public virtual DbSet<Mfa>? MFADb { get; set; }
        public virtual DbSet<BotModel>? BotDb { get; set; }
        public virtual DbSet<SupportChat>? SupportDb { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SupportChat>().Property<byte[]>("VersionForOptimisticLocking").IsRowVersion();
        }
    }
}
