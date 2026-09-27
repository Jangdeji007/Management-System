using ManagementSystem.Application.Abstractions;
using ManagementSystem.Domain.Entities;
using ManagementSystem.Domain.Enums;

namespace ManagementSystem.Application.Services.Tasks;

public class TaskAccessEvaluator(ITeamRepository teamRepository)
{
    public async Task<bool> CanAccessTaskAsync(
        Guid callerId,
        UserRole callerRole,
        TaskItem task,
        CancellationToken cancellationToken = default)
    {
        if (callerRole == UserRole.Admin)
            return true;

        if (callerRole == UserRole.User)
            return task.AssigneeId == callerId;

        if (callerRole == UserRole.Manager)
            return await IsTaskInManagerScopeAsync(callerId, task, cancellationToken);

        return false;
    }

    public async Task<bool> CanManageTaskAsync(
        Guid callerId,
        UserRole callerRole,
        TaskItem task,
        CancellationToken cancellationToken = default)
    {
        if (callerRole == UserRole.Admin)
            return true;

        if (callerRole == UserRole.Manager)
            return await IsTaskInManagerScopeAsync(callerId, task, cancellationToken);

        return false;
    }

    public async Task<bool> CanUpdateStatusAsync(
        Guid callerId,
        UserRole callerRole,
        TaskItem task,
        CancellationToken cancellationToken = default)
    {
        if (callerRole == UserRole.User)
            return task.AssigneeId == callerId;

        return await CanAccessTaskAsync(callerId, callerRole, task, cancellationToken);
    }

    private async Task<bool> IsTaskInManagerScopeAsync(
        Guid callerId,
        TaskItem task,
        CancellationToken cancellationToken)
    {
        if (task.TeamId is not { } teamId)
            return false;

        return await teamRepository.IsMemberAsync(teamId, callerId, cancellationToken);
    }
}
