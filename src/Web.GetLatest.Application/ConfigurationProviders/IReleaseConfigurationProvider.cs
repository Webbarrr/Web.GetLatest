using Web.GetLatest.Application.Contracts.Infrastructure.GetLatestRelease;

namespace Web.GetLatest.Application.ConfigurationProviders;

public interface IReleaseConfigurationProvider
{
    GetLatestReleaseRequest ResolveConfiguration(GetLatestReleaseResolverRequest request);
}