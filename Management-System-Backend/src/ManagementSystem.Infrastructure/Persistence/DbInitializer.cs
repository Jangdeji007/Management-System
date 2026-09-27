using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManagementSystem.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        ApplicationDbContext context,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        await context.Database.MigrateAsync(cancellationToken);
        await DemoDataSeeder.SeedAsync(context, logger, cancellationToken);
    }
}
