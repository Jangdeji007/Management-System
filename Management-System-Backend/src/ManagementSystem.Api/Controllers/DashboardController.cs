using ManagementSystem.Api.Extensions;
using ManagementSystem.Application.Authorization;
using ManagementSystem.Application.Models.RequestModel;
using ManagementSystem.Application.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet("summary")]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    public async Task<IActionResult> GetSummary(
        [FromQuery] ListTasksQueryRequest query,
        CancellationToken cancellationToken)
    {
        var result = await dashboardService.GetSummaryAsync(
            User.GetUserId(),
            User.GetUserRole(),
            query,
            cancellationToken);

        return result.ToDashboardSummaryActionResult(this);
    }
}
