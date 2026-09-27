using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Models.Mappings;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Api.Extensions;

public static class TaskOperationResultExtensions
{
    public static IActionResult ToTaskListActionResult(
        this OperationResult<IReadOnlyList<TaskListItemDto>> result,
        ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value!.ToResponse());

        return result.ToActionResult(controller);
    }

    public static IActionResult ToTaskDetailActionResult(
        this OperationResult<TaskDetailDto> result,
        ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value!.ToResponse());

        return result.ToActionResult(controller);
    }

    public static IActionResult ToCreatedTaskDetailResult(
        this OperationResult<TaskDetailDto> result,
        ControllerBase controller,
        string location)
    {
        if (result.IsSuccess)
            return controller.Created(location, result.Value!.ToResponse());

        return result.ToActionResult(controller);
    }
}
