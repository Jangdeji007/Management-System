using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Models.RequestModel;
using ManagementSystem.Domain.Enums;

namespace ManagementSystem.Application.Services.Tasks;

public interface ITaskCommentService
{
    Task<OperationResult<IReadOnlyList<TaskCommentDto>>> ListCommentsAsync(
        Guid callerId,
        UserRole callerRole,
        Guid taskId,
        CancellationToken cancellationToken = default);

    Task<OperationResult<TaskCommentDto>> AddCommentAsync(
        Guid callerId,
        UserRole callerRole,
        Guid taskId,
        CreateTaskCommentRequest request,
        CancellationToken cancellationToken = default);
}
