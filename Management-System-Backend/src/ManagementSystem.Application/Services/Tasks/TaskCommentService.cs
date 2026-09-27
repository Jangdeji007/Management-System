using ManagementSystem.Application.Abstractions;
using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Models.RequestModel;
using ManagementSystem.Domain.Entities;
using ManagementSystem.Domain.Enums;

namespace ManagementSystem.Application.Services.Tasks;

public class TaskCommentService(
    ITaskRepository taskRepository,
    ITaskCommentRepository commentRepository,
    TaskAccessEvaluator taskAccess) : ITaskCommentService
{
    public async Task<OperationResult<IReadOnlyList<TaskCommentDto>>> ListCommentsAsync(
        Guid callerId,
        UserRole callerRole,
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        var gate = await EnsureCanAccessTaskAsync(callerId, callerRole, taskId, cancellationToken);
        if (gate is not null)
            return OperationResult<IReadOnlyList<TaskCommentDto>>.Fail(gate.Value);

        var comments = await commentRepository.ListByTaskIdAsync(taskId, cancellationToken);
        var dtos = comments.Select(MapComment).ToList();
        return OperationResult<IReadOnlyList<TaskCommentDto>>.Success(dtos);
    }

    public async Task<OperationResult<TaskCommentDto>> AddCommentAsync(
        Guid callerId,
        UserRole callerRole,
        Guid taskId,
        CreateTaskCommentRequest request,
        CancellationToken cancellationToken = default)
    {
        var gate = await EnsureCanAccessTaskAsync(callerId, callerRole, taskId, cancellationToken);
        if (gate is not null)
            return OperationResult<TaskCommentDto>.Fail(gate.Value);

        var now = DateTime.UtcNow;
        var comment = new TaskComment
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            AuthorId = callerId,
            Body = request.Body.Trim(),
            CreatedAt = now
        };

        await commentRepository.AddAsync(comment, cancellationToken);

        var comments = await commentRepository.ListByTaskIdAsync(taskId, cancellationToken);
        var created = comments.First(c => c.Id == comment.Id);
        return OperationResult<TaskCommentDto>.Success(MapComment(created));
    }

    private async Task<OperationFailureKind?> EnsureCanAccessTaskAsync(
        Guid callerId,
        UserRole callerRole,
        Guid taskId,
        CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetByIdForReadAsync(taskId, cancellationToken);
        if (task is null)
            return OperationFailureKind.NotFound;

        if (!await taskAccess.CanAccessTaskAsync(callerId, callerRole, task, cancellationToken))
            return OperationFailureKind.Forbidden;

        return null;
    }

    internal static TaskCommentDto MapComment(TaskComment comment) =>
        new(
            comment.Id,
            comment.TaskId,
            comment.AuthorId,
            comment.Author.FullName,
            comment.Body,
            comment.CreatedAt);

    internal static IReadOnlyList<TaskCommentDto> MapComments(IEnumerable<TaskComment> comments) =>
        comments.Select(MapComment).ToList();
}
