using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Models.ResponseModel;

namespace ManagementSystem.Application.Models.Mappings;

public static class TaskResponseMapping
{
    public static TaskListItemResponse ToResponse(this TaskListItemDto dto) =>
        new(
            dto.Id,
            dto.Title,
            dto.Status,
            dto.Priority,
            dto.DueDate,
            dto.AssigneeId,
            dto.AssigneeName,
            dto.TeamId,
            dto.TeamName,
            dto.CreatedAt,
            dto.UpdatedAt);

    public static IReadOnlyList<TaskListItemResponse> ToResponse(this IReadOnlyList<TaskListItemDto> dtos) =>
        dtos.Select(d => d.ToResponse()).ToList();

    public static TaskDetailResponse ToResponse(this TaskDetailDto dto) =>
        new(
            dto.Id,
            dto.Title,
            dto.Description,
            dto.Status,
            dto.Priority,
            dto.DueDate,
            dto.AssigneeId,
            dto.AssigneeName,
            dto.CreatedById,
            dto.CreatedByName,
            dto.TeamId,
            dto.TeamName,
            dto.CreatedAt,
            dto.UpdatedAt,
            dto.Comments.ToResponse());
}
