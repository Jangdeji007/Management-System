using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Models.RequestModel;
using ManagementSystem.Domain.Enums;

namespace ManagementSystem.Application.Services.Tasks;

public interface ITaskService
{
    Task<OperationResult<IReadOnlyList<TaskListItemDto>>> ListTasksAsync(
        Guid callerId,
        UserRole callerRole,
        ListTasksQueryRequest query,
        CancellationToken cancellationToken = default);

    Task<OperationResult<TaskDetailDto>> GetTaskAsync(
        Guid callerId,
        UserRole callerRole,
        Guid taskId,
        CancellationToken cancellationToken = default);

    Task<OperationResult<TaskDetailDto>> CreateTaskAsync(
        Guid callerId,
        UserRole callerRole,
        CreateTaskRequest request,
        CancellationToken cancellationToken = default);

    Task<OperationResult<TaskDetailDto>> UpdateTaskAsync(
        Guid callerId,
        UserRole callerRole,
        Guid taskId,
        UpdateTaskRequest request,
        CancellationToken cancellationToken = default);

    Task<OperationResult<TaskDetailDto>> UpdateTaskStatusAsync(
        Guid callerId,
        UserRole callerRole,
        Guid taskId,
        UpdateTaskStatusRequest request,
        CancellationToken cancellationToken = default);

    Task<OperationResult<bool>> DeleteTaskAsync(
        Guid callerId,
        UserRole callerRole,
        Guid taskId,
        CancellationToken cancellationToken = default);
}
