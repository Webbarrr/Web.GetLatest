using System.Text.Json.Serialization;

namespace Web.GetLatest.Infrastructure.Github;

public sealed class GitHubReleaseResponseDto
{
    [JsonPropertyName("tag_name")]
    public string TagName { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("published_at")]
    public DateTime PublishedAt { get; set; }
}