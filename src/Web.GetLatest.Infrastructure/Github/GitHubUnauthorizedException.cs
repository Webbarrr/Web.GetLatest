namespace Web.GetLatest.Infrastructure.Github;

public sealed class GitHubUnauthorizedException(string message) : Exception(message)
{
}