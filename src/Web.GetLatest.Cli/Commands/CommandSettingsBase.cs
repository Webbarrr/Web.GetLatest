using Spectre.Console.Cli;

namespace Web.GetLatest.Cli.Commands;

public abstract class CommandSettingsBase : CommandSettings
{
    /// <summary>
    /// Enabled verbose logging output.
    /// </summary>
    [CommandOption("-v|--verbose")]
    public bool Verbose { get; init; }
}