﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyCode_Backend_Server.Data;

namespace MyCode_Backend_Server_Tests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DataContext>));

                services.Remove(dbContextDescriptor!);

                services.AddDbContext<DataContext>((options) =>
                {
                    options.UseInMemoryDatabase("testDatabase");
                });
            });
            builder.UseEnvironment("Development");
        }
    }
}
