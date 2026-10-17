using Spectre.Console.Cli;

namespace Web.GetLatest.Cli.Commands;

public abstract class AsyncCommandBase<TSettings> : AsyncCommand<TSettings> where TSettings : CommandSettingsBase
{
    public override async Task<int> ExecuteAsync(CommandContext context, TSettings settings, CancellationToken cancellationToken)
    {
        return await ExecuteCommandAsync(context, settings, cancellationToken);
    }

    protected abstract Task<int> ExecuteCommandAsync(CommandContext context, TSettings settings, CancellationToken cancellationToken);
}