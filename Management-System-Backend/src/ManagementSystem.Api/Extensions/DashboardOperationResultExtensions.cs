using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Models.Mappings;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Api.Extensions;

public static class DashboardOperationResultExtensions
{
    public static IActionResult ToDashboardSummaryActionResult(
        this OperationResult<DashboardSummaryDto> result,
        ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value!.ToResponse());

        return result.ToActionResult(controller);
    }
}
