using ManagementSystem.Domain.Entities;

namespace ManagementSystem.Application.Abstractions;

public interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IReadOnlyList<Notification> notifications, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Notification>> ListByUserIdAsync(
        Guid userId,
        bool? unreadOnly,
        CancellationToken cancellationToken = default);

    Task<int> CountUnreadByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Notification?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);

    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
}
