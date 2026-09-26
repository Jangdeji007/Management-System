using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Application.Models.RequestModel;

public class AddTeamMemberRequest
{
    [Required]
    public Guid UserId { get; set; }
}
