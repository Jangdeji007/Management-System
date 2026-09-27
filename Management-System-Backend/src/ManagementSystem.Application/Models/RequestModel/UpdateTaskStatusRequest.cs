using System.ComponentModel.DataAnnotations;
using DomainTaskStatus = ManagementSystem.Domain.Enums.TaskStatus;

namespace ManagementSystem.Application.Models.RequestModel;

public class UpdateTaskStatusRequest
{
    [Required]
    public DomainTaskStatus Status { get; set; }
}
