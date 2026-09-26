using ManagementSystem.Application.Abstractions;
using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Domain.Entities;
using ManagementSystem.Domain.Enums;

namespace ManagementSystem.Application.Services.Users;

public class UserQueryService(IUserRepository userRepository, ITeamRepository teamRepository) : IUserQueryService
{
    public async Task<OperationResult<IReadOnlyList<UserSummaryDto>>> ListUsersAsync(
        Guid callerId,
        UserRole callerRole,
        Guid? teamId,
        CancellationToken cancellationToken = default)
    {
        if (callerRole is UserRole.User)
            return OperationResult<IReadOnlyList<UserSummaryDto>>.Fail(OperationFailureKind.Forbidden);

        IReadOnlyList<User> users;

        if (callerRole == UserRole.Admin)
        {
            if (teamId is null)
            {
                users = await userRepository.ListAllAsync(cancellationToken);
            }
            else
            {
                var team = await teamRepository.GetByIdWithMembersAsync(teamId.Value, cancellationToken);
                if (team is null)
                    return OperationResult<IReadOnlyList<UserSummaryDto>>.Fail(OperationFailureKind.NotFound);

                users = await userRepository.ListByTeamIdsAsync([teamId.Value], cancellationToken);
            }
        }
        else
        {
            var callerTeamIds = await userRepository.GetTeamIdsForUserAsync(callerId, cancellationToken);
            if (callerTeamIds.Count == 0)
                return OperationResult<IReadOnlyList<UserSummaryDto>>.Success([]);

            if (teamId is not null)
            {
                if (!callerTeamIds.Contains(teamId.Value))
                    return OperationResult<IReadOnlyList<UserSummaryDto>>.Fail(OperationFailureKind.Forbidden);

                var team = await teamRepository.GetByIdWithMembersAsync(teamId.Value, cancellationToken);
                if (team is null)
                    return OperationResult<IReadOnlyList<UserSummaryDto>>.Fail(OperationFailureKind.NotFound);

                users = await userRepository.ListByTeamIdsAsync([teamId.Value], cancellationToken);
            }
            else
            {
                users = await userRepository.ListByTeamIdsAsync(callerTeamIds, cancellationToken);
            }
        }

        var dtos = users.Select(u => new UserSummaryDto(
            u.Id,
            u.Email,
            u.FullName,
            u.Role.ToString())).ToList();

        return OperationResult<IReadOnlyList<UserSummaryDto>>.Success(dtos);
    }
}
