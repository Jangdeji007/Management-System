using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Models.ResponseModel;

namespace ManagementSystem.Application.Models.Mappings;

public static class DashboardResponseMapping
{
    public static DashboardSummaryResponse ToResponse(this DashboardSummaryDto dto) =>
        new(
            new TaskStatusCountsResponse(
                dto.TaskCounts.ToDo,
                dto.TaskCounts.InProgress,
                dto.TaskCounts.Done,
                dto.TaskCounts.Total),
            dto.UnreadNotificationCount);
}
