using ManagementSystem.Domain.Enums;
using DomainTaskStatus = ManagementSystem.Domain.Enums.TaskStatus;

namespace ManagementSystem.Application.Common;

public sealed record TaskQueryFilter
{
    public bool ScopeAll { get; init; }

    public Guid? ScopeAssigneeId { get; init; }

    public IReadOnlyCollection<Guid>? ScopeTeamIds { get; init; }

    public DomainTaskStatus? Status { get; init; }

    public TaskPriority? Priority { get; init; }

    public Guid? AssigneeId { get; init; }

    public Guid? TeamId { get; init; }

    public DateTime? DueBefore { get; init; }

    public DateTime? DueAfter { get; init; }
}
