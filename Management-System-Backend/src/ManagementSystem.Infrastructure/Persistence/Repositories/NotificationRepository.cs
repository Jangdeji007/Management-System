using ManagementSystem.Application.Abstractions;
using ManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManagementSystem.Infrastructure.Persistence.Repositories;

public class NotificationRepository(ApplicationDbContext context) : INotificationRepository
{
    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        context.Notifications.Add(notification);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IReadOnlyList<Notification> notifications, CancellationToken cancellationToken = default)
    {
        if (notifications.Count == 0)
            return;

        context.Notifications.AddRange(notifications);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> ListByUserIdAsync(
        Guid userId,
        bool? unreadOnly,
        CancellationToken cancellationToken = default)
    {
        var query = context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId);

        if (unreadOnly == true)
            query = query.Where(n => !n.IsRead);

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountUnreadByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        context.Notifications
            .AsNoTracking()
            .CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);

    public Task<Notification?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Notifications.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        context.Notifications.Update(notification);
        await context.SaveChangesAsync(cancellationToken);
    }
}
