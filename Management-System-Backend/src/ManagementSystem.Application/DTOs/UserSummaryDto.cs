namespace ManagementSystem.Application.DTOs;

public record UserSummaryDto(
    Guid Id,
    string Email,
    string FullName,
    string Role);
