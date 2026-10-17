namespace Web.GetLatest.Application.Contracts.Infrastructure.GetLatestRelease;

public sealed record GitHubReleaseResponse
{
    public string TagName { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public DateTime PublishedAt { get; init; }
}