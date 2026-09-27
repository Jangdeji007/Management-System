namespace ManagementSystem.Application.Models.ResponseModel;

public record TaskCommentResponse(
    Guid Id,
    Guid TaskId,
    Guid AuthorId,
    string AuthorName,
    string Body,
    DateTime CreatedAt);
