using ManagementSystem.Api.Extensions;
using ManagementSystem.Application.Authorization;
using ManagementSystem.Application.Common;
using ManagementSystem.Application.Models.RequestModel;
using ManagementSystem.Application.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await authService.LoginAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return Unauthorized(new { title = "Invalid email or password." });

        return Ok(result.Value);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await authService.RegisterAsync(request, cancellationToken);
        if (result.Failure == AuthFailureKind.EmailAlreadyExists)
            return Conflict(new { title = "An account with this email already exists." });

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [HttpGet("me")]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var result = await authService.GetCurrentUserAsync(User.GetUserId(), cancellationToken);
        if (result.Failure == AuthFailureKind.UserNotFound)
            return NotFound(new { title = "User not found." });

        return Ok(result.Value);
    }
}
