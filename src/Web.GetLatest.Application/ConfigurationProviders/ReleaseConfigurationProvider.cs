using Web.GetLatest.Application.Contracts.Infrastructure.GetLatestRelease;
using Web.GetLatest.Application.Contracts.Infrastructure.UserConfig;

namespace Web.GetLatest.Application.ConfigurationProviders;

public sealed class ReleaseConfigurationProvider(
    IUserConfigRepository configRepository,
    IEnvironmentRepository environmentRepository
    ) : IReleaseConfigurationProvider
{
    private const string _defaultOwner = "Webbarrr";
    private const string _defaultRepo = "Web.GetLatest";

    public GetLatestReleaseRequest ResolveConfiguration(GetLatestReleaseResolverRequest request)
    {
        var userConfig = configRepository.LoadConfig();

        string owner = request.Owner
                       ?? userConfig?.Owner
                       ?? environmentRepository.GetOwner()
                       ?? _defaultOwner;

        string repo = request.Repo
                      ?? userConfig?.Repo
                      ?? environmentRepository.GetRepo()
                      ?? _defaultRepo;

        string? pat = request.Pat
                      ?? environmentRepository.GetPersonalAccessToken();

        return new GetLatestReleaseRequest
        {
            Owner = owner,
            Repo = repo,
            Pat = pat
        };
    }
}