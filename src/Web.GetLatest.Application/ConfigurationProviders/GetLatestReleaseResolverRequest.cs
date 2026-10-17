namespace Web.GetLatest.Application.ConfigurationProviders;

public sealed record GetLatestReleaseResolverRequest
{
    public string? Owner { get; init; }
    public string? Repo { get; init; }
    public string? Pat { get; init; }
}
