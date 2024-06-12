using DotNetEnv;
using System.Net;

namespace MyCode_Backend_Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load();

            var builder = CreateHostBuilder(args);

            if (IsDockerEnvironment())
            {
                builder.ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("https://*:443");
                    webBuilder.UseKestrel(options =>
                    {
                        options.Listen(IPAddress.Any, 8081, listenOptions =>
                        {
                            listenOptions.UseHttps("/etc/ssl/certs/MyCode Backend Server.pfx");
                        });
                    });
                });
            }

            builder.Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables();
                });

        private static bool IsDockerEnvironment()
        {
            return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOCKER_CONTAINER"));
        }
    }
}
