using Microsoft.Extensions.Logging;
using System.Text.Json;
using Web.GetLatest.Application.Contracts.Infrastructure.UserConfig;

namespace Web.GetLatest.Infrastructure.UserConfig;

public class LocalUserConfigRepository(ILogger<LocalUserConfigRepository> logger) : IUserConfigRepository
{
    public UserConfigResponse? LoadConfig()
    {
        try
        {
            // Search for the config file in common per-user locations (in order of preference):
            // 1) %APPDATA% (ApplicationData)
            // 2) %LOCALAPPDATA% (LocalApplicationData)
            // 3) %USERPROFILE% (legacy behavior)
            var candidateRoots = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            };

            foreach (var root in candidateRoots)
            {
                if (string.IsNullOrWhiteSpace(root))
                    continue;

                var configPath = Path.Combine(root, ".ghver", "config.json");

                if (!File.Exists(configPath))
                    continue;

                try
                {
                    string json = File.ReadAllText(configPath);
                    var deserialized = JsonSerializer.Deserialize<UserConfigResponse>(json);
                    if (deserialized is not null)
                        return deserialized;
                    
                    logger.LogWarning("Configuration file at '{ConfigPath}' could not be deserialized to UserConfigResponse.", configPath);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "The configuration file exists at '{ConfigPath}', but failed to be read or parsed.", configPath);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "An unexpected error occurred while checking for the user configuration path.");
        }

        return null;
    }
}