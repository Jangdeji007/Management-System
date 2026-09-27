using ManagementSystem.Application.Abstractions;
using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Models.RequestModel;
using ManagementSystem.Application.Models.ResponseModel;
using ManagementSystem.Domain.Entities;
using ManagementSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ManagementSystem.Application.Services.Auth;

public class AuthService(
    IUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordHasher<User> passwordHasher) : IAuthService
{
    public async Task<AuthResult<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || !VerifyPassword(user, request.Password))
            return AuthResult<AuthResponse>.Fail(AuthFailureKind.InvalidCredentials);

        return AuthResult<AuthResponse>.Success(BuildAuthResponse(user));
    }

    public async Task<AuthResult<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await userRepository.ExistsByEmailAsync(email, cancellationToken))
            return AuthResult<AuthResponse>.Fail(AuthFailureKind.EmailAlreadyExists);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = request.FullName.Trim(),
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow,
            PasswordHash = string.Empty
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        await userRepository.AddAsync(user, cancellationToken);

        return AuthResult<AuthResponse>.Success(BuildAuthResponse(user));
    }

    public async Task<AuthResult<UserProfileDto>> GetCurrentUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return AuthResult<UserProfileDto>.Fail(AuthFailureKind.UserNotFound);

        return AuthResult<UserProfileDto>.Success(MapProfile(user));
    }

    private bool VerifyPassword(User user, string password)
    {
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }

    private AuthResponse BuildAuthResponse(User user)
    {
        var token = jwtTokenGenerator.GenerateAccessToken(user);
        return new AuthResponse(token, MapProfile(user));
    }

    private static UserProfileDto MapProfile(User user) =>
        new(user.Id, user.Email, user.FullName, user.Role.ToString(), user.CreatedAt);
}
