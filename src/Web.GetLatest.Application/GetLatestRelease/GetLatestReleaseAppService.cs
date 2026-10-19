using Web.GetLatest.Application.Contracts.Infrastructure.GetLatestRelease;

namespace Web.GetLatest.Application.GetLatestRelease;

public interface IGetLatestReleaseAppService
{
    Task<GitHubReleaseResponse> GetLatestReleaseAsync(GetLatestReleaseRequest request, CancellationToken cancellationToken = default);

    Task<GitHubReleaseResponse?> GetLatestReleaseByVersionAsync(
        GetLatestReleaseRequest request,
        string versionPrefix,
        CancellationToken cancellationToken = default);
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

    public async Task<GitHubReleaseResponse?> GetLatestReleaseByVersionAsync(
        GetLatestReleaseRequest request,
        string versionPrefix,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Owner);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Repo);
        ArgumentException.ThrowIfNullOrWhiteSpace(versionPrefix);

        var normalizedVersionPrefix = versionPrefix.Trim().TrimStart('v', 'V');
        var tagPrefix = $"v{normalizedVersionPrefix}.";

        for (var pageNumber = 1; ; pageNumber++)
        {
            var clientRequest = new GetPagedReleasesRequest
            {
                Owner = request.Owner,
                Repo = request.Repo,
                Pat = request.Pat,
                PageNumber = pageNumber,
                PageSize = GetPagedReleasesRequest.DefaultPageSize,
            };

            var releases = await gitHubClient.GetReleasesAsync(clientRequest, cancellationToken);
            if (releases.Count == 0)
            {
                return null;
            }

            var matchedRelease = releases
                .Where(r => r.TagName.StartsWith(tagPrefix, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(r => r.PublishedAt)
                .FirstOrDefault();

            if (matchedRelease is not null)
            {
                return matchedRelease;
            }

            if (releases.Count < GetPagedReleasesRequest.DefaultPageSize)
            {
                return null;
            }
        }
    }
}