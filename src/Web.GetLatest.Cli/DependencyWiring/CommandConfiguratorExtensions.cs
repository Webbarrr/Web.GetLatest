using Spectre.Console.Cli;
using Web.GetLatest.Cli.Commands;

namespace Web.GetLatest.Cli.DependencyWiring;

public static class CommandConfiguratorExtensions
{
    public static void ConfigureCommands(this IConfigurator configurator)
    {
        configurator.RegisterGetLatestReleaseCommand();
    }

    private static void RegisterGetLatestReleaseCommand(this IConfigurator configurator)
    {
        string[] examples =
        [
            "release",
            "release --owner Webbarrr --repo Web.GetLatest",
            "release -o Webbarrr -r Web.GetLatest",
            "release -p ghp_abc123xyz"
        ];

        var command = configurator.AddCommand<GetLatestReleaseCommand>("release")
            .WithDescription("Fetches and displays the latest release information for a GitHub repository.");

        foreach (var example in examples)
        {
            command.WithExample(example);
        }
    }
}