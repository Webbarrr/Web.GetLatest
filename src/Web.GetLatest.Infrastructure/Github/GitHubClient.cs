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
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Owner);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Repo);

        var relativeUrl = $"repos/{request.Owner}/{request.Repo}/releases/latest";

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, relativeUrl);

        if (!string.IsNullOrWhiteSpace(request.Pat))
        {
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.Pat);
        }

        var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new GitHubNotFoundException($"Repository {request.Owner}/{request.Repo} not found. Details: {errorContent}");
            }

            bool isRateLimited = response.StatusCode == HttpStatusCode.TooManyRequests;
            if (!isRateLimited && response.StatusCode == HttpStatusCode.Forbidden)
            {
                if (response.Headers.TryGetValues("X-RateLimit-Remaining", out var remVals) &&
                    remVals.FirstOrDefault() == "0")
                {
                    isRateLimited = true;
                }
            }
            if (!isRateLimited && response.Headers.Contains("Retry-After"))
            {
                isRateLimited = true;
            }

            if (isRateLimited)
            {
                DateTime resetUtc = DateTime.UtcNow; // TODO; use TimeProvider

                if (response.Headers.TryGetValues("X-RateLimit-Reset", out var resetVals) &&
                    long.TryParse(resetVals.FirstOrDefault(), out var unixEpoch))
                {
                    resetUtc = DateTimeOffset.FromUnixTimeSeconds(unixEpoch).UtcDateTime;
                }
                else if (response.Headers.TryGetValues("Retry-After", out var retryVals) &&
                         int.TryParse(retryVals.FirstOrDefault(), out var retrySeconds))
                {
                    resetUtc = DateTime.UtcNow.AddSeconds(retrySeconds);
                }

                throw new RateLimitExceededException(resetUtc);
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new GitHubUnauthorizedException($"Unauthorized access to {request.Owner}/{request.Repo}. Status: {response.StatusCode}. Details: {errorContent}");
            }

            throw new HttpRequestException($"GitHub API request failed with status code {response.StatusCode}: {errorContent}");
        }

        var releaseDto = await response.Content.ReadFromJsonAsync<GitHubReleaseResponseDto>(cancellationToken);

        return releaseDto is null
            ? throw new InvalidOperationException("Failed to deserialize the GitHub release response.")
            : new GitHubReleaseResponse
            {
                TagName = releaseDto.TagName,
                Name = releaseDto.Name,
                PublishedAt = releaseDto.PublishedAt
            };
    }
}