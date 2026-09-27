using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Models.Mappings;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Api.Extensions;

public static class TaskCommentOperationResultExtensions
{
    public static IActionResult ToTaskCommentListActionResult(
        this OperationResult<IReadOnlyList<TaskCommentDto>> result,
        ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value!.ToResponse());

        return result.ToActionResult(controller);
    }

    public static IActionResult ToCreatedTaskCommentResult(
        this OperationResult<TaskCommentDto> result,
        ControllerBase controller,
        string location)
    {
        if (result.IsSuccess)
            return controller.Created(location, result.Value!.ToResponse());

        return result.ToActionResult(controller);
    }
}
