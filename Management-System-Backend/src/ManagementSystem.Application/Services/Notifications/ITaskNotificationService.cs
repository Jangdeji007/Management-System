using ManagementSystem.Domain.Entities;
using DomainTaskStatus = ManagementSystem.Domain.Enums.TaskStatus;

namespace ManagementSystem.Application.Services.Notifications;

public interface ITaskNotificationService
{
    Task NotifyAssignmentAsync(TaskItem task, Guid actorId, CancellationToken cancellationToken = default);

    Task NotifyStatusChangeAsync(
        TaskItem task,
        DomainTaskStatus newStatus,
        Guid actorId,
        CancellationToken cancellationToken = default);
}
