using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace ManagementSystem.Api;

public static class AuthorizationResultHandlerExtensions
{
    public static IServiceCollection AddJsonAuthorizationResponses(this IServiceCollection services) =>
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, JsonAuthorizationMiddlewareResultHandler>();
}

public sealed class JsonAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Forbidden)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                title = "You are not allowed to perform this action.",
                status = StatusCodes.Status403Forbidden
            });
            return;
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
