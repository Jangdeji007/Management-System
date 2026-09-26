using ManagementSystem.Domain.Enums;

namespace ManagementSystem.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public NotificationType Type { get; set; }

    public required string Message { get; set; }

    public Guid? RelatedTaskId { get; set; }

    public TaskItem? RelatedTask { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }
}
