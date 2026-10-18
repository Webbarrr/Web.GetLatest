using Web.GetLatest.Application.Contracts.Infrastructure.GetLatestRelease;

namespace Web.GetLatest.Application.GetLatestRelease;

public interface IGetLatestReleaseAppService
{
    Task<GitHubReleaseResponse> GetLatestReleaseAsync(GetLatestReleaseRequest request, CancellationToken cancellationToken = default);
}

public sealed class GetLatestReleaseAppService(IGitHubClient gitHubClient) : IGetLatestReleaseAppService
{
    public async Task<GitHubReleaseResponse> GetLatestReleaseAsync(GetLatestReleaseRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Owner);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Repo);

        return await gitHubClient.GetLatestReleaseAsync(request, cancellationToken);
    }
}