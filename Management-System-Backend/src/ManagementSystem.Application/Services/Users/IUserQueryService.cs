using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Domain.Enums;

namespace ManagementSystem.Application.Services.Users;

public interface IUserQueryService
{
    Task<OperationResult<IReadOnlyList<UserSummaryDto>>> ListUsersAsync(
        Guid callerId,
        UserRole callerRole,
        Guid? teamId,
        CancellationToken cancellationToken = default);
}
