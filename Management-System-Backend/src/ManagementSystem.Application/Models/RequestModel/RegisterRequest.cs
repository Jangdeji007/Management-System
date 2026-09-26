using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Application.Models.RequestModel;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [MinLength(8)]
    public required string Password { get; set; }

    [Required]
    [MaxLength(200)]
    public required string FullName { get; set; }
}
