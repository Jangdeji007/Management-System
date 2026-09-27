using System.ComponentModel.DataAnnotations;
using ManagementSystem.Domain.Enums;

namespace ManagementSystem.Application.Models.RequestModel;

public class UpdateTaskRequest
{
    [Required]
    [MaxLength(300)]
    public required string Title { get; set; }

    [MaxLength(4000)]
    public string? Description { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime? DueDate { get; set; }

    [Required]
    public Guid AssigneeId { get; set; }

    public Guid? TeamId { get; set; }
}
