using Microsoft.Extensions.Logging;

namespace Web.GetLatest.Cli.Logging;

public sealed class LoggingState
{
    public LogLevel MinimumLevel { get; set; } = LogLevel.Warning;
}