using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Domain.Entities;

namespace ManagementSystem.Application.Abstractions;

public interface ITaskRepository
{
    Task<IReadOnlyList<TaskItem>> ListAsync(
        TaskQueryFilter filter,
        CancellationToken cancellationToken = default);

    Task<TaskStatusCountsDto> GetStatusCountsAsync(
        TaskQueryFilter filter,
        CancellationToken cancellationToken = default);

    Task<TaskItem?> GetByIdForReadAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TaskItem?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);

    Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default);

    Task DeleteAsync(TaskItem task, CancellationToken cancellationToken = default);
}
