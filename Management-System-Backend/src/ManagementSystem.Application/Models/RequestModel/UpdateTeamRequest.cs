using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Application.Models.RequestModel;

public class UpdateTeamRequest
{
    [Required]
    [MaxLength(200)]
    public required string Name { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }
}
