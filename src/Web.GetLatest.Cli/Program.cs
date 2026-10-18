using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using Web.GetLatest.Application.DependencyWiring;
using Web.GetLatest.Cli.Commands;
using Web.GetLatest.Cli.DependencyWiring;
using Web.GetLatest.Cli.Enums;
using Web.GetLatest.Infrastructure.DependencyWiring;
using Web.GetLatest.Infrastructure.Github;

namespace Web.GetLatest.Cli;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        var trace = args.Any(a => string.Equals(a, "-t", StringComparison.OrdinalIgnoreCase)
                         || string.Equals(a, "--trace", StringComparison.OrdinalIgnoreCase));

        using var cancellationSource = new CancellationTokenSource();
        void cancelKeyPressHandler(object? _, ConsoleCancelEventArgs e)
        {
            if (!cancellationSource.IsCancellationRequested)
            {
                e.Cancel = true;
                cancellationSource.Cancel();
            }
            else
            {
                e.Cancel = false;
            }
        }
        Console.CancelKeyPress += cancelKeyPressHandler;

        try
        {
            var services = new ServiceCollection();

            var registrar = new TypeRegistrar(services);
            var app = new CommandApp(registrar);

            app.SetDefaultCommand<GetLatestReleaseCommand>();
            app.Configure(config =>
            {
                services.RegisterLogging(trace);
                services.AddGithubClient();
                services.AddLocalUserConfigRepository();

                services.AddAppServices();
                services.AddConfigurationProvider();

                config.SetApplicationName("ghver");
                config.PropagateExceptions();
                config.ConfigureCommands();
            });

            return await app.RunAsync(args, cancellationSource.Token);
        }
        catch (OperationCanceledException)
        {
            AnsiConsole.MarkupLine("\n[yellow]Operation aborted by user.[/]");
            return CliStatusCodes.OperationCancelled.ToExitCode();
        }
        catch (Exception ex)
        {
            return HandleException(ex, trace);
        }
        finally
        {
            Console.CancelKeyPress -= cancelKeyPressHandler;
        }
    }

    private static int HandleException(Exception ex, bool trace)
    {
        switch (ex)
        {
            case RateLimitExceededException rle:
                if (trace)
                {
                    AnsiConsole.WriteException(rle, ExceptionFormats.Default);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[red]Rate limit exceeded.[/] The GitHub API limit resets at {Markup.Escape(rle.ResetTimeUtc.ToString("u"))} UTC.");
                    AnsiConsole.MarkupLine("[grey]Tip: Wait until the reset time or use credentials / token with higher limits.[/]");
                }

                return CliStatusCodes.RateLimited.ToExitCode();

            case GitHubNotFoundException gnf:
                if (trace)
                {
                    AnsiConsole.WriteException(gnf, ExceptionFormats.Default);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[yellow]Not found:[/] {Markup.Escape(gnf.Message)}");
                }

                return CliStatusCodes.NotFound.ToExitCode();

            case GitHubUnauthorizedException gu:
                if (trace)
                {
                    AnsiConsole.WriteException(gu, ExceptionFormats.Default);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[red]Unauthorized:[/] {Markup.Escape(gu.Message)}");
                    AnsiConsole.MarkupLine("[grey]Tip: Provide a valid personal access token (PAT) via -p or GHVER_PAT.[/]");
                }

                return CliStatusCodes.Unauthorized.ToExitCode();

            default:
                if (trace)
                {
                    AnsiConsole.WriteException(ex, ExceptionFormats.Default);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[red]Error:[/] {Markup.Escape(ex.Message)}");
                    AnsiConsole.MarkupLine("[grey]Tip: Re-run with --trace for full diagnostics.[/]");
                }

                return CliStatusCodes.Failure.ToExitCode();
        }
    }
}