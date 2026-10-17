namespace Web.GetLatest.Application.Contracts.Infrastructure.GetLatestRelease;

public sealed record GetLatestReleaseRequest
{
    public string Owner { get; init; } = string.Empty;
    public string Repo { get; init; } = string.Empty;
    public string? Pat { get; init; }
}