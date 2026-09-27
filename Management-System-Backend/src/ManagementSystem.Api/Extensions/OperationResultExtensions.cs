using ManagementSystem.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Api.Extensions;

public static class OperationResultExtensions
{
    public static IActionResult ToActionResult<T>(this OperationResult<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value);

        return result.Failure switch
        {
            OperationFailureKind.NotFound => controller.NotFound(new { title = "The requested resource was not found." }),
            OperationFailureKind.Forbidden => controller.StatusCode(
                StatusCodes.Status403Forbidden,
                new { title = "You are not allowed to perform this action.", status = StatusCodes.Status403Forbidden }),
            OperationFailureKind.Conflict => controller.Conflict(new { title = "The operation conflicts with the current state." }),
            OperationFailureKind.InvalidOperation => controller.BadRequest(new { title = "The operation is not valid." }),
            _ => controller.BadRequest(new { title = "The request could not be completed." })
        };
    }

    public static IActionResult ToCreatedResult<T>(
        this OperationResult<T> result,
        ControllerBase controller,
        string location)
    {
        if (result.IsSuccess)
            return controller.Created(location, result.Value);

        return result.ToActionResult(controller);
    }
}
