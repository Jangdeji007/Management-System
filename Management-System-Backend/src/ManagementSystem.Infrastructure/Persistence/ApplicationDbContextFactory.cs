using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ManagementSystem.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for EF Core CLI (migrations). Reads connection string from Api User Secrets / appsettings.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var connectionString = ConnectionStringResolver.Resolve(configuration);
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        if (ConnectionStringResolver.IsAzureProfile(configuration))
        {
            optionsBuilder.UseSqlServer(connectionString, sql =>
                sql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null));
        }
        else
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    internal static IConfiguration BuildConfiguration()
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "ManagementSystem.Api");
        return new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddUserSecrets("ManagementSystem.Api-7c4e9a2b-1f3d-4b8e-9c0a-2d5e6f708192")
            .AddJsonFile("appsettings.Local.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }
}
