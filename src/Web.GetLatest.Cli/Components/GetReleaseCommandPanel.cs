using Spectre.Console;
using Spectre.Console.Rendering;
using Web.GetLatest.Application.Contracts.Infrastructure.GetLatestRelease;

namespace Web.GetLatest.Cli.Components;

internal sealed class ReleasePanel : IRenderable
{
    private readonly GitHubReleaseResponse _response;
    private readonly string _targetRepo;

    public ReleasePanel(GitHubReleaseResponse response, string targetRepo)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetRepo);

        _response = response;
        _targetRepo = targetRepo;
    }

    private Panel CreatePanel()
    {
        string formattedDate = _response.PublishedAt.ToString("yyyy-MM-dd HH:mm");

        var panelContent = new Markup(
            $"[bold green]Version (Tag):[/] {_response.TagName}\n" +
            $"[bold]Release Name:[/] {_response.Name}\n" +
            $"[bold grey]Published At:[/] {formattedDate}"
        );

        return new Panel(panelContent)
        {
            Header = new PanelHeader($"[bold skyblue1] {_targetRepo} [/]"),
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.SkyBlue1),
            Padding = new Padding(2, 1, 2, 1)
        };
    }

    public Measurement Measure(RenderOptions options, int maxWidth)
    {
        var panel = CreatePanel();
        return ((IRenderable)panel).Measure(options, maxWidth);
    }

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var panel = CreatePanel();
        return ((IRenderable)panel).Render(options, maxWidth);
    }
}