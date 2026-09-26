using ManagementSystem.Application.Abstractions;
using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Models.RequestModel;
using ManagementSystem.Domain.Entities;
using ManagementSystem.Domain.Enums;

namespace ManagementSystem.Application.Services.Teams;

public class TeamService(ITeamRepository teamRepository, IUserRepository userRepository) : ITeamService
{
    public async Task<OperationResult<IReadOnlyList<TeamListItemDto>>> ListTeamsAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default)
    {
        if (callerRole is UserRole.User)
            return OperationResult<IReadOnlyList<TeamListItemDto>>.Fail(OperationFailureKind.Forbidden);

        var teams = callerRole == UserRole.Admin
            ? await teamRepository.ListAllAsync(cancellationToken)
            : await teamRepository.ListByMemberUserIdAsync(callerId, cancellationToken);

        var dtos = teams.Select(MapTeam).ToList();
        return OperationResult<IReadOnlyList<TeamListItemDto>>.Success(dtos);
    }

    public async Task<OperationResult<TeamListItemDto>> CreateTeamAsync(
        Guid callerId,
        UserRole callerRole,
        CreateTeamRequest request,
        CancellationToken cancellationToken = default)
    {
        if (callerRole != UserRole.Admin)
            return OperationResult<TeamListItemDto>.Fail(OperationFailureKind.Forbidden);

        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim(),
            CreatedByUserId = callerId,
            CreatedAt = DateTime.UtcNow
        };

        await teamRepository.AddAsync(team, cancellationToken);

        var created = await teamRepository.GetByIdWithMembersAsync(team.Id, cancellationToken);
        return OperationResult<TeamListItemDto>.Success(MapTeam(created!));
    }

    public async Task<OperationResult<TeamListItemDto>> UpdateTeamAsync(
        Guid callerId,
        UserRole callerRole,
        Guid teamId,
        UpdateTeamRequest request,
        CancellationToken cancellationToken = default)
    {
        if (callerRole != UserRole.Admin)
            return OperationResult<TeamListItemDto>.Fail(OperationFailureKind.Forbidden);

        var team = await teamRepository.GetByIdForUpdateAsync(teamId, cancellationToken);
        if (team is null)
            return OperationResult<TeamListItemDto>.Fail(OperationFailureKind.NotFound);

        team.Name = request.Name.Trim();
        team.Description = string.IsNullOrWhiteSpace(request.Description)
            ? null
            : request.Description.Trim();

        await teamRepository.UpdateAsync(team, cancellationToken);

        var updated = await teamRepository.GetByIdWithMembersAsync(teamId, cancellationToken);
        return OperationResult<TeamListItemDto>.Success(MapTeam(updated!));
    }

    public async Task<OperationResult<TeamListItemDto>> AddMemberAsync(
        Guid callerId,
        UserRole callerRole,
        Guid teamId,
        AddTeamMemberRequest request,
        CancellationToken cancellationToken = default)
    {
        if (callerRole is UserRole.User)
            return OperationResult<TeamListItemDto>.Fail(OperationFailureKind.Forbidden);

        var team = await teamRepository.GetByIdWithMembersAsync(teamId, cancellationToken);
        if (team is null)
            return OperationResult<TeamListItemDto>.Fail(OperationFailureKind.NotFound);

        if (!await CanManageTeamMembersAsync(callerId, callerRole, teamId, cancellationToken))
            return OperationResult<TeamListItemDto>.Fail(OperationFailureKind.Forbidden);

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return OperationResult<TeamListItemDto>.Fail(OperationFailureKind.NotFound);

        var member = new TeamMember
        {
            TeamId = teamId,
            UserId = request.UserId,
            JoinedAt = DateTime.UtcNow
        };

        var added = await teamRepository.AddMemberAsync(member, cancellationToken);
        if (!added)
            return OperationResult<TeamListItemDto>.Fail(OperationFailureKind.Conflict);

        var updated = await teamRepository.GetByIdWithMembersAsync(teamId, cancellationToken);
        return OperationResult<TeamListItemDto>.Success(MapTeam(updated!));
    }

    public async Task<OperationResult<TeamListItemDto>> RemoveMemberAsync(
        Guid callerId,
        UserRole callerRole,
        Guid teamId,
        Guid memberUserId,
        CancellationToken cancellationToken = default)
    {
        if (callerRole is UserRole.User)
            return OperationResult<TeamListItemDto>.Fail(OperationFailureKind.Forbidden);

        var team = await teamRepository.GetByIdWithMembersAsync(teamId, cancellationToken);
        if (team is null)
            return OperationResult<TeamListItemDto>.Fail(OperationFailureKind.NotFound);

        if (!await CanManageTeamMembersAsync(callerId, callerRole, teamId, cancellationToken))
            return OperationResult<TeamListItemDto>.Fail(OperationFailureKind.Forbidden);

        var removed = await teamRepository.RemoveMemberAsync(teamId, memberUserId, cancellationToken);
        if (!removed)
            return OperationResult<TeamListItemDto>.Fail(OperationFailureKind.NotFound);

        var updated = await teamRepository.GetByIdWithMembersAsync(teamId, cancellationToken);
        return OperationResult<TeamListItemDto>.Success(MapTeam(updated!));
    }

    private async Task<bool> CanManageTeamMembersAsync(
        Guid callerId,
        UserRole callerRole,
        Guid teamId,
        CancellationToken cancellationToken)
    {
        if (callerRole == UserRole.Admin)
            return true;

        return await teamRepository.IsMemberAsync(teamId, callerId, cancellationToken);
    }

    private static TeamListItemDto MapTeam(Team team) =>
        new(
            team.Id,
            team.Name,
            team.Description,
            team.CreatedByUserId,
            team.CreatedAt,
            team.Members
                .OrderBy(m => m.User.FullName)
                .Select(m => new TeamMemberDto(
                    m.UserId,
                    m.User.Email,
                    m.User.FullName,
                    m.User.Role.ToString(),
                    m.JoinedAt))
                .ToList());
}
