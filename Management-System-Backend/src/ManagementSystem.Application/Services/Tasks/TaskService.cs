using ManagementSystem.Application.Abstractions;
using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Models.RequestModel;
using ManagementSystem.Application.Services.Notifications;
using ManagementSystem.Domain.Entities;
using ManagementSystem.Domain.Enums;
using DomainTaskStatus = ManagementSystem.Domain.Enums.TaskStatus;

namespace ManagementSystem.Application.Services.Tasks;

public class TaskService(
    ITaskRepository taskRepository,
    ITeamRepository teamRepository,
    IUserRepository userRepository,
    TaskAccessEvaluator taskAccess,
    ITaskNotificationService taskNotificationService,
    TaskQueryScopeBuilder scopeBuilder) : ITaskService
{
    public async Task<OperationResult<IReadOnlyList<TaskListItemDto>>> ListTasksAsync(
        Guid callerId,
        UserRole callerRole,
        ListTasksQueryRequest query,
        CancellationToken cancellationToken = default)
    {
        var filter = await BuildListFilterAsync(callerId, callerRole, query, cancellationToken);
        if (filter is null)
            return OperationResult<IReadOnlyList<TaskListItemDto>>.Fail(OperationFailureKind.Forbidden);

        var tasks = await taskRepository.ListAsync(filter, cancellationToken);
        var dtos = tasks.Select(MapListItem).ToList();
        return OperationResult<IReadOnlyList<TaskListItemDto>>.Success(dtos);
    }

    public async Task<OperationResult<TaskDetailDto>> GetTaskAsync(
        Guid callerId,
        UserRole callerRole,
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdForReadAsync(taskId, cancellationToken);
        if (task is null)
            return OperationResult<TaskDetailDto>.Fail(OperationFailureKind.NotFound);

        if (!await taskAccess.CanAccessTaskAsync(callerId, callerRole, task, cancellationToken))
            return OperationResult<TaskDetailDto>.Fail(OperationFailureKind.Forbidden);

        return OperationResult<TaskDetailDto>.Success(MapDetail(task));
    }

    public async Task<OperationResult<TaskDetailDto>> CreateTaskAsync(
        Guid callerId,
        UserRole callerRole,
        CreateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        if (callerRole is UserRole.User)
            return OperationResult<TaskDetailDto>.Fail(OperationFailureKind.Forbidden);

        var validation = await ValidateAssignmentAsync(
            callerId,
            callerRole,
            request.AssigneeId,
            request.TeamId,
            requireTeamIdForManager: true,
            cancellationToken);

        if (validation is not null)
            return OperationResult<TaskDetailDto>.Fail(validation.Value);

        var now = DateTime.UtcNow;
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim(),
            Status = DomainTaskStatus.ToDo,
            Priority = request.Priority,
            DueDate = request.DueDate,
            AssigneeId = request.AssigneeId,
            CreatedById = callerId,
            TeamId = request.TeamId,
            CreatedAt = now,
            UpdatedAt = now
        };

        await taskRepository.AddAsync(task, cancellationToken);

        await taskNotificationService.NotifyAssignmentAsync(task, callerId, cancellationToken);

        var created = await taskRepository.GetByIdForReadAsync(task.Id, cancellationToken);
        return OperationResult<TaskDetailDto>.Success(MapDetail(created!));
    }

    public async Task<OperationResult<TaskDetailDto>> UpdateTaskAsync(
        Guid callerId,
        UserRole callerRole,
        Guid taskId,
        UpdateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        if (callerRole is UserRole.User)
            return OperationResult<TaskDetailDto>.Fail(OperationFailureKind.Forbidden);

        var task = await taskRepository.GetByIdForUpdateAsync(taskId, cancellationToken);
        if (task is null)
            return OperationResult<TaskDetailDto>.Fail(OperationFailureKind.NotFound);

        if (!await taskAccess.CanManageTaskAsync(callerId, callerRole, task, cancellationToken))
            return OperationResult<TaskDetailDto>.Fail(OperationFailureKind.Forbidden);

        var validation = await ValidateAssignmentAsync(
            callerId,
            callerRole,
            request.AssigneeId,
            request.TeamId,
            requireTeamIdForManager: false,
            cancellationToken);

        if (validation is not null)
            return OperationResult<TaskDetailDto>.Fail(validation.Value);

        var assigneeChanged = task.AssigneeId != request.AssigneeId;

        task.Title = request.Title.Trim();
        task.Description = string.IsNullOrWhiteSpace(request.Description)
            ? null
            : request.Description.Trim();
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;
        task.AssigneeId = request.AssigneeId;
        task.TeamId = request.TeamId;
        task.UpdatedAt = DateTime.UtcNow;

        await taskRepository.UpdateAsync(task, cancellationToken);

        if (assigneeChanged)
            await taskNotificationService.NotifyAssignmentAsync(task, callerId, cancellationToken);

        var updated = await taskRepository.GetByIdForReadAsync(taskId, cancellationToken);
        return OperationResult<TaskDetailDto>.Success(MapDetail(updated!));
    }

    public async Task<OperationResult<TaskDetailDto>> UpdateTaskStatusAsync(
        Guid callerId,
        UserRole callerRole,
        Guid taskId,
        UpdateTaskStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdForUpdateAsync(taskId, cancellationToken);
        if (task is null)
            return OperationResult<TaskDetailDto>.Fail(OperationFailureKind.NotFound);

        if (!await taskAccess.CanUpdateStatusAsync(callerId, callerRole, task, cancellationToken))
            return OperationResult<TaskDetailDto>.Fail(OperationFailureKind.Forbidden);

        var statusChanged = task.Status != request.Status;
        task.Status = request.Status;
        task.UpdatedAt = DateTime.UtcNow;

        await taskRepository.UpdateAsync(task, cancellationToken);

        if (statusChanged)
            await taskNotificationService.NotifyStatusChangeAsync(task, request.Status, callerId, cancellationToken);

        var updated = await taskRepository.GetByIdForReadAsync(taskId, cancellationToken);
        return OperationResult<TaskDetailDto>.Success(MapDetail(updated!));
    }

    public async Task<OperationResult<bool>> DeleteTaskAsync(
        Guid callerId,
        UserRole callerRole,
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        if (callerRole is UserRole.User)
            return OperationResult<bool>.Fail(OperationFailureKind.Forbidden);

        var task = await taskRepository.GetByIdForUpdateAsync(taskId, cancellationToken);
        if (task is null)
            return OperationResult<bool>.Fail(OperationFailureKind.NotFound);

        if (!await taskAccess.CanManageTaskAsync(callerId, callerRole, task, cancellationToken))
            return OperationResult<bool>.Fail(OperationFailureKind.Forbidden);

        await taskRepository.DeleteAsync(task, cancellationToken);
        return OperationResult<bool>.Success(true);
    }

    private async Task<TaskQueryFilter?> BuildListFilterAsync(
        Guid callerId,
        UserRole callerRole,
        ListTasksQueryRequest query,
        CancellationToken cancellationToken)
    {
        var baseFilter = await scopeBuilder.BuildScopeAsync(callerId, callerRole, cancellationToken);
        if (baseFilter is null)
            return null;

        return baseFilter with
        {
            Status = query.Status,
            Priority = query.Priority,
            AssigneeId = query.AssigneeId,
            TeamId = query.TeamId,
            DueBefore = query.DueBefore,
            DueAfter = query.DueAfter
        };
    }

    private async Task<OperationFailureKind?> ValidateAssignmentAsync(
        Guid callerId,
        UserRole callerRole,
        Guid assigneeId,
        Guid? teamId,
        bool requireTeamIdForManager,
        CancellationToken cancellationToken)
    {
        if (callerRole == UserRole.Manager)
        {
            if (requireTeamIdForManager && teamId is null)
                return OperationFailureKind.InvalidOperation;

            if (teamId is { } managerTeamId
                && !await teamRepository.IsMemberAsync(managerTeamId, callerId, cancellationToken))
                return OperationFailureKind.Forbidden;
        }

        var assignee = await userRepository.GetByIdAsync(assigneeId, cancellationToken);
        if (assignee is null)
            return OperationFailureKind.NotFound;

        if (teamId is { } tid)
        {
            var team = await teamRepository.GetByIdWithMembersAsync(tid, cancellationToken);
            if (team is null)
                return OperationFailureKind.NotFound;

            if (!await teamRepository.IsMemberAsync(tid, assigneeId, cancellationToken))
                return OperationFailureKind.InvalidOperation;
        }

        return null;
    }

    private static TaskListItemDto MapListItem(TaskItem task) =>
        new(
            task.Id,
            task.Title,
            task.Status.ToString(),
            task.Priority.ToString(),
            task.DueDate,
            task.AssigneeId,
            task.Assignee.FullName,
            task.TeamId,
            task.Team?.Name,
            task.CreatedAt,
            task.UpdatedAt);

    private static TaskDetailDto MapDetail(TaskItem task) =>
        new(
            task.Id,
            task.Title,
            task.Description,
            task.Status.ToString(),
            task.Priority.ToString(),
            task.DueDate,
            task.AssigneeId,
            task.Assignee.FullName,
            task.CreatedById,
            task.CreatedBy.FullName,
            task.TeamId,
            task.Team?.Name,
            task.CreatedAt,
            task.UpdatedAt,
            TaskCommentService.MapComments(task.Comments));
}
