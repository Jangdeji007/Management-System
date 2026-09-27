using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;

namespace ManagementSystem.Application.Services.Notifications;

public interface INotificationService
{
    Task<OperationResult<IReadOnlyList<NotificationDto>>> ListForCurrentUserAsync(
        Guid callerId,
        bool? unreadOnly,
        CancellationToken cancellationToken = default);

    Task<OperationResult<NotificationDto>> MarkReadAsync(
        Guid callerId,
        Guid notificationId,
        CancellationToken cancellationToken = default);
}
