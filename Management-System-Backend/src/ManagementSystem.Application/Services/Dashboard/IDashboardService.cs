using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Models.RequestModel;
using ManagementSystem.Domain.Enums;

namespace ManagementSystem.Application.Services.Dashboard;

public interface IDashboardService
{
    Task<OperationResult<DashboardSummaryDto>> GetSummaryAsync(
        Guid callerId,
        UserRole callerRole,
        ListTasksQueryRequest query,
        CancellationToken cancellationToken = default);
}
