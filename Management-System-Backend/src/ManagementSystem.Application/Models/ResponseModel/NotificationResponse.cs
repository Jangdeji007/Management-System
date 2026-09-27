namespace ManagementSystem.Application.Models.ResponseModel;

public sealed record NotificationResponse(
    Guid Id,
    string Type,
    string Message,
    Guid? RelatedTaskId,
    bool IsRead,
    DateTime CreatedAt);
