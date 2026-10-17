using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using Web.GetLatest.Application.Contracts.Infrastructure.GetLatestRelease;
using Web.GetLatest.Application.Contracts.Infrastructure.UserConfig;
using Web.GetLatest.Infrastructure.Github;
using Web.GetLatest.Infrastructure.UserConfig;

namespace Web.GetLatest.Infrastructure.DependencyWiring;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubClient(this IServiceCollection services)
    {
        services.AddHttpClient<IGitHubClient, GitHubClient>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("https://api.github.com/");

                client.DefaultRequestHeaders.UserAgent.Add(
                    new ProductInfoHeaderValue(ClientConsts.ProductInfoHeaderValue, ClientConsts.ProductInfoHeaderVersion));

                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(ClientConsts.MediaTypeWithQualityHeaderValue));

                client.DefaultRequestHeaders.Add(
                    ClientConsts.ApiVersionHeaderName, ClientConsts.ApiVersionHeaderValue);
            });

        return services;
    }

    public static IServiceCollection AddLocalUserConfigRepository(this IServiceCollection services)
    {
        services.AddSingleton<IUserConfigRepository, LocalUserConfigRepository>();
        return services;
    }
}