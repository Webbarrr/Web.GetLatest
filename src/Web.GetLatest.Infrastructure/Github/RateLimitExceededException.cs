namespace Web.GetLatest.Infrastructure.Github;

public sealed class RateLimitExceededException(DateTime resetTimeUtc) : Exception($"GitHub rate limit exceeded. Resets at {resetTimeUtc:u} UTC.")
{
    public DateTime ResetTimeUtc { get; } = resetTimeUtc;
}