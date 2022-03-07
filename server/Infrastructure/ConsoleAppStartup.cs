using System;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Infrastructure
{
    public static class ConsoleAppStartup
    {
        public static IServiceProvider BuildServiceProvider(Action<IServiceCollection> config, bool includeConsole = true)
        {
            SetupStaticLogger(includeConsole);

            var services = new ServiceCollection();

            config(services);

            services.AddLogging(logging => logging.AddSerilog());

            return services.BuildServiceProvider();
        }

        private static void SetupStaticLogger(bool includeConsole)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Conditional(@event => includeConsole, configuration => configuration.Console())
                .CreateLogger();
        }
    }
}