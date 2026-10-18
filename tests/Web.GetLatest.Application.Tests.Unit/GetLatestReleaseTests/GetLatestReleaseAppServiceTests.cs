using FakeItEasy;
using FluentAssertions;
using Web.GetLatest.Application.Contracts.Infrastructure.GetLatestRelease;
using Web.GetLatest.Application.GetLatestRelease;

namespace Web.GetLatest.Application.Tests.Unit.GetLatestReleaseTests;

public sealed class GetLatestReleaseAppServiceTests
{
    [Fact]
    public async Task GetLatestReleaseAsync_ReturnsResponse_FromGitHubClient()
    {
        // Arrange
        var githubClient = A.Fake<IGitHubClient>();
        var expected = new GitHubReleaseResponse { TagName = "v1.2.3", Name = "Release", PublishedAt = DateTime.UtcNow };
        A.CallTo(() => githubClient.GetLatestReleaseAsync(A<GetLatestReleaseRequest>._, A<CancellationToken>._))
            .Returns(Task.FromResult(expected));

        var sut = new GetLatestReleaseAppService(githubClient);

        var request = new GetLatestReleaseRequest { Owner = "owner", Repo = "repo" };

        // Act
        var result = await sut.GetLatestReleaseAsync(request, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetLatestReleaseAsync_Throws_OnNullRequest()
    {
        // Arrange
        var githubClient = A.Fake<IGitHubClient>();
        var sut = new GetLatestReleaseAppService(githubClient);

        // Act
        Func<Task> act = async () => await sut.GetLatestReleaseAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetLatestReleaseAsync_Throws_OnMissingOwner()
    {
        // Arrange
        var githubClient = A.Fake<IGitHubClient>();
        var sut = new GetLatestReleaseAppService(githubClient);
        var request = new GetLatestReleaseRequest { Owner = "", Repo = "repo" };

        // Act
        Func<Task> act = async () => await sut.GetLatestReleaseAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetLatestReleaseAsync_Throws_OnMissingRepo()
    {
        // Arrange
        var githubClient = A.Fake<IGitHubClient>();
        var sut = new GetLatestReleaseAppService(githubClient);
        var request = new GetLatestReleaseRequest { Owner = "owner", Repo = "" };

        // Act
        Func<Task> act = async () => await sut.GetLatestReleaseAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}