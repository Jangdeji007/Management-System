using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Models.ResponseModel;

namespace ManagementSystem.Application.Models.Mappings;

public static class TaskCommentResponseMapping
{
    public static TaskCommentResponse ToResponse(this TaskCommentDto dto) =>
        new(
            dto.Id,
            dto.TaskId,
            dto.AuthorId,
            dto.AuthorName,
            dto.Body,
            dto.CreatedAt);

    public static IReadOnlyList<TaskCommentResponse> ToResponse(this IReadOnlyList<TaskCommentDto> dtos) =>
        dtos.Select(d => d.ToResponse()).ToList();
}
