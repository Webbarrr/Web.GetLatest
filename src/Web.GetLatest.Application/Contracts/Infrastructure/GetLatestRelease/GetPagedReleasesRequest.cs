namespace Web.GetLatest.Application.Contracts.Infrastructure.GetLatestRelease;

public sealed record GetPagedReleasesRequest
{
    public const int DefaultPageSize = 30;

    public string Owner { get; init; } = string.Empty;
    public string Repo { get; init; } = string.Empty;
    public string? Pat { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = DefaultPageSize;
}
