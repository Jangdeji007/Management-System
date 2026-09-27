namespace ManagementSystem.Application.Models.ResponseModel;

public record TaskDetailResponse(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    string Priority,
    DateTime? DueDate,
    Guid AssigneeId,
    string AssigneeName,
    Guid CreatedById,
    string CreatedByName,
    Guid? TeamId,
    string? TeamName,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<TaskCommentResponse> Comments);
