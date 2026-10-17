namespace Web.GetLatest.Infrastructure.Github;

public sealed class GitHubNotFoundException(string message) : Exception(message)
{
}