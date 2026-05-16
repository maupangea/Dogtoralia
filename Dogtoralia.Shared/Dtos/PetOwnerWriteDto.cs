using System.ComponentModel.DataAnnotations;

namespace Dogtoralia.Shared.Dtos;

public class PetOwnerWriteDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
}
