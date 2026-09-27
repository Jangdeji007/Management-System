using ManagementSystem.Application.Abstractions;
using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Domain.Entities;

namespace ManagementSystem.Application.Services.Notifications;

public class NotificationService(INotificationRepository notificationRepository) : INotificationService
{
    public async Task<OperationResult<IReadOnlyList<NotificationDto>>> ListForCurrentUserAsync(
        Guid callerId,
        bool? unreadOnly,
        CancellationToken cancellationToken = default)
    {
        var notifications = await notificationRepository.ListByUserIdAsync(callerId, unreadOnly, cancellationToken);
        var dtos = notifications.Select(Map).ToList();
        return OperationResult<IReadOnlyList<NotificationDto>>.Success(dtos);
    }

    public async Task<OperationResult<NotificationDto>> MarkReadAsync(
        Guid callerId,
        Guid notificationId,
        CancellationToken cancellationToken = default)
    {
        var notification = await notificationRepository.GetByIdForUpdateAsync(notificationId, cancellationToken);
        if (notification is null)
            return OperationResult<NotificationDto>.Fail(OperationFailureKind.NotFound);

        if (notification.UserId != callerId)
            return OperationResult<NotificationDto>.Fail(OperationFailureKind.Forbidden);

        notification.IsRead = true;
        await notificationRepository.UpdateAsync(notification, cancellationToken);

        return OperationResult<NotificationDto>.Success(Map(notification));
    }

    private static NotificationDto Map(Notification notification) =>
        new(
            notification.Id,
            notification.Type.ToString(),
            notification.Message,
            notification.RelatedTaskId,
            notification.IsRead,
            notification.CreatedAt);
}
