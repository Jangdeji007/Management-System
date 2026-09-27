using ManagementSystem.Api.Extensions;
using ManagementSystem.Application.Authorization;
using ManagementSystem.Application.Models.RequestModel;
using ManagementSystem.Application.Services.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController(ITaskService taskService, ITaskCommentService taskCommentService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    public async Task<IActionResult> ListTasks(
        [FromQuery] ListTasksQueryRequest query,
        CancellationToken cancellationToken)
    {
        var result = await taskService.ListTasksAsync(
            User.GetUserId(),
            User.GetUserRole(),
            query,
            cancellationToken);

        return result.ToTaskListActionResult(this);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    public async Task<IActionResult> GetTask(Guid id, CancellationToken cancellationToken)
    {
        var result = await taskService.GetTaskAsync(
            User.GetUserId(),
            User.GetUserRole(),
            id,
            cancellationToken);

        return result.ToTaskDetailActionResult(this);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.ManagerOrAdmin)]
    public async Task<IActionResult> CreateTask(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await taskService.CreateTaskAsync(
            User.GetUserId(),
            User.GetUserRole(),
            request,
            cancellationToken);

        if (!result.IsSuccess)
            return result.ToActionResult(this);

        return result.ToCreatedTaskDetailResult(this, $"/api/tasks/{result.Value!.Id}");
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.ManagerOrAdmin)]
    public async Task<IActionResult> UpdateTask(
        Guid id,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await taskService.UpdateTaskAsync(
            User.GetUserId(),
            User.GetUserRole(),
            id,
            request,
            cancellationToken);

        return result.ToTaskDetailActionResult(this);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    public async Task<IActionResult> UpdateTaskStatus(
        Guid id,
        [FromBody] UpdateTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await taskService.UpdateTaskStatusAsync(
            User.GetUserId(),
            User.GetUserRole(),
            id,
            request,
            cancellationToken);

        return result.ToTaskDetailActionResult(this);
    }

    [HttpGet("{id:guid}/comments")]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    public async Task<IActionResult> ListTaskComments(Guid id, CancellationToken cancellationToken)
    {
        var result = await taskCommentService.ListCommentsAsync(
            User.GetUserId(),
            User.GetUserRole(),
            id,
            cancellationToken);

        return result.ToTaskCommentListActionResult(this);
    }

    [HttpPost("{id:guid}/comments")]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    public async Task<IActionResult> AddTaskComment(
        Guid id,
        [FromBody] CreateTaskCommentRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await taskCommentService.AddCommentAsync(
            User.GetUserId(),
            User.GetUserRole(),
            id,
            request,
            cancellationToken);

        return result.ToCreatedTaskCommentResult(this, $"/api/tasks/{id}/comments/{result.Value?.Id}");
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.ManagerOrAdmin)]
    public async Task<IActionResult> DeleteTask(Guid id, CancellationToken cancellationToken)
    {
        var result = await taskService.DeleteTaskAsync(
            User.GetUserId(),
            User.GetUserRole(),
            id,
            cancellationToken);

        if (!result.IsSuccess)
            return result.ToActionResult(this);

        return NoContent();
    }
}
