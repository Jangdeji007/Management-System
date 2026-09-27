using Microsoft.Extensions.Configuration;

namespace ManagementSystem.Infrastructure.Persistence;

public static class ConnectionStringResolver
{
    public const string ProfileKey = "Database:ConnectionProfile";

    public static string Resolve(IConfiguration configuration)
    {
        var profile = configuration[ProfileKey];
        if (string.IsNullOrWhiteSpace(profile))
            profile = "Local";

        string? connectionString = profile.Equals("Azure", StringComparison.OrdinalIgnoreCase)
            ? configuration.GetConnectionString("Azure")
            : configuration.GetConnectionString("Local");

        connectionString ??= configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string is missing for profile '{profile}'. " +
                "Set ConnectionStrings:Local (or Azure) and Database:ConnectionProfile in appsettings.Local.json.");
        }

        return connectionString;
    }

    public static bool IsAzureProfile(IConfiguration configuration)
    {
        var profile = configuration[ProfileKey];
        return profile?.Equals("Azure", StringComparison.OrdinalIgnoreCase) == true;
    }
}
