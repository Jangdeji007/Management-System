using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Models.Mappings;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Api.Extensions;

public static class NotificationOperationResultExtensions
{
    public static IActionResult ToNotificationListActionResult(
        this OperationResult<IReadOnlyList<NotificationDto>> result,
        ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value!.ToResponse());

        return result.ToActionResult(controller);
    }

    public static IActionResult ToNotificationActionResult(
        this OperationResult<NotificationDto> result,
        ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value!.ToResponse());

        return result.ToActionResult(controller);
    }
}
