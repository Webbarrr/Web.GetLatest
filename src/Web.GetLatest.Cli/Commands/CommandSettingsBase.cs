using Spectre.Console.Cli;

namespace Web.GetLatest.Cli.Commands;

public abstract class CommandSettingsBase : CommandSettings
{
    /// <summary>
    /// Enabled trace logging output.
    /// </summary>
    [CommandOption("-t|--trace")]
    public bool Trace { get; init; }
}