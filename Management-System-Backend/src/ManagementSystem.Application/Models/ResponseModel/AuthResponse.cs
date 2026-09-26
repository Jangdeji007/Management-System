using ManagementSystem.Application.DTOs;

namespace ManagementSystem.Application.Models.ResponseModel;

public record AuthResponse(string AccessToken, UserProfileDto User);
