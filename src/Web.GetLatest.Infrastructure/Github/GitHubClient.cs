using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Web.GetLatest.Application.Contracts.Infrastructure.GetLatestRelease;

namespace Web.GetLatest.Infrastructure.Github;

public sealed class GitHubClient(HttpClient httpClient) : IGitHubClient
{
    public async Task<GitHubReleaseResponse> GetLatestReleaseAsync(GetLatestReleaseRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateRepositoryRequest(request.Owner, request.Repo);

        var relativeUrl = $"repos/{request.Owner}/{request.Repo}/releases/latest";

        using var httpRequest = CreateRequest(HttpMethod.Get, relativeUrl, request.Pat);
        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);

        await EnsureSuccessOrThrowAsync(response, request.Owner, request.Repo, cancellationToken);

        var releaseDto = await response.Content.ReadFromJsonAsync<GitHubReleaseResponseDto>(cancellationToken);

        return releaseDto is null
            ? throw new InvalidOperationException("Failed to deserialize the GitHub release response.")
            : MapRelease(releaseDto);
    }

    public async Task<IReadOnlyList<GitHubReleaseResponse>> GetReleasesAsync(GetPagedReleasesRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateRepositoryRequest(request.Owner, request.Repo);
        ValidatePaging(request.PageNumber, request.PageSize);

        var relativeUrl = $"repos/{request.Owner}/{request.Repo}/releases?page={request.PageNumber}&per_page={request.PageSize}";

        using var httpRequest = CreateRequest(HttpMethod.Get, relativeUrl, request.Pat);
        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);

        await EnsureSuccessOrThrowAsync(response, request.Owner, request.Repo, cancellationToken);

        var releaseDtos = await response.Content.ReadFromJsonAsync<List<GitHubReleaseResponseDto>>(cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize the GitHub releases response.");

        return releaseDtos.Select(MapRelease).ToList();
    }

    private static void ValidateRepositoryRequest(string owner, string repo)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(owner);
        ArgumentException.ThrowIfNullOrWhiteSpace(repo);
    }

    private static void ValidatePaging(int pageNumber, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageNumber);

        if (pageSize is <= 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "Page size must be between 1 and 100.");
        }
    }

    private static HttpRequestMessage CreateRequest(HttpMethod method, string relativeUrl, string? pat)
    {
        var httpRequest = new HttpRequestMessage(method, relativeUrl);

        if (!string.IsNullOrWhiteSpace(pat))
        {
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", pat);
        }

        return httpRequest;
    }

    private static async Task EnsureSuccessOrThrowAsync(
        HttpResponseMessage response,
        string owner,
        string repo,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new GitHubNotFoundException($"Repository {owner}/{repo} not found. Details: {errorContent}");
        }

        if (IsRateLimited(response))
        {
            throw new RateLimitExceededException(GetRateLimitResetUtc(response));
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new GitHubUnauthorizedException($"Unauthorized access to {owner}/{repo}. Status: {response.StatusCode}. Details: {errorContent}");
        }

        throw new HttpRequestException($"GitHub API request failed with status code {response.StatusCode}: {errorContent}");
    }

    private static bool IsRateLimited(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            return true;
        }

        if (response.StatusCode == HttpStatusCode.Forbidden &&
            response.Headers.TryGetValues("X-RateLimit-Remaining", out var remainingValues) &&
            remainingValues.FirstOrDefault() == "0")
        {
            return true;
        }

        return response.Headers.Contains("Retry-After");
    }

    private static DateTime GetRateLimitResetUtc(HttpResponseMessage response)
    {
        var resetUtc = DateTime.UtcNow; // TODO; use TimeProvider

        if (response.Headers.TryGetValues("X-RateLimit-Reset", out var resetValues) &&
            long.TryParse(resetValues.FirstOrDefault(), out var unixEpoch))
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixEpoch).UtcDateTime;
        }

        if (response.Headers.TryGetValues("Retry-After", out var retryValues) &&
            int.TryParse(retryValues.FirstOrDefault(), out var retrySeconds))
        {
            return resetUtc.AddSeconds(retrySeconds);
        }

        return resetUtc;
    }

    private static GitHubReleaseResponse MapRelease(GitHubReleaseResponseDto releaseDto) => new()
    {
        TagName = releaseDto.TagName,
        Name = releaseDto.Name,
        PublishedAt = releaseDto.PublishedAt
    };
}