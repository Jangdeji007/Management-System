using ManagementSystem.Api.Extensions;
using ManagementSystem.Application.Authorization;
using ManagementSystem.Application.Services.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Api.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController(INotificationService notificationService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    public async Task<IActionResult> ListNotifications(
        [FromQuery] bool? unreadOnly,
        CancellationToken cancellationToken)
    {
        var result = await notificationService.ListForCurrentUserAsync(
            User.GetUserId(),
            unreadOnly,
            cancellationToken);

        return result.ToNotificationListActionResult(this);
    }

    [HttpPatch("{id:guid}/read")]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken)
    {
        var result = await notificationService.MarkReadAsync(
            User.GetUserId(),
            id,
            cancellationToken);

        return result.ToNotificationActionResult(this);
    }
}
