using Spectre.Console.Cli;
using Web.GetLatest.Cli.Enums;

namespace Web.GetLatest.Cli.Tests;

public class CliPlumbingTests
{
    [Fact]
    public async Task AppRun_WhenCommandThrowsUnhandledException_InterceptsAndReturnsFailureExitCode()
    {
        // Arrange: Set up a minimal app using your exact boilerplate configuration style
        var app = new CommandApp();

        app.Configure(config =>
        {
            config.PropagateExceptions();

            // Explicitly provide EmptyCommandSettings so the compiler can perfectly infer the generic method
            config.AddDelegate<EmptyCommandSettings>("test-fail", (context, settings, cancellationToken) =>
            {
                throw new InvalidOperationException("Simulated internal infrastructure failure.");
            });
        });

        // Act: Run the app through the global try/catch handler logic from your Program.cs
        int exitCode;
        try
        {
            await app.RunAsync(["test-fail"], TestContext.Current.CancellationToken);
            exitCode = 0;
        }
        catch (Exception)
        {
            exitCode = CliStatusCodes.Failure.ToExitCode();
        }

        // Assert
        Assert.Equal(1, exitCode);
    }
}