using ManagementSystem.Application.Abstractions;
using ManagementSystem.Application.Common;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DomainTaskStatus = ManagementSystem.Domain.Enums.TaskStatus;

namespace ManagementSystem.Infrastructure.Persistence.Repositories;

public class TaskRepository(ApplicationDbContext context) : ITaskRepository
{
    private IQueryable<TaskItem> TasksForRead() =>
        context.TaskItems
            .AsNoTracking()
            .Include(t => t.Assignee)
            .Include(t => t.CreatedBy)
            .Include(t => t.Team)
            .Include(t => t.Comments.OrderBy(c => c.CreatedAt))
            .ThenInclude(c => c.Author);

    public async Task<IReadOnlyList<TaskItem>> ListAsync(
        TaskQueryFilter filter,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyScopeAndFilters(TasksForRead(), filter);

        return await query
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskStatusCountsDto> GetStatusCountsAsync(
        TaskQueryFilter filter,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyScopeAndFilters(context.TaskItems.AsNoTracking(), filter);

        var counts = await query
            .GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var toDo = counts.FirstOrDefault(c => c.Status == DomainTaskStatus.ToDo)?.Count ?? 0;
        var inProgress = counts.FirstOrDefault(c => c.Status == DomainTaskStatus.InProgress)?.Count ?? 0;
        var done = counts.FirstOrDefault(c => c.Status == DomainTaskStatus.Done)?.Count ?? 0;

        return new TaskStatusCountsDto(toDo, inProgress, done, toDo + inProgress + done);
    }

    public Task<TaskItem?> GetByIdForReadAsync(Guid id, CancellationToken cancellationToken = default) =>
        TasksForRead().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public Task<TaskItem?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.TaskItems.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        context.TaskItems.Add(task);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        context.TaskItems.Update(task);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        context.TaskItems.Remove(task);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<TaskItem> ApplyScopeAndFilters(IQueryable<TaskItem> query, TaskQueryFilter filter)
    {
        if (!filter.ScopeAll)
        {
            if (filter.ScopeAssigneeId is { } assigneeId)
                query = query.Where(t => t.AssigneeId == assigneeId);
            else if (filter.ScopeTeamIds is { Count: > 0 } teamIds)
                query = query.Where(t => t.TeamId != null && teamIds.Contains(t.TeamId.Value));
            else if (filter.ScopeTeamIds is { Count: 0 })
                query = query.Where(_ => false);
        }

        if (filter.Status is { } status)
            query = query.Where(t => t.Status == status);

        if (filter.Priority is { } priority)
            query = query.Where(t => t.Priority == priority);

        if (filter.AssigneeId is { } filterAssigneeId)
            query = query.Where(t => t.AssigneeId == filterAssigneeId);

        if (filter.TeamId is { } filterTeamId)
            query = query.Where(t => t.TeamId == filterTeamId);

        if (filter.DueBefore is { } dueBefore)
            query = query.Where(t => t.DueDate != null && t.DueDate <= dueBefore);

        if (filter.DueAfter is { } dueAfter)
            query = query.Where(t => t.DueDate != null && t.DueDate >= dueAfter);

        return query;
    }
}
