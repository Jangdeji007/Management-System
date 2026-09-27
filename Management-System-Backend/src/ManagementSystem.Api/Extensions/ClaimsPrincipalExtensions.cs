using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ManagementSystem.Api;
using ManagementSystem.Domain.Enums;

namespace ManagementSystem.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var sub = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (sub is null || !Guid.TryParse(sub, out var userId))
            throw new UnauthorizedUserException("Authenticated user id claim is missing or invalid.");

        return userId;
    }

    public static UserRole GetUserRole(this ClaimsPrincipal principal)
    {
        var role = principal.FindFirstValue(ClaimTypes.Role);
        if (role is null || !Enum.TryParse<UserRole>(role, out var userRole))
            throw new UnauthorizedUserException("Authenticated user role claim is missing or invalid.");

        return userRole;
    }
}
