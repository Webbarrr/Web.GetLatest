using Microsoft.Extensions.DependencyInjection;
using Web.GetLatest.Application.ConfigurationProviders;
using Web.GetLatest.Application.GetLatestRelease;

namespace Web.GetLatest.Application.DependencyWiring;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IGetLatestReleaseAppService, GetLatestReleaseAppService>();
        return services;
    }

    public static IServiceCollection AddConfigurationProvider(this IServiceCollection services)
    {
        services.AddScoped<IReleaseConfigurationProvider, ReleaseConfigurationProvider>();
        return services;
    }
}