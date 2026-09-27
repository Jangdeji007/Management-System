using ManagementSystem.Application.Abstractions;
using ManagementSystem.Application.Common;
using ManagementSystem.Domain.Enums;

namespace ManagementSystem.Application.Services.Tasks;

public class TaskQueryScopeBuilder(IUserRepository userRepository)
{
    public async Task<TaskQueryFilter?> BuildScopeAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default)
    {
        return callerRole switch
        {
            UserRole.Admin => new TaskQueryFilter { ScopeAll = true },
            UserRole.Manager => new TaskQueryFilter
            {
                ScopeTeamIds = await userRepository.GetTeamIdsForUserAsync(callerId, cancellationToken)
            },
            UserRole.User => new TaskQueryFilter { ScopeAssigneeId = callerId },
            _ => null
        };
    }
}
