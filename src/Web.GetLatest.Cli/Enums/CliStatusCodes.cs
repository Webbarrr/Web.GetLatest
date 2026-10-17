namespace Web.GetLatest.Cli.Enums;

public enum CliStatusCodes
{
    /// <summary>
    /// The command executed successfully without errors.
    /// </summary>
    Success = 0,

    /// <summary>
    /// A catch-all for general errors (unhandled exceptions, unknown failures).
    /// </summary>
    Failure = 1,

    /// <summary>
    /// The user provided invalid, missing, or malformed arguments/options.
    /// </summary>
    InvalidArgument = 2,

    /// <summary>
    /// The operation was cancelled by the user (e.g., Ctrl+C).
    /// </summary>
    OperationCancelled = 3,

    /// <summary>
    /// A required external resource, file, or API dependency was not found.
    /// </summary>
    NotFound = 4,

    /// <summary>
    /// The user lacks the necessary permissions or API keys to execute the command.
    /// </summary>
    Unauthorized = 5,

    /// <summary>
    /// An external dependency or network request timed out.
    /// </summary>
    Timeout = 6,

    /// <summary>
    /// The command failed because an external API rate limit was exceeded.
    /// </summary>
    RateLimited = 7
}

public static class CliStatusCodesExtensions
{
    public static int ToExitCode(this CliStatusCodes statusCode)
    {
        return (int)statusCode;
    }
}