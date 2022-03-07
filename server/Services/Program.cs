using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Services
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((hostingContext, loggerConfiguration) =>
                    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
                )
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((env, builder) =>
                    {
                        builder.SetBasePath(env.HostingEnvironment.ContentRootPath)
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables();
                    }).UseStartup<Startup>();
                });
    }
}
