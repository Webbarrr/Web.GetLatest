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

        string owner = FirstNonBlank(
            request.Owner,
            environmentRepository.GetOwner(),
            userConfig?.Owner,
            _defaultOwner)!;

        string repo = FirstNonBlank(
            request.Repo,
            environmentRepository.GetRepo(),
            userConfig?.Repo,
            _defaultRepo)!;

        string? pat = FirstNonBlank(
            request.Pat,
            environmentRepository.GetPersonalAccessToken());

        return new GetLatestReleaseRequest
        {
            Owner = owner,
            Repo = repo,
            Pat = pat
        };
    }

    private static string? FirstNonBlank(params string?[] values)
    {
        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
    }
}