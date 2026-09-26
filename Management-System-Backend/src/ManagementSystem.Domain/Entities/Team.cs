namespace ManagementSystem.Domain.Entities;

public class Team
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public Guid CreatedByUserId { get; set; }

    public User CreatedByUser { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public ICollection<TeamMember> Members { get; set; } = [];

    public ICollection<TaskItem> Tasks { get; set; } = [];
}
