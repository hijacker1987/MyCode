using DotNetEnv;

namespace MyCode_Backend_Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load();

            var builder = CreateHostBuilder(args);

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
    }
}
