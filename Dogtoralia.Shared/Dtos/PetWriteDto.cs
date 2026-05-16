using System.ComponentModel.DataAnnotations;

namespace Dogtoralia.Shared.Dtos;

public class PetWriteDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Species { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Breed { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    [Required]
    public int PetOwnerId { get; set; }
}
