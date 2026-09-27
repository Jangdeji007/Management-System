using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Api;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is UnauthorizedUserException)
        {
            await WriteProblemAsync(
                httpContext,
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "The access token is invalid or missing required user information.",
                cancellationToken);
            return true;
        }

        logger.LogError(exception, "Unhandled exception processing {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        await WriteProblemAsync(
            httpContext,
            StatusCodes.Status500InternalServerError,
            "An error occurred",
            "An unexpected error occurred while processing your request.",
            cancellationToken);
        return true;
    }

    private static Task WriteProblemAsync(
        HttpContext httpContext,
        int statusCode,
        string title,
        string detail,
        CancellationToken cancellationToken)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };

        httpContext.Response.StatusCode = statusCode;
        return httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
    }
}
