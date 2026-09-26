namespace ManagementSystem.Application.DTOs;

public record UserProfileDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    DateTime CreatedAt);
