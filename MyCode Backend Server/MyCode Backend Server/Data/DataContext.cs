﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Data
{
    public class DataContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public DbSet<Code>? CodesDb { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DataContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Code)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId);
        }
    }
}
