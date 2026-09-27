using ManagementSystem.Application.Abstractions;
using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Models.RequestModel;
using ManagementSystem.Application.Services.Tasks;
using ManagementSystem.Domain.Enums;

namespace ManagementSystem.Application.Services.Dashboard;

public class DashboardService(
    ITaskRepository taskRepository,
    INotificationRepository notificationRepository,
    TaskQueryScopeBuilder scopeBuilder) : IDashboardService
{
    public async Task<OperationResult<DashboardSummaryDto>> GetSummaryAsync(
        Guid callerId,
        UserRole callerRole,
        ListTasksQueryRequest query,
        CancellationToken cancellationToken = default)
    {
        var scope = await scopeBuilder.BuildScopeAsync(callerId, callerRole, cancellationToken);
        if (scope is null)
            return OperationResult<DashboardSummaryDto>.Fail(OperationFailureKind.Forbidden);

        var filter = scope with
        {
            Priority = query.Priority,
            AssigneeId = query.AssigneeId,
            TeamId = query.TeamId,
            DueBefore = query.DueBefore,
            DueAfter = query.DueAfter
        };

        var taskCounts = await taskRepository.GetStatusCountsAsync(filter, cancellationToken);
        var unreadCount = await notificationRepository.CountUnreadByUserIdAsync(callerId, cancellationToken);

        return OperationResult<DashboardSummaryDto>.Success(
            new DashboardSummaryDto(taskCounts, unreadCount));
    }
}
