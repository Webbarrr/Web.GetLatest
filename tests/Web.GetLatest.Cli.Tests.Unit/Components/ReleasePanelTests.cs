using FluentAssertions;
using Spectre.Console.Testing;
using Web.GetLatest.Application.Contracts.Infrastructure.GetLatestRelease;
using Web.GetLatest.Cli.Components;

namespace Web.GetLatest.Cli.Tests.Unit.Components;

public sealed class ReleasePanelTests
{
    [Fact]
    public void Render_DisplaysMarkupLikeReleaseMetadataAsLiteralText()
    {
        var response = new GitHubReleaseResponse
        {
            TagName = "[tag]v1.2.3[/]",
            Name = "[red]Breaking release[/]"
        };
        var panel = new ReleasePanel(response, "owner/[repo]");
        var console = new TestConsole();

        console.Write(panel);

        console.Output.Should().Contain("[tag]v1.2.3[/]");
        console.Output.Should().Contain("[red]Breaking release[/]");
        console.Output.Should().Contain("owner/[repo]");
    }

    [Fact]
    public void Render_DoesNotThrowForMarkupLikeReleaseMetadata()
    {
        var response = new GitHubReleaseResponse
        {
            TagName = "[",
            Name = "] [bold]"
        };
        var panel = new ReleasePanel(response, "owner/[repo]");
        var console = new TestConsole();

        Action render = () => console.Write(panel);

        render.Should().NotThrow();
    }
}