using Web.GetLatest.Application.Contracts.Infrastructure.UserConfig;

namespace Web.GetLatest.Infrastructure.UserConfig;

public sealed class EnvironmentRepository : IEnvironmentRepository
{
    public string? GetOwner()
    {
        return Environment.GetEnvironmentVariable("GHVER_OWNER");
    }

    public string? GetRepo()
    {
        return Environment.GetEnvironmentVariable("GHVER_REPO");
    }

    public string? GetPersonalAccessToken()
    {
        return Environment.GetEnvironmentVariable("GHVER_PAT");
    }
}