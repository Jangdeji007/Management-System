using ManagementSystem.Api.Extensions;
using ManagementSystem.Application.Authorization;
using ManagementSystem.Application.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserQueryService userQueryService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.ManagerOrAdmin)]
    public async Task<IActionResult> ListUsers([FromQuery] Guid? teamId, CancellationToken cancellationToken)
    {
        var result = await userQueryService.ListUsersAsync(User.GetUserId(), User.GetUserRole(), teamId, cancellationToken);

        return result.ToActionResult(this);
    }
}
