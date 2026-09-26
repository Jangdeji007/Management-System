using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Models.RequestModel;
using ManagementSystem.Domain.Enums;

namespace ManagementSystem.Application.Services.Teams;

public interface ITeamService
{
    Task<OperationResult<IReadOnlyList<TeamListItemDto>>> ListTeamsAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default);

    Task<OperationResult<TeamListItemDto>> CreateTeamAsync(
        Guid callerId,
        UserRole callerRole,
        CreateTeamRequest request,
        CancellationToken cancellationToken = default);

    Task<OperationResult<TeamListItemDto>> UpdateTeamAsync(
        Guid callerId,
        UserRole callerRole,
        Guid teamId,
        UpdateTeamRequest request,
        CancellationToken cancellationToken = default);

    Task<OperationResult<TeamListItemDto>> AddMemberAsync(
        Guid callerId,
        UserRole callerRole,
        Guid teamId,
        AddTeamMemberRequest request,
        CancellationToken cancellationToken = default);

    Task<OperationResult<TeamListItemDto>> RemoveMemberAsync(
        Guid callerId,
        UserRole callerRole,
        Guid teamId,
        Guid memberUserId,
        CancellationToken cancellationToken = default);
}
