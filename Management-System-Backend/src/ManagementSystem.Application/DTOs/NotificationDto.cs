namespace ManagementSystem.Application.DTOs;

public sealed record NotificationDto(
    Guid Id,
    string Type,
    string Message,
    Guid? RelatedTaskId,
    bool IsRead,
    DateTime CreatedAt);
