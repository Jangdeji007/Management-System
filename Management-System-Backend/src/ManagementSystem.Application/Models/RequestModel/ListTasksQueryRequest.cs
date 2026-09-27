using ManagementSystem.Domain.Enums;
using DomainTaskStatus = ManagementSystem.Domain.Enums.TaskStatus;

namespace ManagementSystem.Application.Models.RequestModel;

public sealed class ListTasksQueryRequest
{
    public DomainTaskStatus? Status { get; set; }

    public TaskPriority? Priority { get; set; }

    public Guid? AssigneeId { get; set; }

    public Guid? TeamId { get; set; }

    public DateTime? DueBefore { get; set; }

    public DateTime? DueAfter { get; set; }
}
