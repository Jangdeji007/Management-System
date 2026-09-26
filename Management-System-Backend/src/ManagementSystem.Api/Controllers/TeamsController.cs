using ManagementSystem.Api.Extensions;
using ManagementSystem.Application.Authorization;
using ManagementSystem.Application.Models.RequestModel;
using ManagementSystem.Application.Services.Teams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Api.Controllers;

[ApiController]
[Route("api/teams")]
public class TeamsController(ITeamService teamService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.ManagerOrAdmin)]
    public async Task<IActionResult> ListTeams(CancellationToken cancellationToken)
    {
        var result = await teamService.ListTeamsAsync(
            User.GetUserId(),
            User.GetUserRole(),
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> CreateTeam(
        [FromBody] CreateTeamRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await teamService.CreateTeamAsync(
            User.GetUserId(),
            User.GetUserRole(),
            request,
            cancellationToken);

        if (!result.IsSuccess)
            return result.ToActionResult(this);

        return Created($"/api/teams/{result.Value!.Id}", result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> UpdateTeam(
        Guid id,
        [FromBody] UpdateTeamRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await teamService.UpdateTeamAsync(
            User.GetUserId(),
            User.GetUserRole(),
            id,
            request,
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPost("{id:guid}/members")]
    [Authorize(Policy = AuthorizationPolicies.ManagerOrAdmin)]
    public async Task<IActionResult> AddMember(
        Guid id,
        [FromBody] AddTeamMemberRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await teamService.AddMemberAsync(
            User.GetUserId(),
            User.GetUserRole(),
            id,
            request,
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpDelete("{id:guid}/members/{userId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.ManagerOrAdmin)]
    public async Task<IActionResult> RemoveMember(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await teamService.RemoveMemberAsync(
            User.GetUserId(),
            User.GetUserRole(),
            id,
            userId,
            cancellationToken);

        return result.ToActionResult(this);
    }
}
