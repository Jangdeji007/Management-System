namespace ManagementSystem.Application.DTOs;

public sealed record TaskStatusCountsDto(int ToDo, int InProgress, int Done, int Total);
