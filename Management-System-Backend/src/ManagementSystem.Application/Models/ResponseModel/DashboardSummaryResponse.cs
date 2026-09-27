namespace ManagementSystem.Application.Models.ResponseModel;

public sealed record TaskStatusCountsResponse(int ToDo, int InProgress, int Done, int Total);

public sealed record DashboardSummaryResponse(
    TaskStatusCountsResponse TaskCounts,
    int UnreadNotificationCount);
