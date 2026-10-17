using Web.GetLatest.Application.Contracts.Infrastructure.GetLatestRelease;
using Web.GetLatest.Application.Contracts.Infrastructure.UserConfig;

namespace Web.GetLatest.Application.ConfigurationProviders;

public class ReleaseConfigurationProvider(IUserConfigRepository configRepository) : IReleaseConfigurationProvider
{
    private const string _defaultOwner = "Webbarrr";
    private const string _defaultRepo = "Web.GetLatest";

    public GetLatestReleaseRequest ResolveConfiguration(GetLatestReleaseResolverRequest request)
    {
        var userConfig = configRepository.LoadConfig();

        string owner = request.Owner
                       ?? userConfig?.Owner
                       ?? Environment.GetEnvironmentVariable("GHVER_OWNER")
                       ?? _defaultOwner;

        string repo = request.Repo
                      ?? userConfig?.Repo
                      ?? Environment.GetEnvironmentVariable("GHVER_REPO")
                      ?? _defaultRepo;

        string? pat = request.Pat
                      ?? Environment.GetEnvironmentVariable("GHVER_PAT");

        return new GetLatestReleaseRequest
        {
            Owner = owner,
            Repo = repo,
            Pat = pat
        };
    }
}