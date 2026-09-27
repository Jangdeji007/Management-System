namespace ManagementSystem.Application.DTOs;

public record TaskCommentDto(
    Guid Id,
    Guid TaskId,
    Guid AuthorId,
    string AuthorName,
    string Body,
    DateTime CreatedAt);
