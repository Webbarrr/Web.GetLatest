using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using Web.GetLatest.Application.ConfigurationProviders;
using Web.GetLatest.Application.Contracts.Infrastructure.GetLatestRelease;
using Web.GetLatest.Application.GetLatestRelease;
using Web.GetLatest.Cli.Components;
using Web.GetLatest.Cli.Enums;

namespace Web.GetLatest.Cli.Commands;

public sealed class GetLatestReleaseCommand(
    IAnsiConsole console,
    IGetLatestReleaseAppService getLatestReleaseAppService,
    IReleaseConfigurationProvider releaseConfigurationProvider
    ) : AsyncCommandBase<GetLatestReleaseCommand.Settings>
{
    public sealed class Settings : CommandSettingsBase
    {
        [CommandOption("-o|--owner <OWNER>")]
        [Description("The owner of the repository.")]
        public string? Owner { get; set; }

        [CommandOption("-r|--repo <REPO>")]
        [Description("The name of the repository.")]
        public string? Repo { get; set; }

        [CommandOption("-p|--pat <PAT>")]
        [Description("Your GitHub Personal Access Token.")]
        public string? Pat { get; set; }

        [CommandOption("-v|--version <VERSION_PREFIX>")]
        [Description("Version prefix to filter by (for example: 2.5).")]
        public string? Version { get; set; }
    }

    private sealed record UserConfig(string? Owner, string? Repo);

    protected override async Task<int> ExecuteCommandAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var request = releaseConfigurationProvider.ResolveConfiguration(new GetLatestReleaseResolverRequest
        {
            Owner = settings.Owner,
            Repo = settings.Repo,
            Pat = settings.Pat
        });

        string targetRepo = $"{request.Owner}/{request.Repo}";

        console.MarkupLine($"[grey]Resolved Config --> Repo: {targetRepo} | PAT: {(string.IsNullOrEmpty(request.Pat) ? "(none)" : "****")}[/]");
        console.WriteLine();

        GitHubReleaseResponse? response = null;

        await console.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync(GetStatusMessage(targetRepo, settings.Version), async _ =>
            {
                response = string.IsNullOrWhiteSpace(settings.Version)
                    ? await getLatestReleaseAppService.GetLatestReleaseAsync(request, cancellationToken)
                    : await getLatestReleaseAppService.GetLatestReleaseByVersionAsync(request, settings.Version, cancellationToken);
            });

        if (response is null)
        {
            console.MarkupLine($"[yellow]No release information found for {targetRepo}.[/]");
            return (int)CliStatusCodes.Success;
        }

        var panel = new ReleasePanel(response, targetRepo);
        console.WriteLine();
        console.Write(panel);
        console.WriteLine();

        return (int)CliStatusCodes.Success;
    }

    private static string GetStatusMessage(string targetRepo, string? versionPrefix)
    {
        string escapedTargetRepo = Markup.Escape(targetRepo);

        if (string.IsNullOrWhiteSpace(versionPrefix))
        {
            return $"Fetching latest release for [cyan]{escapedTargetRepo}[/]...";
        }

        return $"Fetching latest release for [cyan]{escapedTargetRepo}[/] matching [yellow]v{Markup.Escape(versionPrefix.Trim().TrimStart('v', 'V'))}.*[/]...";
    }
}