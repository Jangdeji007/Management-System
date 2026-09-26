using ManagementSystem.Domain.Enums;

namespace ManagementSystem.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public required string FullName { get; set; }

    public UserRole Role { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<TeamMember> TeamMemberships { get; set; } = [];

    public ICollection<Team> TeamsCreated { get; set; } = [];

    public ICollection<TaskItem> AssignedTasks { get; set; } = [];

    public ICollection<TaskItem> CreatedTasks { get; set; } = [];

    public ICollection<TaskComment> Comments { get; set; } = [];

    public ICollection<Notification> Notifications { get; set; } = [];
}
