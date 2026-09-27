namespace ManagementSystem.Application.DTOs;

public sealed record DashboardSummaryDto(
    TaskStatusCountsDto TaskCounts,
    int UnreadNotificationCount);
