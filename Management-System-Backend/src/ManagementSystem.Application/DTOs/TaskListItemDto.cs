namespace ManagementSystem.Application.DTOs;

public record TaskListItemDto(
    Guid Id,
    string Title,
    string Status,
    string Priority,
    DateTime? DueDate,
    Guid AssigneeId,
    string AssigneeName,
    Guid? TeamId,
    string? TeamName,
    DateTime CreatedAt,
    DateTime UpdatedAt);
