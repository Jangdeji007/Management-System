using ManagementSystem.Application.Abstractions;
using ManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManagementSystem.Infrastructure.Persistence.Repositories;

public class TaskCommentRepository(ApplicationDbContext context) : ITaskCommentRepository
{
    public async Task<IReadOnlyList<TaskComment>> ListByTaskIdAsync(
        Guid taskId,
        CancellationToken cancellationToken = default) =>
        await context.TaskComments
            .AsNoTracking()
            .Include(c => c.Author)
            .Where(c => c.TaskId == taskId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(TaskComment comment, CancellationToken cancellationToken = default)
    {
        context.TaskComments.Add(comment);
        await context.SaveChangesAsync(cancellationToken);
    }
}
