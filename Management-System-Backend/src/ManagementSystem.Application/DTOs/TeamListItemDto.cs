namespace ManagementSystem.Application.DTOs;

public record TeamListItemDto(
    Guid Id,
    string Name,
    string? Description,
    Guid CreatedByUserId,
    DateTime CreatedAt,
    IReadOnlyList<TeamMemberDto> Members);
