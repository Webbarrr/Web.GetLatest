namespace Web.GetLatest.Application.Contracts.Infrastructure.UserConfig;

public record UserConfigResponse
{
    public string? Owner { get; init; }
    public string? Repo { get; init; }
}