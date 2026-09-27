using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Application.Models.RequestModel;

public class CreateTaskCommentRequest
{
    [Required]
    [MaxLength(4000)]
    public required string Body { get; set; }
}
