using ManagementSystem.Domain.Entities;

namespace ManagementSystem.Application.Abstractions;

public interface ITeamRepository
{
    Task<IReadOnlyList<Team>> ListAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Team>> ListByMemberUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Team?> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Team?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> IsMemberAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(Team team, CancellationToken cancellationToken = default);

    Task UpdateAsync(Team team, CancellationToken cancellationToken = default);

    Task<bool> AddMemberAsync(TeamMember member, CancellationToken cancellationToken = default);

    Task<bool> RemoveMemberAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default);
}
