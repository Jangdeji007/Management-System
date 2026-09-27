using ManagementSystem.Application.Abstractions;
using ManagementSystem.Domain.Entities;
using ManagementSystem.Domain.Enums;
using DomainTaskStatus = ManagementSystem.Domain.Enums.TaskStatus;

namespace ManagementSystem.Application.Services.Notifications;

public class TaskNotificationService(INotificationRepository notificationRepository) : ITaskNotificationService
{
    public Task NotifyAssignmentAsync(TaskItem task, Guid actorId, CancellationToken cancellationToken = default)
    {
        if (task.AssigneeId == actorId)
            return Task.CompletedTask;

        var notification = CreateNotification(
            task.AssigneeId,
            NotificationType.TaskAssigned,
            $"You were assigned: {task.Title}",
            task.Id);

        return notificationRepository.AddAsync(notification, cancellationToken);
    }

    public Task NotifyStatusChangeAsync(
        TaskItem task,
        DomainTaskStatus newStatus,
        Guid actorId,
        CancellationToken cancellationToken = default)
    {
        var recipientIds = new HashSet<Guid> { task.AssigneeId, task.CreatedById };
        recipientIds.Remove(actorId);

        if (recipientIds.Count == 0)
            return Task.CompletedTask;

        var message = $"Status updated to {newStatus} on: {task.Title}";
        var notifications = recipientIds
            .Select(userId => CreateNotification(
                userId,
                NotificationType.TaskStatusUpdated,
                message,
                task.Id))
            .ToList();

        return notificationRepository.AddRangeAsync(notifications, cancellationToken);
    }

    private static Notification CreateNotification(
        Guid userId,
        NotificationType type,
        string message,
        Guid relatedTaskId) =>
        new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = type,
            Message = message,
            RelatedTaskId = relatedTaskId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
}
