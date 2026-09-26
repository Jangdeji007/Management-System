using DomainTaskStatus = ManagementSystem.Domain.Enums.TaskStatus;

namespace ManagementSystem.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public DomainTaskStatus Status { get; set; }

    public Enums.TaskPriority Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid AssigneeId { get; set; }

    public User Assignee { get; set; } = null!;

    public Guid CreatedById { get; set; }

    public User CreatedBy { get; set; } = null!;

    public Guid? TeamId { get; set; }

    public Team? Team { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<TaskComment> Comments { get; set; } = [];
}
