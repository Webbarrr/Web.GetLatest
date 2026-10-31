using FakeItEasy;
using FluentAssertions;
using Web.GetLatest.Application.ConfigurationProviders;

using Web.GetLatest.Application.Contracts.Infrastructure.UserConfig;

namespace Web.GetLatest.Application.Tests.Unit.ConfigurationProviderTests;

public sealed class ResolveConfigurationTests
{
    [Fact]
    public void ResolveConfiguration_UsesRequestValues_WhenProvided()
    {
        // Arrange
        var configRepo = A.Fake<IUserConfigRepository>();
        A.CallTo(() => configRepo.LoadConfig()).Returns(new UserConfigResponse
        {
            Owner = "configOwner",
            Repo = "configRepo"
        });

        var envRepo = A.Fake<IEnvironmentRepository>();
        A.CallTo(() => envRepo.GetOwner()).Returns("envOwner");
        A.CallTo(() => envRepo.GetRepo()).Returns("envRepo");
        A.CallTo(() => envRepo.GetPersonalAccessToken()).Returns("envPat");

        var provider = new ReleaseConfigurationProvider(configRepo, envRepo);

        var request = new GetLatestReleaseResolverRequest
        {
            Owner = "reqOwner",
            Repo = "reqRepo",
            Pat = "reqPat"
        };

        // Act
        var result = provider.ResolveConfiguration(request);

        // Assert
        result.Owner.Should().Be("reqOwner");
        result.Repo.Should().Be("reqRepo");
        result.Pat.Should().Be("reqPat");
    }

    [Fact]
    public void ResolveConfiguration_UsesUserConfig_WhenRequestMissing()
    {
        // Arrange
        var configRepo = A.Fake<IUserConfigRepository>();
        A.CallTo(() => configRepo.LoadConfig()).Returns(new UserConfigResponse { Owner = "userOwner", Repo = "userRepo" });

        var envRepo = A.Fake<IEnvironmentRepository>();
        A.CallTo(() => envRepo.GetOwner()).Returns(null);
        A.CallTo(() => envRepo.GetRepo()).Returns(null);

        var provider = new ReleaseConfigurationProvider(configRepo, envRepo);

        var request = new GetLatestReleaseResolverRequest();

        // Act
        var result = provider.ResolveConfiguration(request);

        // Assert
        result.Owner.Should().Be("userOwner");
        result.Repo.Should().Be("userRepo");
        result.Pat.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ResolveConfiguration_UsesEnvironment_WhenNoRequestOrUserConfig()
    {
        // Arrange
        var configRepo = A.Fake<IUserConfigRepository>();
        A.CallTo(() => configRepo.LoadConfig()).Returns(null);

        var envRepo = A.Fake<IEnvironmentRepository>();
        A.CallTo(() => envRepo.GetOwner()).Returns("envOwner");
        A.CallTo(() => envRepo.GetRepo()).Returns("envRepo");
        A.CallTo(() => envRepo.GetPersonalAccessToken()).Returns("envPat");

        var provider = new ReleaseConfigurationProvider(configRepo, envRepo);

        var request = new GetLatestReleaseResolverRequest();

        // Act
        var result = provider.ResolveConfiguration(request);

        // Assert
        result.Owner.Should().Be("envOwner");
        result.Repo.Should().Be("envRepo");
        result.Pat.Should().Be("envPat");
    }

    [Fact]
    public void ResolveConfiguration_UsesEnvironmentOverUserConfig_WhenRequestMissing()
    {
        // Arrange
        var configRepo = A.Fake<IUserConfigRepository>();
        A.CallTo(() => configRepo.LoadConfig()).Returns(new UserConfigResponse
        {
            Owner = "configOwner",
            Repo = "configRepo"
        });

        var envRepo = A.Fake<IEnvironmentRepository>();
        A.CallTo(() => envRepo.GetOwner()).Returns("envOwner");
        A.CallTo(() => envRepo.GetRepo()).Returns("envRepo");
        A.CallTo(() => envRepo.GetPersonalAccessToken()).Returns("envPat");

        var provider = new ReleaseConfigurationProvider(configRepo, envRepo);

        // Act
        var result = provider.ResolveConfiguration(new GetLatestReleaseResolverRequest());

        // Assert
        result.Owner.Should().Be("envOwner");
        result.Repo.Should().Be("envRepo");
        result.Pat.Should().Be("envPat");
    }

    [Fact]
    public void ResolveConfiguration_TreatsEmptyAndWhitespaceValuesAsMissing()
    {
        // Arrange
        var configRepo = A.Fake<IUserConfigRepository>();
        A.CallTo(() => configRepo.LoadConfig()).Returns(new UserConfigResponse
        {
            Owner = "configOwner",
            Repo = "configRepo"
        });

        var envRepo = A.Fake<IEnvironmentRepository>();
        A.CallTo(() => envRepo.GetOwner()).Returns("   ");
        A.CallTo(() => envRepo.GetRepo()).Returns(string.Empty);
        A.CallTo(() => envRepo.GetPersonalAccessToken()).Returns("envPat");

        var provider = new ReleaseConfigurationProvider(configRepo, envRepo);
        var request = new GetLatestReleaseResolverRequest
        {
            Owner = string.Empty,
            Repo = "\t",
            Pat = "   "
        };

        // Act
        var result = provider.ResolveConfiguration(request);

        // Assert
        result.Owner.Should().Be("configOwner");
        result.Repo.Should().Be("configRepo");
        result.Pat.Should().Be("envPat");
    }

    [Fact]
    public void ResolveConfiguration_UsesDefaults_WhenNoneProvided()
    {
        // Arrange
        var configRepo = A.Fake<IUserConfigRepository>();
        A.CallTo(() => configRepo.LoadConfig()).Returns(null);

        var envRepo = A.Fake<IEnvironmentRepository>();
        A.CallTo(() => envRepo.GetOwner()).Returns(null);
        A.CallTo(() => envRepo.GetRepo()).Returns(null);
        A.CallTo(() => envRepo.GetPersonalAccessToken()).Returns(null);

        var provider = new ReleaseConfigurationProvider(configRepo, envRepo);

        var request = new GetLatestReleaseResolverRequest();

        // Act
        var result = provider.ResolveConfiguration(request);

        // Assert
        result.Owner.Should().Be("Webbarrr");
        result.Repo.Should().Be("Web.GetLatest");
        result.Pat.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ResolveConfiguration_RequestPatOverridesEnvironment()
    {
        // Arrange
        var configRepo = A.Fake<IUserConfigRepository>();
        A.CallTo(() => configRepo.LoadConfig()).Returns(null);

        var envRepo = A.Fake<IEnvironmentRepository>();
        A.CallTo(() => envRepo.GetPersonalAccessToken()).Returns("envPat");

        var provider = new ReleaseConfigurationProvider(configRepo, envRepo);

        var request = new GetLatestReleaseResolverRequest { Pat = "reqPat" };

        // Act
        var result = provider.ResolveConfiguration(request);

        // Assert
        result.Pat.Should().Be("reqPat");
    }
}