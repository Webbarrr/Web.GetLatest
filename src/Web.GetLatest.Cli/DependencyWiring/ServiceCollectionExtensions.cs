using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Web.GetLatest.Cli.Logging;

namespace Web.GetLatest.Cli.DependencyWiring;

public static class ServiceCollectionExtensions
{
    public static void RegisterLogging(this IServiceCollection services, bool trace)
    {
        var loggingState = new LoggingState
        {
            MinimumLevel = trace ? LogLevel.Trace : LogLevel.Warning
        };

        services.AddSingleton(loggingState);

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddFilter((_, level) => level >= loggingState.MinimumLevel);
        });
    }
}