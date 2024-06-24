using DotNetEnv;

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
                    webBuilder.UseStartup<Startup>()
                        .UseKestrel(options =>
                        {
                            options.ListenAnyIP(443, listenOptions =>
                            {
                                string certPath = "/https/mycode.pfx";
                                string certPassword = "mycodessl";
                                if (!File.Exists(certPath))
                                {
                                    Console.WriteLine($"Certificate file not found at path: {certPath}");
                                }
                                else
                                {
                                    listenOptions.UseHttps(certPath, certPassword);
                                }
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
