using ManagementSystem.Domain.Entities;

namespace ManagementSystem.Application.Abstractions;

public interface ITaskCommentRepository
{
    Task<IReadOnlyList<TaskComment>> ListByTaskIdAsync(
        Guid taskId,
        CancellationToken cancellationToken = default);

    Task AddAsync(TaskComment comment, CancellationToken cancellationToken = default);
}
