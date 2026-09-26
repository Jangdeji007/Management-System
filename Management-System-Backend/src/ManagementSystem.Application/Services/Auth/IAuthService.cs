using ManagementSystem.Application.Common;
using ManagementSystem.Application.Models.RequestModel;
using ManagementSystem.Application.Models.ResponseModel;

namespace ManagementSystem.Application.Services.Auth;

public interface IAuthService
{
    Task<AuthResult<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<AuthResult<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}
