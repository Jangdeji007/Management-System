using ManagementSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManagementSystem.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "healthy" });

    [HttpGet("db")]
    public async Task<IActionResult> GetDatabase(CancellationToken cancellationToken)
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
        if (!canConnect)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { status = "database_unavailable" });
        }

        return Ok(new { status = "database_connected" });
    }
}
