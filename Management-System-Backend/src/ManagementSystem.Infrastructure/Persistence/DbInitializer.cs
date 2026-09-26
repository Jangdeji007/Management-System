using ManagementSystem.Domain.Entities;
using ManagementSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;
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

        if (await context.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var hasher = new PasswordHasher<User>();
        var now = DateTime.UtcNow;

        var admin = CreateUser(
            "admin@demo.com",
            "System Admin",
            UserRole.Admin,
            "Admin@123",
            hasher,
            now);

        var manager = CreateUser(
            "manager@demo.com",
            "Team Manager",
            UserRole.Manager,
            "Manager@123",
            hasher,
            now);

        var user = CreateUser(
            "user@demo.com",
            "Regular User",
            UserRole.User,
            "User@123",
            hasher,
            now);

        context.Users.AddRange(admin, manager, user);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded demo users (admin@demo.com, manager@demo.com, user@demo.com).");
    }

    private static User CreateUser(
        string email,
        string fullName,
        UserRole role,
        string password,
        PasswordHasher<User> hasher,
        DateTime createdAt)
    {
        var entity = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = fullName,
            Role = role,
            CreatedAt = createdAt,
            PasswordHash = string.Empty
        };

        entity.PasswordHash = hasher.HashPassword(entity, password);
        return entity;
    }
}
