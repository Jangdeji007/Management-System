namespace ManagementSystem.Application.DTOs;

public record TeamMemberDto(
    Guid UserId,
    string Email,
    string FullName,
    string Role,
    DateTime JoinedAt);
