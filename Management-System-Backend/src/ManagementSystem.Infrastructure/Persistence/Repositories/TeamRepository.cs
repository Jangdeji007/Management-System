using ManagementSystem.Application.Abstractions;
using ManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManagementSystem.Infrastructure.Persistence.Repositories;

public class TeamRepository(ApplicationDbContext context) : ITeamRepository
{
    private IQueryable<Team> TeamsWithMembers() =>
        context.Teams
            .AsNoTracking()
            .Include(t => t.Members)
            .ThenInclude(m => m.User);

    public async Task<IReadOnlyList<Team>> ListAllAsync(CancellationToken cancellationToken = default) =>
        await TeamsWithMembers()
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Team>> ListByMemberUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await TeamsWithMembers()
            .Where(t => t.Members.Any(m => m.UserId == userId))
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

    public Task<Team?> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken = default) =>
        TeamsWithMembers().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public Task<Team?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Teams.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public Task<bool> IsMemberAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default) =>
        context.TeamMembers.AnyAsync(tm => tm.TeamId == teamId && tm.UserId == userId, cancellationToken);

    public async Task AddAsync(Team team, CancellationToken cancellationToken = default)
    {
        context.Teams.Add(team);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Team team, CancellationToken cancellationToken = default)
    {
        context.Teams.Update(team);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> AddMemberAsync(TeamMember member, CancellationToken cancellationToken = default)
    {
        var exists = await context.TeamMembers.AnyAsync(
            tm => tm.TeamId == member.TeamId && tm.UserId == member.UserId,
            cancellationToken);

        if (exists)
            return false;

        context.TeamMembers.Add(member);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RemoveMemberAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default)
    {
        var member = await context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId, cancellationToken);

        if (member is null)
            return false;

        context.TeamMembers.Remove(member);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
