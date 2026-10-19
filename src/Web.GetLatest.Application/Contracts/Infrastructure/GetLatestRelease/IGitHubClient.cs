namespace Web.GetLatest.Application.Contracts.Infrastructure.GetLatestRelease;

public interface IGitHubClient
{
    /// <summary>
    /// Gets the latest release information from a GitHub repository.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<GitHubReleaseResponse> GetLatestReleaseAsync(GetLatestReleaseRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged list of releases from a GitHub repository.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<GitHubReleaseResponse>> GetReleasesAsync(GetPagedReleasesRequest request, CancellationToken cancellationToken = default);
}